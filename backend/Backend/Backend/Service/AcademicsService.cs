using AutoMapper;
using Backend.Domain;
using Backend.Domain.DTOs;
using Backend.Exceptions.Custom;
using Backend.Interfaces;
using FluentValidation;
using IValidatorFactory = Backend.Interfaces.IValidatorFactory;

namespace Backend.Service;

public class AcademicsService(IAcademicRepository academicRepository, IMapper mapper, ILogger<AcademicsService> logger, IValidatorFactory validatorFactory
    ) 
    : IAcademicsService
{
    private readonly IAcademicRepository _academicRepository = academicRepository;

    private readonly IMapper _mapper = mapper;
    private readonly ILogger<AcademicsService> _logger = logger;
    private readonly IValidatorFactory _validatorFactory = validatorFactory;

    public async Task<FacultyResponseDTO> CreateFaculty(FacultyPostDTO facultyPostDto)
    {
        _logger.LogInformation("Validating FacultyPostDTO: {@FacultyPostDTO}", facultyPostDto);
        var validator = _validatorFactory.Get<FacultyPostDTO>();
        var validationResult = await validator.ValidateAsync(facultyPostDto);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(validationResult.Errors);
        }

        var faculty = _mapper.Map<Faculty>(facultyPostDto);

        _logger.LogInformation("Adding new faculty to repository: {@Faculty}", faculty);
        faculty = await _academicRepository.AddFacultyAsync(faculty);
        await _academicRepository.SaveChangesAsync();

        var facultyDto = _mapper.Map<FacultyResponseDTO>(faculty);
        return facultyDto;
    }

    public async Task<GroupYearResponseDTO> CreateGroupYear(GroupYearPostDTO groupYearPostDto)
    {
        _logger.LogInformation("Validating GroupYearPostDTO: {@GroupYearPostDTO}", groupYearPostDto);
        var validator = _validatorFactory.Get<GroupYearPostDTO>();
        var validationResult = await validator.ValidateAsync(groupYearPostDto);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(validationResult.Errors);
        }

        var groupYear = _mapper.Map<GroupYear>(groupYearPostDto);

        _logger.LogInformation("Adding new group year to repository: {@GroupYear}", groupYear);
        groupYear = await _academicRepository.AddGroupYearAsync(groupYear);
        await _academicRepository.SaveChangesAsync();

        var groupYearDto = _mapper.Map<GroupYearResponseDTO>(groupYear);
        return groupYearDto;
    }

    public async Task<SpecialisationResponseDTO> CreateSpecialisation(SpecialisationPostDTO specialisationPostDto)
    {
        _logger.LogInformation("Validating SpecialisationPostDTO: {@SpecialisationPostDTO}", specialisationPostDto);
        var validator = _validatorFactory.Get<SpecialisationPostDTO>();
        var validationResult = await validator.ValidateAsync(specialisationPostDto);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(validationResult.Errors);
        }

        var specialisation = _mapper.Map<Specialisation>(specialisationPostDto);

        _logger.LogInformation("Adding new specialisation to repository: {@Specialisation}", specialisation);
        specialisation = await _academicRepository.AddSpecialisationAsync(specialisation);
        await _academicRepository.SaveChangesAsync();

        var specialisationDto = _mapper.Map<SpecialisationResponseDTO>(specialisation);
        return specialisationDto;
    }

    public async Task<StudentGroupResponseDTO> CreateStudentGroup(StudentGroupPostDTO studentGroupPostDto)
    {
        _logger.LogInformation("Validating StudentGroupPostDTO: {@StudentGroupPostDTO}", studentGroupPostDto);
        var validator = _validatorFactory.Get<StudentGroupPostDTO>();
        var validationResult = await validator.ValidateAsync(studentGroupPostDto);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(validationResult.Errors);
        }

        var studentGroup = _mapper.Map<StudentGroup>(studentGroupPostDto);

        _logger.LogInformation("Adding new student group to repository: {@StudentGroup}", studentGroup);
        studentGroup = await _academicRepository.AddGroupAsync(studentGroup);
        await _academicRepository.SaveChangesAsync();

        var studentGroupDto = _mapper.Map<StudentGroupResponseDTO>(studentGroup);
        return studentGroupDto;
    }

    public async Task<StudentSubGroupResponseDTO> CreateStudentSubGroup(StudentSubGroupPostDTO studentSubGroupPostDto)
    {
        _logger.LogInformation("Validating StudentSubGroupPostDTO: {@StudentSubGroupPostDTO}", studentSubGroupPostDto);
        var validator = _validatorFactory.Get<StudentSubGroupPostDTO>();
        var validationResult = await validator.ValidateAsync(studentSubGroupPostDto);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(validationResult.Errors);
        }

        var studentSubGroup = _mapper.Map<StudentSubGroup>(studentSubGroupPostDto);

        _logger.LogInformation("Adding new student sub-group to repository: {@StudentSubGroup}", studentSubGroup);
        studentSubGroup = await _academicRepository.AddSubGroupAsync(studentSubGroup);
        await _academicRepository.SaveChangesAsync();

        var studentSubGroupDto = _mapper.Map<StudentSubGroupResponseDTO>(studentSubGroup);
        return studentSubGroupDto;
    }

    public  async Task<EnrollmentResponseDTO> CreateUserEnrollment(EnrollmentPostDTO enrollmentPostDto)
    {
        _logger.LogInformation("Validating EnrollmentPostDTO: {@EnrollmentPostDTO}", enrollmentPostDto);
        var validator = _validatorFactory.Get<EnrollmentPostDTO>();
        var validationResult = await validator.ValidateAsync(enrollmentPostDto);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(validationResult.Errors);
        }

        var enrollment = _mapper.Map<Enrollment>(enrollmentPostDto);

        _logger.LogInformation("Adding new enrollment to repository: {@Enrollment}", enrollment);
        enrollment = await _academicRepository.AddEnrollmentAsync(enrollment);
        await _academicRepository.SaveChangesAsync();

        _logger.LogInformation("Mapping enrollment entity to DTO for user with ID {UserId}", enrollmentPostDto.UserId);
        var enrollmentDto = _mapper.Map<EnrollmentResponseDTO>(enrollment);

        return enrollmentDto;
    }

    public async Task<FacultyResponseDTO> GetFacultyById(int facultyId)
    {
        _logger.LogInformation("Trying to retrieve faculty with id {@FacultyId}", facultyId);
        var faculty = await _academicRepository.GetFacultyByIdAsync(facultyId)
            ?? throw new NotFoundException($"Faculty with ID {facultyId} not found.");

        _logger.LogInformation("Mapping faculty entity to DTO for ID {FacultyId}", facultyId);
        var facultyDto = _mapper.Map<FacultyResponseDTO>(faculty);

        return facultyDto;
    }

    public async Task<GroupYearResponseDTO> GetGroupYearById(int groupYearId)
    {
        _logger.LogInformation("Trying to retrieve group year with id {@GroupYearId}", groupYearId);
        var groupYear = await _academicRepository.GetGroupYearByIdAsync(groupYearId)
            ?? throw new NotFoundException($"GroupYear with ID {groupYearId} not found.");

        _logger.LogInformation("Mapping group year entity to DTO for ID {GroupYearId}", groupYearId);
        var groupYearDto = _mapper.Map<GroupYearResponseDTO>(groupYear);

        return groupYearDto;
    }

    public async Task<SpecialisationResponseDTO> GetSpecialisationById(int specialisationId)
    {
        _logger.LogInformation("Trying to retrieve specialisation with id {@SpecialisationId}", specialisationId);
        var specialisation = await _academicRepository.GetSpecialisationByIdAsync(specialisationId)
            ?? throw new NotFoundException($"Specialisation with ID {specialisationId} not found.");

        _logger.LogInformation("Mapping specialisation entity to DTO for ID {SpecialisationId}", specialisationId);
        var specialisationDto = _mapper.Map<SpecialisationResponseDTO>(specialisation);

        return specialisationDto;
    }

    public async Task<StudentGroupResponseDTO> GetStudentGroupById(int studentGroupId)
    {
        _logger.LogInformation("Trying to retrieve student group with id {@StudentGroupId}", studentGroupId);
        var studentGroup = await _academicRepository.GetGroupByIdAsync(studentGroupId)
            ?? throw new NotFoundException($"StudentGroup with ID {studentGroupId} not found.");

        _logger.LogInformation("Mapping student group entity to DTO for ID {StudentGroupId}", studentGroupId);
        var studentGroupDto = _mapper.Map<StudentGroupResponseDTO>(studentGroup);

        return studentGroupDto;
    }

    public async Task<StudentSubGroupResponseDTO> GetStudentSubGroupById(int studentSubGroupId)
    {
        _logger.LogInformation("Trying to retrieve student sub-group with id {@StudentSubGroupId}", studentSubGroupId);
        var studentSubGroup = await _academicRepository.GetSubGroupByIdAsync(studentSubGroupId)
           ?? throw new NotFoundException($"StudentSubGroup with ID {studentSubGroupId} not found.");

        _logger.LogInformation("Mapping student sub-group entity to DTO for ID {StudentSubGroupId}", studentSubGroupId);
        var studentSubGroupDto = _mapper.Map<StudentSubGroupResponseDTO>(studentSubGroup);

        return studentSubGroupDto;
    }

    public async Task<EnrollmentResponseDTO> GetUserEnrollment(int userId)
    {
        _logger.LogInformation("Trying to retrieve enrollment for user with ID {UserId}", userId);
        var enrollment = await _academicRepository.GetEnrollmentByUserId(userId)
            ?? throw new NotFoundException($"Enrollment for user with ID {userId} not found.");

        _logger.LogInformation("Mapping enrollment entity to DTO for user with ID {UserId}", userId);
        var enrollmentDto = _mapper.Map<EnrollmentResponseDTO>(enrollment);

        return enrollmentDto;
    }
}
