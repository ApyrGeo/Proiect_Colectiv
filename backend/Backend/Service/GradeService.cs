using System.Text.Json;
using log4net;
using TrackForUBB.Controller.Interfaces;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Enums;
using TrackForUBB.Domain.Exceptions.Custom;
using TrackForUBB.Service.EmailService.Interfaces;
using TrackForUBB.Service.EmailService.Models;
using TrackForUBB.Service.Interfaces;
using TrackForUBB.Service.Utils;

namespace TrackForUBB.Service;

public class GradeService(IGradeRepository gradeRepository, IUserRepository userRepository, IValidatorFactory validatorFactory, IEmailProvider emailProvider) : IGradeService
{
    private readonly IGradeRepository _gradeRepository = gradeRepository;
    private readonly IUserRepository _userRepository = userRepository;

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

        var teacher = await _userRepository.GetByIdAsync(teacherId)
                ?? throw new NotFoundException("Teacher not found.");
        
        if (teacher.Role != UserRole.Teacher.ToString())
        {
            throw new UnauthorizedAccessException("Only teachers can add grades.");
        }

        _logger.InfoFormat("Adding new grade : {0}", JsonSerializer.Serialize(gradePostDto));
        var gradeDto = await _gradeRepository.AddGradeAsync(gradePostDto);
        await _gradeRepository.SaveChangesAsync();
        await CheckIfSemesterCompletedAndSendEmail(gradeDto);
        return gradeDto;
    }
    

    public async Task<List<GradeResponseDTO>> GetGradesFiteredAsync(int userId, int? yearOfStudy, int? semester)
    {
        _logger.InfoFormat("Trying to retrieve filtered grades for user with ID {0}, year {1}, semester {2}", 
            userId, yearOfStudy, semester);
        
        var _ = await _userRepository.GetByIdAsync(userId)
                ?? throw new NotFoundException($"Student with ID {userId} not found.");
        
        var gradesDto = await _gradeRepository.GetGradesFilteredAsync(userId, yearOfStudy, semester)
                        ?? throw new NotFoundException($"Filtered grades for user with ID {userId} not found.");

        _logger.InfoFormat("Mapping filtered grade entity to DTO for user with ID {0}", userId);

        return gradesDto;
    }

    private async Task CheckIfSemesterCompletedAndSendEmail(GradeResponseDTO grade)
    {
        var subjectsInSemester = await _gradeRepository.GetSubjectsForSemesterAsync(grade.PromotionSemester.Id);
        var gradesForSemester = await _gradeRepository.GetGradesForStudentInSemesterAsync(grade.Enrollment.UserId, grade.PromotionSemester.Id);
       
        if (gradesForSemester.Count != subjectsInSemester.Count)
        {
            _logger.Info("Semester not fully graded yet.");
            return;
        }
        _logger.Info("All grades posted → preparing email");
      

        var gradesSemester = new PostedSemesterGradesModel
        {
            UserFirstName = grade.Enrollment.User.FirstName,
            UserLastName = grade.Enrollment.User.LastName,
            YearOfStudy = grade.PromotionSemester.PromotionYear.YearNumber,
            SemesterNumber = grade.PromotionSemester.SemesterNumber,
        };
        
        await _emailProvider.SendSemesterGradesEmailAsync(grade.Enrollment.User.Email,gradesSemester);
    }
}


