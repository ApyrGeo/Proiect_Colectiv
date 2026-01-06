using System.Text.Json;
using log4net;
using TrackForUBB.Controller.Interfaces;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Enums;
using TrackForUBB.Domain.Exceptions.Custom;
using TrackForUBB.Domain.Utils;
using TrackForUBB.Service.EmailService.Interfaces;
using TrackForUBB.Service.EmailService.Models;
using TrackForUBB.Service.Interfaces;
using TrackForUBB.Service.Utils;

namespace TrackForUBB.Service;

public class GradeService(IGradeRepository gradeRepository, IUserRepository userRepository, IAcademicRepository academicRepository, IValidatorFactory validatorFactory, IEmailProvider emailProvider) : IGradeService
{
    private readonly IGradeRepository _gradeRepository = gradeRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IAcademicRepository _academicRepository = academicRepository;

    private readonly ILog _logger = LogManager.GetLogger(typeof(GradeService));
    private readonly IValidatorFactory _validatorFactory = validatorFactory;
    private readonly IEmailProvider _emailProvider = emailProvider;

    public async Task<GradeResponseDTO> CreateGrade(int teacherId, GradePostDTO gradePostDto)
    {
        _logger.InfoFormat("Validating GradePostDTO: {0}", JsonSerializer.Serialize(gradePostDto));
        var validator = _validatorFactory.Get<GradePostDTO>();
        var validationResult = await validator.ValidateAsync(gradePostDto);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(ValidationHelper.ConvertErrorsToListOfStrings(validationResult.Errors));
        }

        var teacher = await _academicRepository.GetTeacherByUserId(teacherId)
                ?? throw new NotFoundException("Teacher not found.");
        
        bool teaches = await _gradeRepository.TeacherTeachesSubjectAsync(teacher.Id, gradePostDto.SubjectId);
        if (!teaches)
            throw new  UnauthorizedAccessException("Teacher does not teach this subject.");
        
