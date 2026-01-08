using log4net;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Exceptions.Custom;
using TrackForUBB.Service.Interfaces;
using IValidatorFactory = TrackForUBB.Service.Interfaces.IValidatorFactory;
using System.Text.Json;
using TrackForUBB.Service.Utils;
using TrackForUBB.Service.EmailService.Interfaces;
using TrackForUBB.Service.EmailService.Models;
using TrackForUBB.Controller.Interfaces;
using TrackForUBB.Domain.Enums;

namespace TrackForUBB.Service;

public class AcademicsService(IAcademicRepository academicRepository, IUserRepository userRepository, IValidatorFactory validatorFactory, IEmailProvider emailProvider) : IAcademicsService
{
    private readonly IAcademicRepository _academicRepository = academicRepository;
    private readonly IUserRepository _userRepository = userRepository;

    private readonly ILog _logger = LogManager.GetLogger(typeof(AcademicsService));
    private readonly IValidatorFactory _validatorFactory = validatorFactory;
    private readonly IEmailProvider _emailProvider = emailProvider;

    public async Task<FacultyResponseDTO> CreateFaculty(FacultyPostDTO facultyPostDto)
    {
        _logger.InfoFormat("Validating FacultyPostDTO: {0}", JsonSerializer.Serialize(facultyPostDto));
        var validator = _validatorFactory.Get<FacultyPostDTO>();
        var validationResult = await validator.ValidateAsync(facultyPostDto);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(ValidationHelper.ConvertErrorsToListOfStrings(validationResult.Errors));
        }

        _logger.InfoFormat("Adding new faculty to repository: {0}", JsonSerializer.Serialize(facultyPostDto));
        var facultyDto = await _academicRepository.AddFacultyAsync(facultyPostDto);

