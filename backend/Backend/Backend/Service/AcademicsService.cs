using AutoMapper;
using Backend.Domain;
using Backend.Domain.DTOs;
using Backend.Exceptions.Custom;
using Backend.Interfaces;
using FluentValidation;
using IValidatorFactory = Backend.Interfaces.IValidatorFactory;

namespace Backend.Service;

public class AcademicsService(IEnrollmentRepository enrollmentRepository, IFacultyRepository facultyRepository,
    ISpecialisationRepository specialisationRepository, IGroupYearRepository groupYearRepository, IStudentGroupRepository studentGroupRepository,
    IStudentSubGroupRepository studentSubGroupRepository, IMapper mapper, ILogger<AcademicsService> logger, IValidatorFactory validatorFactory
    ) 
    : IAcademicsService
{
    private readonly IEnrollmentRepository _enrollmentRepository = enrollmentRepository;
    private readonly IFacultyRepository _facultyRepository = facultyRepository;
    private readonly ISpecialisationRepository _specialisationRepository = specialisationRepository;
    private readonly IGroupYearRepository _groupYearRepository = groupYearRepository;
    private readonly IStudentGroupRepository _studentGroupRepository = studentGroupRepository;
    private readonly IStudentSubGroupRepository _studentSubGroupRepository = studentSubGroupRepository;

    private readonly IMapper _mapper = mapper;
    private readonly ILogger<AcademicsService> _logger = logger;
    private readonly IValidatorFactory _validatorFactory = validatorFactory;

    public async Task<FacultyResponseDTO> CreateFaculty(FacultyPostDTO facultyPostDto)
    {
        _logger.LogInformation("Validating FacultyPostDTO: {@FacultyPostDTO}", facultyPostDto);
        IValidator<FacultyPostDTO> validator = _validatorFactory.Get<FacultyPostDTO>();
        var validationResult = await validator.ValidateAsync(facultyPostDto);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(validationResult.Errors);
        }

        Faculty faculty = _mapper.Map<Faculty>(facultyPostDto);

        _logger.LogInformation("Adding new faculty to repository: {@Faculty}", faculty);
        faculty = await _facultyRepository.AddAsync(faculty);
        await _facultyRepository.SaveChangesAsync();

        FacultyResponseDTO facultyDto = _mapper.Map<FacultyResponseDTO>(faculty);
        return facultyDto;
    }

    public async Task<GroupYearResponseDTO> CreateGroupYear(GroupYearPostDTO groupYearPostDto)
    {
        _logger.LogInformation("Validating GroupYearPostDTO: {@GroupYearPostDTO}", groupYearPostDto);
        IValidator<GroupYearPostDTO> validator = _validatorFactory.Get<GroupYearPostDTO>();
        var validationResult = await validator.ValidateAsync(groupYearPostDto);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(validationResult.Errors);
        }

        GroupYear groupYear = _mapper.Map<GroupYear>(groupYearPostDto);

        _logger.LogInformation("Adding new group year to repository: {@GroupYear}", groupYear);
        groupYear = await _groupYearRepository.AddAsync(groupYear);
        await _groupYearRepository.SaveChangesAsync();

        GroupYearResponseDTO groupYearDto = _mapper.Map<GroupYearResponseDTO>(groupYear);
        return groupYearDto;
    }

    public async Task<SpecialisationResponseDTO> CreateSpecialisation(SpecialisationPostDTO specialisationPostDto)
    {
        _logger.LogInformation("Validating SpecialisationPostDTO: {@SpecialisationPostDTO}", specialisationPostDto);
        IValidator<SpecialisationPostDTO> validator = _validatorFactory.Get<SpecialisationPostDTO>();
        var validationResult = await validator.ValidateAsync(specialisationPostDto);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(validationResult.Errors);
        }

        Specialisation specialisation = _mapper.Map<Specialisation>(specialisationPostDto);

        _logger.LogInformation("Adding new specialisation to repository: {@Specialisation}", specialisation);
        specialisation = await _specialisationRepository.AddAsync(specialisation);
        await _specialisationRepository.SaveChangesAsync();

        SpecialisationResponseDTO specialisationDto = _mapper.Map<SpecialisationResponseDTO>(specialisation);
        return specialisationDto;
    }

    public async Task<StudentGroupResponseDTO> CreateStudentGroup(StudentGroupPostDTO studentGroupPostDto)
    {
        _logger.LogInformation("Validating StudentGroupPostDTO: {@StudentGroupPostDTO}", studentGroupPostDto);
        IValidator<StudentGroupPostDTO> validator = _validatorFactory.Get<StudentGroupPostDTO>();
        var validationResult = await validator.ValidateAsync(studentGroupPostDto);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(validationResult.Errors);
        }

        StudentGroup studentGroup = _mapper.Map<StudentGroup>(studentGroupPostDto);

        _logger.LogInformation("Adding new student group to repository: {@StudentGroup}", studentGroup);
        studentGroup = await _studentGroupRepository.AddAsync(studentGroup);
        await _studentGroupRepository.SaveChangesAsync();

        StudentGroupResponseDTO studentGroupDto = _mapper.Map<StudentGroupResponseDTO>(studentGroup);
        return studentGroupDto;
    }

    public async Task<StudentSubGroupResponseDTO> CreateStudentSubGroup(StudentSubGroupPostDTO studentSubGroupPostDto)
    {
        _logger.LogInformation("Validating StudentSubGroupPostDTO: {@StudentSubGroupPostDTO}", studentSubGroupPostDto);
        IValidator<StudentSubGroupPostDTO> validator = _validatorFactory.Get<StudentSubGroupPostDTO>();
        var validationResult = await validator.ValidateAsync(studentSubGroupPostDto);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(validationResult.Errors);
        }

        StudentSubGroup studentSubGroup = _mapper.Map<StudentSubGroup>(studentSubGroupPostDto);

        _logger.LogInformation("Adding new student sub-group to repository: {@StudentSubGroup}", studentSubGroup);
        studentSubGroup = await _studentSubGroupRepository.AddAsync(studentSubGroup);
        await _studentSubGroupRepository.SaveChangesAsync();

        StudentSubGroupResponseDTO studentSubGroupDto = _mapper.Map<StudentSubGroupResponseDTO>(studentSubGroup);
        return studentSubGroupDto;
    }

    public  async Task<EnrollmentResponseDTO> CreateUserEnrollment(EnrollmentPostDTO enrollmentPostDto)
    {
        _logger.LogInformation("Validating EnrollmentPostDTO: {@EnrollmentPostDTO}", enrollmentPostDto);
        IValidator<EnrollmentPostDTO> validator = _validatorFactory.Get<EnrollmentPostDTO>();
        var validationResult = await validator.ValidateAsync(enrollmentPostDto);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(validationResult.Errors);
        }

        Enrollment enrollment = _mapper.Map<Enrollment>(enrollmentPostDto);

        _logger.LogInformation("Adding new enrollment to repository: {@Enrollment}", enrollment);
        enrollment = await _enrollmentRepository.AddAsync(enrollment);
        await _enrollmentRepository.SaveChangesAsync();

        _logger.LogInformation("Mapping enrollment entity to DTO for user with ID {UserId}", enrollmentPostDto.UserId);
        EnrollmentResponseDTO enrollmentDto = _mapper.Map<EnrollmentResponseDTO>(enrollment);

        return enrollmentDto;
    }

    public async Task<FacultyResponseDTO> GetFacultyById(int facultyId)
    {
        _logger.LogInformation("Trying to retrieve faculty with id {@FacultyId}", facultyId);
        Faculty faculty = await _facultyRepository.GetByIdAsync(facultyId)
            ?? throw new NotFoundException($"Faculty with ID {facultyId} not found.");

        _logger.LogInformation("Mapping faculty entity to DTO for ID {FacultyId}", facultyId);
        FacultyResponseDTO facultyDto = _mapper.Map<FacultyResponseDTO>(faculty);

        return facultyDto;
    }

    public async Task<GroupYearResponseDTO> GetGroupYearById(int groupYearId)
    {
        _logger.LogInformation("Trying to retrieve group year with id {@GroupYearId}", groupYearId);
        GroupYear groupYear = await _groupYearRepository.GetByIdAsync(groupYearId)
            ?? throw new NotFoundException($"GroupYear with ID {groupYearId} not found.");

        _logger.LogInformation("Mapping group year entity to DTO for ID {GroupYearId}", groupYearId);
        GroupYearResponseDTO groupYearDto = _mapper.Map<GroupYearResponseDTO>(groupYear);

        return groupYearDto;
    }

    public async Task<SpecialisationResponseDTO> GetSpecialisationById(int specialisationId)
    {
        _logger.LogInformation("Trying to retrieve specialisation with id {@SpecialisationId}", specialisationId);
        Specialisation specialisation = await _specialisationRepository.GetByIdAsync(specialisationId)
            ?? throw new NotFoundException($"Specialisation with ID {specialisationId} not found.");

        _logger.LogInformation("Mapping specialisation entity to DTO for ID {SpecialisationId}", specialisationId);
        SpecialisationResponseDTO specialisationDto = _mapper.Map<SpecialisationResponseDTO>(specialisation);

        return specialisationDto;
    }

    public async Task<StudentGroupResponseDTO> GetStudentGroupById(int studentGroupId)
    {
        _logger.LogInformation("Trying to retrieve student group with id {@StudentGroupId}", studentGroupId);
        StudentGroup studentGroup = await _studentGroupRepository.GetByIdAsync(studentGroupId)
            ?? throw new NotFoundException($"StudentGroup with ID {studentGroupId} not found.");

        _logger.LogInformation("Mapping student group entity to DTO for ID {StudentGroupId}", studentGroupId);
        StudentGroupResponseDTO studentGroupDto = _mapper.Map<StudentGroupResponseDTO>(studentGroup);

        return studentGroupDto;
    }

    public async Task<StudentSubGroupResponseDTO> GetStudentSubGroupById(int studentSubGroupId)
    {
        _logger.LogInformation("Trying to retrieve student sub-group with id {@StudentSubGroupId}", studentSubGroupId);
        StudentSubGroup studentSubGroup = await _studentSubGroupRepository.GetByIdAsync(studentSubGroupId)
           ?? throw new NotFoundException($"StudentSubGroup with ID {studentSubGroupId} not found.");

        _logger.LogInformation("Mapping student sub-group entity to DTO for ID {StudentSubGroupId}", studentSubGroupId);
        StudentSubGroupResponseDTO studentSubGroupDto = _mapper.Map<StudentSubGroupResponseDTO>(studentSubGroup);

        return studentSubGroupDto;
    }

    public async Task<EnrollmentResponseDTO> GetUserEnrollment(int userId)
    {
        _logger.LogInformation("Trying to retrieve enrollment for user with ID {UserId}", userId);
        Enrollment enrollment = await _enrollmentRepository.GetEnrollmentByUserId(userId)
            ?? throw new NotFoundException($"Enrollment for user with ID {userId} not found.");

        _logger.LogInformation("Mapping enrollment entity to DTO for user with ID {UserId}", userId);
        EnrollmentResponseDTO enrollmentDto = _mapper.Map<EnrollmentResponseDTO>(enrollment);

        return enrollmentDto;
    }

    
}