        _logger.InfoFormat("Adding new grade : {0}", JsonSerializer.Serialize(gradePostDto));
        var gradeDto = await _gradeRepository.AddGradeAsync(gradePostDto);
        await CheckIfSemesterCompletedAndSendEmail(gradeDto);
        return gradeDto;
    }
    
    public async Task<GradeResponseDTO> UpdateGradeAsync(int teacherId, int gradeId, GradePostDTO dto)
    {
        var teacher = await _academicRepository.GetTeacherByUserId(teacherId)
                      ?? throw new NotFoundException("Teacher not found.");

        var grade = await _gradeRepository.GetGradeByIdAsync(gradeId)
                    ?? throw new NotFoundException($"Grade with ID {gradeId} not found.");

        bool teaches = await _gradeRepository.TeacherTeachesSubjectAsync(teacher.Id, dto.SubjectId);
        if (!teaches)
            throw new UnauthorizedAccessException("Teacher does not teach this subject.");

        var updated = await _gradeRepository.UpdateGradeAsync(gradeId, dto);

        return updated;
    }
    
    public async Task<GradeResponseDTO> PatchGradeAsync(int teacherId, int gradeId, int newValue)
    {
        var teacher = await _academicRepository.GetTeacherByUserId(teacherId)
                      ?? throw new NotFoundException("Teacher not found.");

        var grade = await _gradeRepository.GetGradeByIdAsync(gradeId)
                    ?? throw new NotFoundException($"Grade with ID {gradeId} not found.");

        var updated = await _gradeRepository.PatchGradeValueAsync(gradeId, newValue);

        return updated;
    }
    
    public async Task<List<GradeResponseDTO>> GetGradesFiteredAsync(int userId, int? yearOfStudy, int? semester, string specialisation)
    {
        _logger.InfoFormat("Trying to retrieve filtered grades for user with ID {0}, year {1}, semester {2}", 
            userId, yearOfStudy, semester);
        
        var _ = await _userRepository.GetByIdAsync(userId)
                ?? throw new NotFoundException($"Student with ID {userId} not found.");
        
        var gradesDto = await _gradeRepository.GetGradesFilteredAsync(userId, yearOfStudy, semester, specialisation)
                        ?? throw new NotFoundException($"Filtered grades for user with ID {userId} not found.");

        _logger.InfoFormat("Mapping filtered grade entity to DTO for user with ID {0}", userId);

        return gradesDto;
    }

    public async Task<GradeResponseDTO> GetGradeByIdAsync(int gradeId)
    {
        _logger.InfoFormat("Trying to retrieve grade with ID {0}", gradeId);

        var grade = await _gradeRepository.GetGradeByIdAsync(gradeId);

        if (grade == null)
            throw new NotFoundException($"Grade with ID {gradeId} not found.");

        return grade;
    }

    public async Task<ScholarshipStatusDTO?> GetUserAverageScoreAndScholarshipStatusAsync(int userId, int yearOfstudy, int semester, string specialisation)
    {
        var userGrades = await _gradeRepository.GetGradesFilteredAsync(userId, yearOfstudy, semester, specialisation)
                     ?? throw new NotFoundException($"Grades for user with ID {userId} not found.");
        if (userGrades.Count == 0)
            return null;

        var userAverage = userGrades.Average(g => g.Value);

        var otherGrades = await _gradeRepository.GetGradesFilteredAsync(null, yearOfstudy, semester, specialisation)
                          ?? [];

        var averagesOrdered = otherGrades
            .GroupBy(g => g.Enrollment.UserId)
            .Select(g => new { UserId = g.Key, Average = g.Average(grade => grade.Value) })
            .OrderByDescending(x => x.Average)
            .ToList();

        int totalStudents = averagesOrdered.Count;

        int rank1Positions = (int)Math.Floor(HardcodedData.percentageScholarshipType1 * totalStudents);
        int rank2TotalPositions = (int)Math.Floor(HardcodedData.percentageScholarshipType2 * totalStudents);
        int rank2Positions = Math.Max(0, rank2TotalPositions - rank1Positions);

        const double EPS = 1e-6;
        var distinctHigherCount = averagesOrdered
            .Select(a => a.Average)
            .Count(avg => avg - userAverage > EPS);

        int userRank = distinctHigherCount + 1;

        string? scholarshipType = null;
        bool isEligible = false;
        if (userRank <= rank1Positions && rank1Positions > 0)
        {
            scholarshipType = "Type1";
            isEligible = true;
        }
        else if (userRank <= rank1Positions + rank2Positions && (rank1Positions + rank2Positions) > 0)
        {
            scholarshipType = "Type2";
            isEligible = true;
        }

        var result = new ScholarshipStatusDTO
        {
            AverageScore = userAverage,
            Rank = userRank,
            TotalStudents = totalStudents,
            IsEligible = isEligible,
            ScholarshipType = scholarshipType,
        };

        return result;
    }

    private async Task CheckIfSemesterCompletedAndSendEmail(GradeResponseDTO grade)
    {
        var subjectsInSemester = await _gradeRepository.GetSubjectsForSemesterAsync(grade.Semester.Id);
        var gradesForSemester = await _gradeRepository.GetGradesForStudentInSemesterAsync(grade.Enrollment.UserId, grade.Semester.Id);
       
        if (gradesForSemester.Count != subjectsInSemester.Count)
        {
            _logger.Info("Semester not fully graded yet.");
            return;
        }
        _logger.Info("All grades posted, preparing email");
      

        var gradesSemester = new PostedSemesterGradesModel
        {
            UserFirstName = grade.Enrollment.User.FirstName,
            UserLastName = grade.Enrollment.User.LastName,
            YearOfStudy = grade.Semester.PromotionYear.YearNumber,
            SemesterNumber = grade.Semester.SemesterNumber,
        };
        
        await _emailProvider.SendSemesterGradesEmailAsync(grade.Enrollment.User.Email,gradesSemester);
    }
}