        return facultyDto;
    }

    public async Task<PromotionResponseDTO> CreatePromotion(PromotionPostDTO promotionPostDTO)
    {
        _logger.InfoFormat("Validating PromotionPostDTO: {0}", JsonSerializer.Serialize(promotionPostDTO));
        var validator = _validatorFactory.Get<PromotionPostDTO>();
        var validationResult = await validator.ValidateAsync(promotionPostDTO);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(ValidationHelper.ConvertErrorsToListOfStrings(validationResult.Errors));
        }

        _logger.InfoFormat("Adding new group year to repository: {0}", JsonSerializer.Serialize(promotionPostDTO));
        var groupYearDto = await _academicRepository.AddPromotionAsync(promotionPostDTO);

        return groupYearDto;
    }

    public async Task<SpecialisationResponseDTO> CreateSpecialisation(SpecialisationPostDTO specialisationPostDto)
    {
        _logger.InfoFormat("Validating SpecialisationPostDTO: {0}", JsonSerializer.Serialize(specialisationPostDto));
        var validator = _validatorFactory.Get<SpecialisationPostDTO>();
        var validationResult = await validator.ValidateAsync(specialisationPostDto);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(ValidationHelper.ConvertErrorsToListOfStrings(validationResult.Errors));
        }

        _logger.InfoFormat("Adding new specialisation to repository: {0}", JsonSerializer.Serialize(specialisationPostDto));
        var specialisationDto = await _academicRepository.AddSpecialisationAsync(specialisationPostDto);

        return specialisationDto;
    }

    public async Task<StudentGroupResponseDTO> CreateStudentGroup(StudentGroupPostDTO studentGroupPostDto)
    {
        _logger.InfoFormat("Validating StudentGroupPostDTO: {0}", JsonSerializer.Serialize(studentGroupPostDto));
        var validator = _validatorFactory.Get<StudentGroupPostDTO>();
        var validationResult = await validator.ValidateAsync(studentGroupPostDto);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(ValidationHelper.ConvertErrorsToListOfStrings(validationResult.Errors));
        }

        _logger.InfoFormat("Adding new student group to repository: {0}", JsonSerializer.Serialize(studentGroupPostDto));
		var studentGroupDto = await _academicRepository.AddGroupAsync(studentGroupPostDto);

        return studentGroupDto;
    }

    public async Task<StudentSubGroupResponseDTO> CreateStudentSubGroup(StudentSubGroupPostDTO studentSubGroupPostDto)
    {
        _logger.InfoFormat("Validating StudentSubGroupPostDTO: {0}", JsonSerializer.Serialize(studentSubGroupPostDto));
        var validator = _validatorFactory.Get<StudentSubGroupPostDTO>();
        var validationResult = await validator.ValidateAsync(studentSubGroupPostDto);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(ValidationHelper.ConvertErrorsToListOfStrings(validationResult.Errors));
        }

        _logger.InfoFormat("Adding new student sub-group to repository: {0}", JsonSerializer.Serialize(studentSubGroupPostDto));
		var studentSubGroupDto = await _academicRepository.AddSubGroupAsync(studentSubGroupPostDto);

        return studentSubGroupDto;
    }

    public async Task<EnrollmentResponseDTO> CreateUserEnrollment(EnrollmentPostDTO enrollmentPostDto)
    {
        _logger.InfoFormat("Validating EnrollmentPostDTO: {0}", JsonSerializer.Serialize(enrollmentPostDto));
        var validator = _validatorFactory.Get<EnrollmentPostDTO>();
        var validationResult = await validator.ValidateAsync(enrollmentPostDto);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(ValidationHelper.ConvertErrorsToListOfStrings(validationResult.Errors));
        }

        _logger.InfoFormat("Adding new enrollment to repository: {0}", JsonSerializer.Serialize(enrollmentPostDto));
        var enrollmentDto = await _academicRepository.AddEnrollmentAsync(enrollmentPostDto);

        _logger.InfoFormat($"Sending email to: {enrollmentDto.User.Email}");
        await SendAddedEnrollementEmail(enrollmentDto);

        _logger.InfoFormat("Mapping enrollment entity to DTO for user with ID {0}", enrollmentPostDto.UserId);

        return enrollmentDto;
    }

    private async Task SendAddedEnrollementEmail(EnrollmentResponseDTO enrollment)
    {
        var enrollmentModel = new CreatedEnrollmentModel
        {
            UserFirstName = enrollment.User.FirstName,
            UserLastName = enrollment.User.LastName,
            GroupName = enrollment.SubGroup.Name
        };

        await _emailProvider.SendCreateEnrollmentEmailAsync(enrollment.User.Email, enrollmentModel);
    }

    public async Task<FacultyResponseDTO> GetFacultyById(int facultyId)
    {
        _logger.InfoFormat("Trying to retrieve faculty with id {0}", facultyId);
        var facultyDto = await _academicRepository.GetFacultyByIdAsync(facultyId)
            ?? throw new NotFoundException($"Faculty with ID {facultyId} not found.");

        _logger.InfoFormat("Mapping faculty entity to DTO for ID {0}", facultyId);

        return facultyDto;
    }

    public async Task<PromotionResponseDTO> GetPromotionById(int promotionId)
    {
        _logger.InfoFormat("Trying to retrieve promotion with id {0}", promotionId);
        var promotionDto = await _academicRepository.GetPromotionByIdAsync(promotionId)
            ?? throw new NotFoundException($"Promotion with ID {promotionId} not found.");

        _logger.InfoFormat("Mapped promotion entity to DTO {0}", JsonSerializer.Serialize(promotionDto));

        return promotionDto;
    }

    public async Task<SpecialisationResponseDTO> GetSpecialisationById(int specialisationId)
    {
        _logger.InfoFormat("Trying to retrieve specialisation with id {0}", specialisationId);
        var specialisationDto = await _academicRepository.GetSpecialisationByIdAsync(specialisationId)
            ?? throw new NotFoundException($"Specialisation with ID {specialisationId} not found.");

        _logger.InfoFormat("Mapping specialisation entity to DTO for ID {0}", specialisationId);

        return specialisationDto;
    }

    public async Task<StudentGroupResponseDTO> GetStudentGroupById(int studentGroupId)
    {
        _logger.InfoFormat("Trying to retrieve student group with id {0}", studentGroupId);
        var studentGroupDto = await _academicRepository.GetGroupByIdAsync(studentGroupId)
            ?? throw new NotFoundException($"StudentGroup with ID {studentGroupId} not found.");

        _logger.InfoFormat("Mapping student group entity to DTO for ID {0}", studentGroupId);

        return studentGroupDto;
    }

    public async Task<StudentSubGroupResponseDTO> GetStudentSubGroupById(int studentSubGroupId)
    {
        _logger.InfoFormat("Trying to retrieve student sub-group with id {0}", studentSubGroupId);
        var studentSubGroupDto = await _academicRepository.GetSubGroupByIdAsync(studentSubGroupId)
           ?? throw new NotFoundException($"StudentSubGroup with ID {studentSubGroupId} not found.");

        _logger.InfoFormat("Mapping student sub-group entity to DTO for ID {0}", studentSubGroupId);

        return studentSubGroupDto;
    }

    public async Task<List<EnrollmentResponseDTO>> GetUserEnrollments(int userId)
    {
        _logger.InfoFormat("Trying to retrieve enrollment for user with ID {0}", userId);

        var _ = await _userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException($"Student with ID {userId} not found.");

        var enrollmentsDto = await _academicRepository.GetEnrollmentsByUserId(userId)
            ?? throw new NotFoundException($"Enrollment for user with ID {userId} not found.");

        _logger.InfoFormat("Mapping enrollment entity to DTO for user with ID {0}", userId);

        return enrollmentsDto;
    }

    public async Task<TeacherResponseDTO> CreateTeacher(TeacherPostDTO teacherPostDTO)
    {
        _logger.InfoFormat("Validating TeacherPostDTO: {0}", JsonSerializer.Serialize(teacherPostDTO));

        var validator = _validatorFactory.Get<TeacherPostDTO>();
        var validationResult = await validator.ValidateAsync(teacherPostDTO);

        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(ValidationHelper.ConvertErrorsToListOfStrings(validationResult.Errors));
        }

        _logger.InfoFormat("Adding new teacher to repository: {0}", JsonSerializer.Serialize(teacherPostDTO));

        var teacherDto = await _academicRepository.AddTeacherAsync(teacherPostDTO);

        return teacherDto;
	}

    public async Task<TeacherResponseDTO> GetTeacherById(int id)
    {
        _logger.InfoFormat("Trying to retrieve teacher with ID {0}", id);

        var teacherDto = await _academicRepository.GetTeacherById(id)
            ?? throw new NotFoundException($"Teacher with ID {id} not found.");

        _logger.InfoFormat("Mapping teacher entity to DTO with ID {0}", id);

        return teacherDto;

	}

    public async Task<EnrollmentResponseDTO?> GetEnrollmentById(int enrollmentId)
    {
        _logger.InfoFormat("Trying to retrieve enrollment with ID {0}", enrollmentId);
        return await _academicRepository.GetEnrollmentByIdAsync(enrollmentId)
            ?? throw new NotFoundException($"Enrollment with ID {enrollmentId} not found.");
	}

    public async Task<TeacherResponseDTO?> GetTeacherByUserId(int userId)
    {
        _logger.InfoFormat("Trying to retrieve teacher for user with ID {0}", userId);
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException($"User with ID {userId} not found.");

        if(user.Role != UserRole.Teacher)
        {
            throw new EntityValidationException($"User with ID {userId} is not a teacher.");
        }

        return await _academicRepository.GetTeacherByUserId(userId)
            ?? throw new NotFoundException($"Teacher for user with ID {userId} not found.");
    }

    public async Task<List<EnrollmentResponseDTO>> GetStudentsByStudentGroup(int studentGroupId)
    {
        _logger.InfoFormat("Trying to retrieve list of students from group with ID {0}", studentGroupId);
        var studentGroupDto = await _academicRepository.GetGroupByIdAsync(studentGroupId)
                              ?? throw new NotFoundException($"StudentGroup with ID {studentGroupId} not found.");
        return await _academicRepository.GetEnrollmentByGroup(studentGroupId);
    }

    public async Task<List<FacultyResponseDTO>> GetAllFaculties()
    {
        _logger.InfoFormat("Retrieving all faculties from repository");
        return await _academicRepository.GetAllFacultiesAsync();
    }

    public Task<List<TeacherResponseDTO>> GetAllTeachersByFacultyId(int facultyId)
    {
        _logger.InfoFormat("Retrieving all teachers for faculty with ID {0}", facultyId);
        return _academicRepository.GetAllTeachersByFacultyId(facultyId);
    }
}
