using AutoMapper;
using log4net;
using Domain;
using Domain.DTOs;
using Domain.Exceptions.Custom;
using Repository.Interfaces;
using Service.Interfaces;
using IValidatorFactory = Service.Interfaces.IValidatorFactory;
using System.Text.Json;
using EmailService.Models;
using EmailService.Interfaces;

namespace Service;

public class AcademicsService(IAcademicRepository academicRepository, IUserRepository userRepository, IMapper mapper, IValidatorFactory validatorFactory, IEmailProvider emailProvider) : IAcademicsService
{
    private readonly IAcademicRepository _academicRepository = academicRepository;
    private readonly IUserRepository _userRepository = userRepository;

    private readonly IMapper _mapper = mapper;
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
            throw new EntityValidationException(validationResult.Errors);
        }

        var faculty = _mapper.Map<Faculty>(facultyPostDto);

        _logger.InfoFormat("Adding new faculty to repository: {0}", JsonSerializer.Serialize(faculty));
        faculty = await _academicRepository.AddFacultyAsync(faculty);
        await _academicRepository.SaveChangesAsync();

        var facultyDto = _mapper.Map<FacultyResponseDTO>(faculty);
        return facultyDto;
    }

    public async Task<GroupYearResponseDTO> CreateGroupYear(GroupYearPostDTO groupYearPostDto)
    {
        _logger.InfoFormat("Validating GroupYearPostDTO: {0}", JsonSerializer.Serialize(groupYearPostDto));
        var validator = _validatorFactory.Get<GroupYearPostDTO>();
        var validationResult = await validator.ValidateAsync(groupYearPostDto);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(validationResult.Errors);
        }

        var groupYear = _mapper.Map<GroupYear>(groupYearPostDto);

        _logger.InfoFormat("Adding new group year to repository: {0}", JsonSerializer.Serialize(groupYear));
        groupYear = await _academicRepository.AddGroupYearAsync(groupYear);
        await _academicRepository.SaveChangesAsync();

        var groupYearDto = _mapper.Map<GroupYearResponseDTO>(groupYear);
        return groupYearDto;
    }

    public async Task<SpecialisationResponseDTO> CreateSpecialisation(SpecialisationPostDTO specialisationPostDto)
    {
        _logger.InfoFormat("Validating SpecialisationPostDTO: {0}", JsonSerializer.Serialize(specialisationPostDto));
        var validator = _validatorFactory.Get<SpecialisationPostDTO>();
        var validationResult = await validator.ValidateAsync(specialisationPostDto);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(validationResult.Errors);
        }

        var specialisation = _mapper.Map<Specialisation>(specialisationPostDto);

        _logger.InfoFormat("Adding new specialisation to repository: {0}", JsonSerializer.Serialize(specialisation));
        specialisation = await _academicRepository.AddSpecialisationAsync(specialisation);
        await _academicRepository.SaveChangesAsync();

        var specialisationDto = _mapper.Map<SpecialisationResponseDTO>(specialisation);
        return specialisationDto;
    }

    public async Task<StudentGroupResponseDTO> CreateStudentGroup(StudentGroupPostDTO studentGroupPostDto)
    {
        _logger.InfoFormat("Validating StudentGroupPostDTO: {0}", JsonSerializer.Serialize(studentGroupPostDto));
        var validator = _validatorFactory.Get<StudentGroupPostDTO>();
        var validationResult = await validator.ValidateAsync(studentGroupPostDto);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(validationResult.Errors);
        }

        var studentGroup = _mapper.Map<StudentGroup>(studentGroupPostDto);

        _logger.InfoFormat("Adding new student group to repository: {0}", JsonSerializer.Serialize(studentGroup));
        studentGroup = await _academicRepository.AddGroupAsync(studentGroup);
        await _academicRepository.SaveChangesAsync();

        var studentGroupDto = _mapper.Map<StudentGroupResponseDTO>(studentGroup);
        return studentGroupDto;
    }

    public async Task<StudentSubGroupResponseDTO> CreateStudentSubGroup(StudentSubGroupPostDTO studentSubGroupPostDto)
    {
        _logger.InfoFormat("Validating StudentSubGroupPostDTO: {0}", JsonSerializer.Serialize(studentSubGroupPostDto));
        var validator = _validatorFactory.Get<StudentSubGroupPostDTO>();
        var validationResult = await validator.ValidateAsync(studentSubGroupPostDto);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(validationResult.Errors);
        }

        var studentSubGroup = _mapper.Map<StudentSubGroup>(studentSubGroupPostDto);

        _logger.InfoFormat("Adding new student sub-group to repository: {0}", JsonSerializer.Serialize(studentSubGroup));
        studentSubGroup = await _academicRepository.AddSubGroupAsync(studentSubGroup);
        await _academicRepository.SaveChangesAsync();

        var studentSubGroupDto = _mapper.Map<StudentSubGroupResponseDTO>(studentSubGroup);
        return studentSubGroupDto;
    }

    public async Task<EnrollmentResponseDTO> CreateUserEnrollment(EnrollmentPostDTO enrollmentPostDto)
    {
        _logger.InfoFormat("Validating EnrollmentPostDTO: {0}", JsonSerializer.Serialize(enrollmentPostDto));
        var validator = _validatorFactory.Get<EnrollmentPostDTO>();
        var validationResult = await validator.ValidateAsync(enrollmentPostDto);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(validationResult.Errors);
        }

        var enrollment = _mapper.Map<Enrollment>(enrollmentPostDto);

        _logger.InfoFormat("Adding new enrollment to repository: {0}", JsonSerializer.Serialize(enrollment));
        enrollment = await _academicRepository.AddEnrollmentAsync(enrollment);
        await _academicRepository.SaveChangesAsync();
        
        _logger.InfoFormat($"Sending email to: {enrollment.User.Email}");
        await SendAddedEnrollementEmail(enrollment);

        _logger.InfoFormat("Mapping enrollment entity to DTO for user with ID {0}", enrollmentPostDto.UserId);
        var enrollmentDto = _mapper.Map<EnrollmentResponseDTO>(enrollment);

        return enrollmentDto;
    }

    private async Task SendAddedEnrollementEmail(Enrollment enrollment)
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
        var faculty = await _academicRepository.GetFacultyByIdAsync(facultyId)
            ?? throw new NotFoundException($"Faculty with ID {facultyId} not found.");

        _logger.InfoFormat("Mapping faculty entity to DTO for ID {0}", facultyId);
        var facultyDto = _mapper.Map<FacultyResponseDTO>(faculty);

        return facultyDto;
    }

    public async Task<GroupYearResponseDTO> GetGroupYearById(int groupYearId)
    {
        _logger.InfoFormat("Trying to retrieve group year with id {0}", groupYearId);
        var groupYear = await _academicRepository.GetGroupYearByIdAsync(groupYearId)
            ?? throw new NotFoundException($"GroupYear with ID {groupYearId} not found.");

        _logger.InfoFormat("Mapping group year entity to DTO for ID {0}", groupYearId);
        var groupYearDto = _mapper.Map<GroupYearResponseDTO>(groupYear);

        return groupYearDto;
    }

    public async Task<SpecialisationResponseDTO> GetSpecialisationById(int specialisationId)
    {
        _logger.InfoFormat("Trying to retrieve specialisation with id {0}", specialisationId);
        var specialisation = await _academicRepository.GetSpecialisationByIdAsync(specialisationId)
            ?? throw new NotFoundException($"Specialisation with ID {specialisationId} not found.");

        _logger.InfoFormat("Mapping specialisation entity to DTO for ID {0}", specialisationId);
        var specialisationDto = _mapper.Map<SpecialisationResponseDTO>(specialisation);

        return specialisationDto;
    }

    public async Task<StudentGroupResponseDTO> GetStudentGroupById(int studentGroupId)
    {
        _logger.InfoFormat("Trying to retrieve student group with id {0}", studentGroupId);
        var studentGroup = await _academicRepository.GetGroupByIdAsync(studentGroupId)
            ?? throw new NotFoundException($"StudentGroup with ID {studentGroupId} not found.");

        _logger.InfoFormat("Mapping student group entity to DTO for ID {0}", studentGroupId);
        var studentGroupDto = _mapper.Map<StudentGroupResponseDTO>(studentGroup);

        return studentGroupDto;
    }

    public async Task<StudentSubGroupResponseDTO> GetStudentSubGroupById(int studentSubGroupId)
    {
        _logger.InfoFormat("Trying to retrieve student sub-group with id {0}", studentSubGroupId);
        var studentSubGroup = await _academicRepository.GetSubGroupByIdAsync(studentSubGroupId)
           ?? throw new NotFoundException($"StudentSubGroup with ID {studentSubGroupId} not found.");

        _logger.InfoFormat("Mapping student sub-group entity to DTO for ID {0}", studentSubGroupId);
        var studentSubGroupDto = _mapper.Map<StudentSubGroupResponseDTO>(studentSubGroup);

        return studentSubGroupDto;
    }

    public async Task<List<EnrollmentResponseDTO>> GetUserEnrollments(int userId)
    {
        _logger.InfoFormat("Trying to retrieve enrollment for user with ID {0}", userId);

        var _ = await _userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException($"Student with ID {userId} not found.");

        var enrollments = await _academicRepository.GetEnrollmentsByUserId(userId)
            ?? throw new NotFoundException($"Enrollment for user with ID {userId} not found.");

        _logger.InfoFormat("Mapping enrollment entity to DTO for user with ID {0}", userId);
        var enrollmentsDto = _mapper.Map<List<EnrollmentResponseDTO>>(enrollments);

        return enrollmentsDto;
    }

    public async Task<TeacherResponseDTO> CreateTeacher(TeacherPostDTO teacherPostDTO)
    {
        _logger.InfoFormat("Validating TeacherPostDTO: {0}", JsonSerializer.Serialize(teacherPostDTO));

        var validator = _validatorFactory.Get<TeacherPostDTO>();
        var validationResult = await validator.ValidateAsync(teacherPostDTO);

        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(validationResult.Errors);
        }

        var teacher = _mapper.Map<Teacher>(teacherPostDTO);

        _logger.InfoFormat("Adding new teacher to repository: {0}", JsonSerializer.Serialize(teacher));

        teacher = await _academicRepository.AddTeacherAsync(teacher);
        await _academicRepository.SaveChangesAsync();

        return _mapper.Map<TeacherResponseDTO>(teacher);
    }

    public async Task<TeacherResponseDTO> GetTeacherById(int id)
    {
        _logger.InfoFormat("Trying to retrieve teacher with ID {0}", id);

        var teacher = await _academicRepository.GetTeacherById(id)
            ?? throw new NotFoundException($"Teacher with ID {id} not found.");

        _logger.InfoFormat("Mapping teacher entity to DTO with ID {0}", id);

        return _mapper.Map<TeacherResponseDTO>(teacher);
    }
}
