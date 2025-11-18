using AutoMapper;
using Domain;
using Domain.DTOs;
using Domain.Enums;
using Domain.Exceptions.Custom;
using Repository.Interfaces;
using Service;
using Service.Validators;
using EmailService.Interfaces;
using Moq;
using Xunit;
using IValidatorFactory = Service.Interfaces.IValidatorFactory;

namespace BackendTests;

public class AcademicsServiceTests
{
    private readonly Mock<IAcademicRepository> _mockRepository = new();
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<IMapper> _mockMapper = new();
    private readonly Mock<IEmailProvider> _mockEmailProvider = new();
    private readonly IValidatorFactory _validatorFactory;
    private readonly AcademicsService _service;

    public AcademicsServiceTests()
    {
        var facultyValidator = new FacultyPostDTOValidator(_mockRepository.Object);
        var groupYearValidator = new GroupYearPostDTOValidator(_mockRepository.Object);
        var specialisationValidator = new SpecialisationPostDTOValidator(_mockRepository.Object);
        var studentGroupValidator = new StudentGroupPostDTOValidator(_mockRepository.Object);
        var studentSubGroupValidator = new StudentSubGroupPostDTOValidator(_mockRepository.Object);
        var enrollmentValidator = new EnrollmentPostDTOValidator(_mockUserRepository.Object, _mockRepository.Object);
        var teacherValidator = new TeacherPostDTOValidator(_mockRepository.Object, _mockUserRepository.Object);
        var mockValidatorFactory = new Mock<IValidatorFactory>();

        mockValidatorFactory.Setup(v => v.Get<FacultyPostDTO>()).Returns(facultyValidator);
        mockValidatorFactory.Setup(v => v.Get<GroupYearPostDTO>()).Returns(groupYearValidator);
        mockValidatorFactory.Setup(v => v.Get<SpecialisationPostDTO>()).Returns(specialisationValidator);
        mockValidatorFactory.Setup(v => v.Get<StudentGroupPostDTO>()).Returns(studentGroupValidator);
        mockValidatorFactory.Setup(v => v.Get<StudentSubGroupPostDTO>()).Returns(studentSubGroupValidator);
        mockValidatorFactory.Setup(v => v.Get<EnrollmentPostDTO>()).Returns(enrollmentValidator);
        mockValidatorFactory.Setup(v => v.Get<TeacherPostDTO>()).Returns(teacherValidator);
        _validatorFactory = mockValidatorFactory.Object;

        _service = new AcademicsService(
            _mockRepository.Object,
            _mockUserRepository.Object,
            _mockMapper.Object,
            _validatorFactory,
            _mockEmailProvider.Object
        );
    }

    [Theory]
    [InlineData("Facultatea de Drept")]
    [InlineData("Facultatea de Geografie")]
    public async Task CreateFacultyValidData(string name)
    {
        var postDto = new FacultyPostDTO { Name = name };
        var entity = new Faculty { Id = 1, Name = name };
        var responseDto = new FacultyResponseDTO { Id = 1, Name = name };

        _mockMapper.Setup(m => m.Map<Faculty>(postDto)).Returns(entity);
        _mockRepository.Setup(r => r.AddFacultyAsync(entity)).ReturnsAsync(entity);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<FacultyResponseDTO>(entity)).Returns(responseDto);

        var result = await _service.CreateFaculty(postDto);

        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
        _mockRepository.Verify(r => r.AddFacultyAsync(entity), Times.Once);
    }

    [Theory]
    [InlineData("")]
    public async Task CreateFacultyInvalidData(string name)
    {
        var postDto = new FacultyPostDTO { Name = name };
        var entity = new Faculty { Id = 1, Name = name };

        await Assert.ThrowsAsync<EntityValidationException>(() => _service.CreateFaculty(postDto));
        _mockRepository.Verify(r => r.AddFacultyAsync(entity), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory]
    [InlineData("IR1", 1)]
    [InlineData("IE3", 1)]
    public async Task CreateGroupYearValid(string year, int specialisationId)
    {
        var dto = new GroupYearPostDTO { Year = year, SpecialisationId = specialisationId };
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        var specialisation = new Specialisation { Id = 1, Name = "Computer Science", Faculty = faculty };
        _mockRepository
            .Setup(r => r.GetSpecialisationByIdAsync(specialisationId))
            .ReturnsAsync(specialisation);
        var entity = new GroupYear { Id = 1, Year = year, Specialisation = specialisation };
        var responseDto = new GroupYearResponseDTO { Id = 1, Year = year };

        _mockMapper.Setup(m => m.Map<GroupYear>(dto)).Returns(entity);
        _mockRepository.Setup(r => r.AddGroupYearAsync(entity)).ReturnsAsync(entity);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<GroupYearResponseDTO>(entity)).Returns(responseDto);

        var result = await _service.CreateGroupYear(dto);

        Assert.NotNull(result);
        Assert.Equal(year, result.Year);
        _mockRepository.Verify(r => r.AddGroupYearAsync(entity), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }


    [Theory]
    [InlineData("", 1)]
    [InlineData("IE3", -1)]
    public async Task CreateGroupYearInvalidData(string year, int specialisationId)
    {
        var dto = new GroupYearPostDTO { Year = year, SpecialisationId = specialisationId };
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        var specialisation = new Specialisation { Id = 1, Name = "Computer Science", Faculty = faculty };
        _mockRepository
            .Setup(r => r.GetSpecialisationByIdAsync(specialisationId))
            .ReturnsAsync(specialisation);
        var entity = new GroupYear { Id = 1, Year = year, Specialisation = specialisation };
        var responseDto = new GroupYearResponseDTO { Id = 1, Year = year };

        _mockMapper.Setup(m => m.Map<GroupYear>(dto)).Returns(entity);
        _mockRepository.Setup(r => r.AddGroupYearAsync(entity)).ReturnsAsync(entity);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<GroupYearResponseDTO>(entity)).Returns(responseDto);


        await Assert.ThrowsAsync<EntityValidationException>(() => _service.CreateGroupYear(dto));

        _mockRepository.Verify(r => r.AddGroupYearAsync(entity), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }


    [Theory]
    [InlineData("Computer-Science", 1)]
    [InlineData("AI", 1)]
    public async Task CreateSpecialisationValidData(string name, int facultyId)
    {
        var dto = new SpecialisationPostDTO { Name = name, FacultyId = facultyId };
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        _mockRepository
            .Setup(r => r.GetFacultyByIdAsync(facultyId))
            .ReturnsAsync(faculty);
        var entity = new Specialisation { Id = 1, Name = name, Faculty = faculty };
        var responseDto = new SpecialisationResponseDTO { Id = 1, Name = name };

        _mockMapper.Setup(m => m.Map<Specialisation>(dto)).Returns(entity);
        _mockRepository.Setup(r => r.AddSpecialisationAsync(entity)).ReturnsAsync(entity);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<SpecialisationResponseDTO>(entity)).Returns(responseDto);

        var result = await _service.CreateSpecialisation(dto);

        Assert.NotNull(result);
        Assert.Equal(name, result.Name);

        _mockRepository.Verify(r => r.AddSpecialisationAsync(entity), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Theory]
    [InlineData("", 1)]
    [InlineData("AI", -1)]
    public async Task CreateSpecialisationInvalidData(string name, int facultyId)
    {
        var dto = new SpecialisationPostDTO { Name = name, FacultyId = facultyId };
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        _mockRepository
            .Setup(r => r.GetFacultyByIdAsync(facultyId))
            .ReturnsAsync(faculty);
        var entity = new Specialisation { Id = 1, Name = name, Faculty = faculty };
        var responseDto = new SpecialisationResponseDTO { Id = 1, Name = name };

        _mockMapper.Setup(m => m.Map<Specialisation>(dto)).Returns(entity);
        _mockRepository.Setup(r => r.AddSpecialisationAsync(entity)).ReturnsAsync(entity);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<SpecialisationResponseDTO>(entity)).Returns(responseDto);

        await Assert.ThrowsAsync<EntityValidationException>(() => _service.CreateSpecialisation(dto));

        _mockRepository.Verify(r => r.AddSpecialisationAsync(entity), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory]
    [InlineData("IR1", 1)]
    [InlineData("IR2", 1)]
    public async Task CreateStudentGroupValidData(string name, int groupYearId)
    {
        var dto = new StudentGroupPostDTO { Name = name, GroupYearId = groupYearId };
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        var specialisation = new Specialisation { Id = 1, Name = "Computer Science", Faculty = faculty };
        var groupYear = new GroupYear { Id = groupYearId, Year = "IR1", Specialisation = specialisation };
        _mockRepository
            .Setup(r => r.GetGroupYearByIdAsync(groupYearId))
            .ReturnsAsync(groupYear);
        var entity = new StudentGroup { Id = 1, Name = name, GroupYear = groupYear };
        var responseDto = new StudentGroupResponseDTO { Id = 1, Name = name };

        _mockMapper.Setup(m => m.Map<StudentGroup>(dto)).Returns(entity);
        _mockRepository.Setup(r => r.AddGroupAsync(entity)).ReturnsAsync(entity);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<StudentGroupResponseDTO>(entity)).Returns(responseDto);

        var result = await _service.CreateStudentGroup(dto);

        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
        _mockRepository.Verify(r => r.AddGroupAsync(entity), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Theory]
    [InlineData("", 1)]
    [InlineData("IR2", -1)]
    public async Task CreateStudentGroupInvalidData(string name, int groupYearId)
    {
        var dto = new StudentGroupPostDTO { Name = name, GroupYearId = groupYearId };
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        var specialisation = new Specialisation { Id = 1, Name = "Computer Science", Faculty = faculty };
        var groupYear = new GroupYear { Id = groupYearId, Year = "IR1", Specialisation = specialisation };
        _mockRepository
            .Setup(r => r.GetGroupYearByIdAsync(groupYearId))
            .ReturnsAsync(groupYear);
        var entity = new StudentGroup { Id = 1, Name = name, GroupYear = groupYear };
        var responseDto = new StudentGroupResponseDTO { Id = 1, Name = name };

        _mockMapper.Setup(m => m.Map<StudentGroup>(dto)).Returns(entity);
        _mockRepository.Setup(r => r.AddGroupAsync(entity)).ReturnsAsync(entity);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<StudentGroupResponseDTO>(entity)).Returns(responseDto);

        await Assert.ThrowsAsync<EntityValidationException>(() => _service.CreateStudentGroup(dto));

        _mockRepository.Verify(r => r.AddGroupAsync(entity), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory]
    [InlineData("236/1", 1)]
    public async Task CreateStudentSubGroupValidData(string name, int studentGroupId)
    {
        var dto = new StudentSubGroupPostDTO { Name = name, StudentGroupId = studentGroupId };
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        var specialisation = new Specialisation { Id = 1, Name = "Computer Science", Faculty = faculty };
        var groupYear = new GroupYear { Id = 1, Year = "IR1", Specialisation = specialisation };
        var studentGroup = new StudentGroup { Id = studentGroupId, Name = name, GroupYear = groupYear };
        _mockRepository
            .Setup(r => r.GetGroupByIdAsync(studentGroupId))
            .ReturnsAsync(studentGroup);
        var entity = new StudentSubGroup { Id = 1, Name = name, StudentGroup = studentGroup };
        var responseDto = new StudentSubGroupResponseDTO { Id = 1, Name = name };

        _mockMapper.Setup(m => m.Map<StudentSubGroup>(dto)).Returns(entity);
        _mockRepository.Setup(r => r.AddSubGroupAsync(entity)).ReturnsAsync(entity);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<StudentSubGroupResponseDTO>(entity)).Returns(responseDto);

        var result = await _service.CreateStudentSubGroup(dto);

        Assert.NotNull(result);
        Assert.Equal(name, result.Name);

        _mockRepository.Verify(r => r.AddSubGroupAsync(entity), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Theory]
    [InlineData("", 1)]
    [InlineData("", -1)]
    public async Task CreateStudentSubGroupInvalidData(string name, int studentGroupId)
    {
        var dto = new StudentSubGroupPostDTO { Name = name, StudentGroupId = studentGroupId };
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        var specialisation = new Specialisation { Id = 1, Name = "Computer Science", Faculty = faculty };
        var groupYear = new GroupYear { Id = 1, Year = "IR1", Specialisation = specialisation };
        var studentGroup = new StudentGroup { Id = studentGroupId, Name = name, GroupYear = groupYear };
        _mockRepository
            .Setup(r => r.GetGroupByIdAsync(studentGroupId))
            .ReturnsAsync(studentGroup);
        var entity = new StudentSubGroup { Id = 1, Name = name, StudentGroup = studentGroup };
        var responseDto = new StudentSubGroupResponseDTO { Id = 1, Name = name };

        _mockMapper.Setup(m => m.Map<StudentSubGroup>(dto)).Returns(entity);
        _mockRepository.Setup(r => r.AddSubGroupAsync(entity)).ReturnsAsync(entity);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<StudentSubGroupResponseDTO>(entity)).Returns(responseDto);

        await Assert.ThrowsAsync<EntityValidationException>(() => _service.CreateStudentSubGroup(dto));

        _mockRepository.Verify(r => r.AddSubGroupAsync(entity), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory]
    [InlineData(1, 1)]
    public async Task CreateUserEnrollmentValidData(int userId, int subGroupId)
    {
        var dto = new EnrollmentPostDTO { UserId = userId, SubGroupId = subGroupId };
        var user = new User
        {
            Id = 1, FirstName = "Andrei", LastName = "Rotaru", Email = "andrei@gmail.com", Password = "111222ppa",
            PhoneNumber = "+40777301089", Role = Enum.Parse<UserRole>("Student")
        };
        _mockUserRepository
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        var specialisation = new Specialisation { Id = 1, Name = "Computer Science", Faculty = faculty };
        var groupYear = new GroupYear { Id = 1, Year = "IR1", Specialisation = specialisation };
        var studentGroup = new StudentGroup { Id = 1, Name = "234", GroupYear = groupYear };
        var subGroup = new StudentSubGroup { Id = 1, Name = "234/1", StudentGroup = studentGroup };
        _mockRepository
            .Setup(r => r.GetSubGroupByIdAsync(subGroupId))
            .ReturnsAsync(subGroup);
        var entity = new Enrollment { Id = 1, User = user, SubGroup = subGroup };
        var userResponseDto = new UserResponseDTO
        {
            Id = userId, FirstName = "Andrei", LastName = "Rotaru", Email = "andrei@gmail.com", Password = "111222ppa",
            PhoneNumber = "+40777301089", Role = "Student"
        };
        var subGroupDto = new StudentSubGroupResponseDTO { Id = subGroupId, Name = subGroup.Name };
        var responseDto = new EnrollmentResponseDTO
            { Id = 1, UserId = userId, SubGroupId = subGroupId, User = userResponseDto, SubGroup = subGroupDto };

        _mockMapper.Setup(m => m.Map<Enrollment>(dto)).Returns(entity);
        _mockRepository.Setup(r => r.AddEnrollmentAsync(entity)).ReturnsAsync(entity);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<EnrollmentResponseDTO>(entity)).Returns(responseDto);

        var result = await _service.CreateUserEnrollment(dto);

        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);

        _mockRepository.Verify(r => r.AddEnrollmentAsync(entity), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Theory]
    [InlineData(-1, 1)]
    [InlineData(1, -1)]
    public async Task CreateUserEnrollmentInvalidData(int userId, int subGroupId)
    {
        var dto = new EnrollmentPostDTO { UserId = userId, SubGroupId = subGroupId };
        var user = new User
        {
            Id = userId, FirstName = "Andrei", LastName = "Rotaru", Email = "andrei@gmail.com", Password = "111222ppa",
            PhoneNumber = "+40777301089", Role = Enum.Parse<UserRole>("Student")
        };
        _mockUserRepository
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        var specialisation = new Specialisation { Id = 1, Name = "Computer Science", Faculty = faculty };
        var groupYear = new GroupYear { Id = 1, Year = "IR1", Specialisation = specialisation };
        var studentGroup = new StudentGroup { Id = 1, Name = "234", GroupYear = groupYear };
        var subGroup = new StudentSubGroup { Id = 1, Name = "234/1", StudentGroup = studentGroup };
        _mockRepository
            .Setup(r => r.GetSubGroupByIdAsync(subGroupId))
            .ReturnsAsync(subGroup);
        var entity = new Enrollment { Id = 1, User = user, SubGroup = subGroup };
        var userResponseDto = new UserResponseDTO
        {
            Id = userId, FirstName = "Andrei", LastName = "Rotaru", Email = "andrei@gmail.com", Password = "111222ppa",
            PhoneNumber = "+40777301089", Role = "Student"
        };
        var subGroupDto = new StudentSubGroupResponseDTO { Id = subGroupId, Name = subGroup.Name };
        var responseDto = new EnrollmentResponseDTO
            { Id = 1, UserId = userId, SubGroupId = subGroupId, User = userResponseDto, SubGroup = subGroupDto };

        _mockMapper.Setup(m => m.Map<Enrollment>(dto)).Returns(entity);
        _mockRepository.Setup(r => r.AddEnrollmentAsync(entity)).ReturnsAsync(entity);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<EnrollmentResponseDTO>(entity)).Returns(responseDto);

        await Assert.ThrowsAsync<EntityValidationException>(() => _service.CreateUserEnrollment(dto));

        _mockRepository.Verify(r => r.AddEnrollmentAsync(entity), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }


    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    public async Task CreateTeacherValidData(int userId, int facultyId)
    {
        var dto = new TeacherPostDTO { UserId = userId, FacultyId = facultyId };
        var user = new User
        {
            Id = userId, FirstName = "Andrei", LastName = "Rotaru", Email = "andrei@gmail.com",
            Password = "TestPassword", PhoneNumber = "+40777301089", Role = Enum.Parse<UserRole>("Teacher")
        };
        _mockUserRepository
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        _mockRepository
            .Setup(r => r.GetFacultyByIdAsync(facultyId))
            .ReturnsAsync(faculty);
        var teacher = new Teacher { Id = 1, UserId = userId, User = user, FacultyId = facultyId, Faculty = faculty };
        var userResponseDto = new UserResponseDTO
        {
            Id = userId, FirstName = "Andrei", LastName = "Rotaru", Email = "andrei@gmail.com",
            Password = "TestPassword", PhoneNumber = "+40777301089", Role = "Teacher"
        };
        var responseDto = new TeacherResponseDTO
            { Id = 1, User = userResponseDto, UserId = userId, FacultyId = facultyId };

        _mockRepository.Setup(r => r.GetFacultyByIdAsync(facultyId)).ReturnsAsync(faculty);
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

        _mockMapper.Setup(m => m.Map<Teacher>(dto)).Returns(teacher);
        _mockRepository.Setup(r => r.AddTeacherAsync(teacher)).ReturnsAsync(teacher);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<TeacherResponseDTO>(teacher)).Returns(responseDto);

        var result = await _service.CreateTeacher(dto);

        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(facultyId, result.FacultyId);
        _mockRepository.Verify(r => r.AddTeacherAsync(teacher), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Theory]
    [InlineData(99, 1)]
    [InlineData(1, 99)]
    public async Task CreateTeacherInvalidData(int userId, int facultyId)
    {
        var dto = new TeacherPostDTO { UserId = userId, FacultyId = facultyId };
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User)null); // simulăm lipsa user-ului
        _mockRepository.Setup(r => r.GetFacultyByIdAsync(facultyId))
            .ReturnsAsync((Faculty)null); // simulăm lipsa facultății

        await Assert.ThrowsAsync<EntityValidationException>(() => _service.CreateTeacher(dto));

        _mockRepository.Verify(r => r.AddTeacherAsync(It.IsAny<Teacher>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory]
    [InlineData(1, "Facultatea de Mate-Info")]
    [InlineData(2, "Facultatea de Drept")]
    public async Task GetFacultyByIdValidId(int id, string name)
    {
        var faculty = new Faculty { Id = id, Name = name };
        var dto = new FacultyResponseDTO { Id = id, Name = name };

        _mockRepository.Setup(r => r.GetFacultyByIdAsync(id)).ReturnsAsync(faculty);
        _mockMapper.Setup(m => m.Map<FacultyResponseDTO>(faculty)).Returns(dto);

        var result = await _service.GetFacultyById(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(name, result.Name);
        _mockRepository.Verify(r => r.GetFacultyByIdAsync(id), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(999)]
    public async Task GetFacultyByIdInvalidId(int id)
    {
        _mockRepository.Setup(r => r.GetFacultyByIdAsync(id)).ReturnsAsync((Faculty?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetFacultyById(id));

        _mockRepository.Verify(r => r.GetFacultyByIdAsync(id), Times.Once);
        _mockMapper.Verify(m => m.Map<FacultyResponseDTO>(It.IsAny<Faculty>()), Times.Never);
    }

    [Theory]
    [InlineData(1, "IR1")]
    [InlineData(2, "IR2")]
    public async Task GetGroupYearByIdValid(int id, string year)
    {
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        var specialisation = new Specialisation { Id = 1, Name = "Computer Science", Faculty = faculty };
        var groupYear = new GroupYear { Id = id, Year = year, Specialisation = specialisation };
        var dto = new GroupYearResponseDTO { Id = id, Year = year };

        _mockRepository.Setup(r => r.GetGroupYearByIdAsync(id)).ReturnsAsync(groupYear);
        _mockMapper.Setup(m => m.Map<GroupYearResponseDTO>(groupYear)).Returns(dto);

        var result = await _service.GetGroupYearById(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(year, result.Year);
        _mockRepository.Verify(r => r.GetGroupYearByIdAsync(id), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(999)]
    public async Task GetGroupYearByIdInvalidId(int id)
    {
        _mockRepository.Setup(r => r.GetGroupYearByIdAsync(id)).ReturnsAsync((GroupYear?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetGroupYearById(id));

        _mockRepository.Verify(r => r.GetGroupYearByIdAsync(id), Times.Once);
        _mockMapper.Verify(m => m.Map<GroupYearResponseDTO>(It.IsAny<GroupYear>()), Times.Never);
    }

    [Theory]
    [InlineData("Computer-Science", 1)]
    [InlineData("AI", 1)]
    public async Task GetSpecializationValidId(string name, int specializationId)
    {
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        var specialisation = new Specialisation { Id = specializationId, Name = "Computer Science", Faculty = faculty };
        var dto = new SpecialisationResponseDTO { Id = specializationId, Name = name };

        _mockRepository.Setup(r => r.GetSpecialisationByIdAsync(specializationId)).ReturnsAsync(specialisation);
        _mockMapper.Setup(m => m.Map<SpecialisationResponseDTO>(specialisation)).Returns(dto);

        var result = await _service.GetSpecialisationById(specializationId);

        Assert.NotNull(result);
        Assert.Equal(specializationId, result.Id);
        Assert.Equal(name, result.Name);
        _mockRepository.Verify(r => r.GetSpecialisationByIdAsync(specializationId), Times.Once);
    }


    [Theory]
    [InlineData(0)]
    [InlineData(999)]
    public async Task GetSpecializationInvalidId(int specializationId)
    {
        _mockRepository.Setup(r => r.GetSpecialisationByIdAsync(specializationId)).ReturnsAsync((Specialisation?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetSpecialisationById(specializationId));

        _mockRepository.Verify(r => r.GetSpecialisationByIdAsync(specializationId), Times.Once);
        _mockMapper.Verify(m => m.Map<Specialisation>(It.IsAny<Specialisation>()), Times.Never);
    }

    [Theory]
    [InlineData("IR1", 1)]
    [InlineData("IR2", 1)]
    public async Task GetStudentGroupValidId(string name, int studentGroupId)
    {
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        var specialisation = new Specialisation { Id = 1, Name = "Computer Science", Faculty = faculty };
        var groupYear = new GroupYear { Id = 1, Year = "IR1", Specialisation = specialisation };
        var entity = new StudentGroup { Id = 1, Name = name, GroupYear = groupYear };
        var dto = new StudentGroupResponseDTO { Id = studentGroupId, Name = name };

        _mockRepository.Setup(r => r.GetGroupByIdAsync(studentGroupId)).ReturnsAsync(entity);
        _mockMapper.Setup(m => m.Map<StudentGroupResponseDTO>(entity)).Returns(dto);

        var result = await _service.GetStudentGroupById(studentGroupId);

        Assert.NotNull(result);
        Assert.Equal(studentGroupId, result.Id);
        Assert.Equal(name, result.Name);
        _mockRepository.Verify(r => r.GetGroupByIdAsync(studentGroupId), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(999)]
    public async Task GetStudentGroupInvalidId(int studentGroupId)
    {
        _mockRepository.Setup(r => r.GetGroupByIdAsync(studentGroupId)).ReturnsAsync((StudentGroup?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetStudentGroupById(studentGroupId));

        _mockRepository.Verify(r => r.GetGroupByIdAsync(studentGroupId), Times.Once);
        _mockMapper.Verify(m => m.Map<StudentGroupResponseDTO>(It.IsAny<StudentGroup>()), Times.Never);
    }

    [Theory]
    [InlineData("236/1", 1)]
    public async Task GetStudentSubGroupValidId(string name, int studentSubGroupId)
    {
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        var specialisation = new Specialisation { Id = 1, Name = "Computer Science", Faculty = faculty };
        var groupYear = new GroupYear { Id = 1, Year = "IR1", Specialisation = specialisation };
        var studentGroup = new StudentGroup { Id = 1, Name = name, GroupYear = groupYear };
        var entity = new StudentSubGroup { Id = studentSubGroupId, Name = name, StudentGroup = studentGroup };
        var dto = new StudentSubGroupResponseDTO { Id = studentSubGroupId, Name = name };

        _mockRepository.Setup(r => r.GetSubGroupByIdAsync(studentSubGroupId)).ReturnsAsync(entity);
        _mockMapper.Setup(m => m.Map<StudentSubGroupResponseDTO>(entity)).Returns(dto);

        var result = await _service.GetStudentSubGroupById(studentSubGroupId);

        Assert.NotNull(result);
        Assert.Equal(studentSubGroupId, result.Id);
        Assert.Equal(name, result.Name);
        _mockRepository.Verify(r => r.GetSubGroupByIdAsync(studentSubGroupId), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(999)]
    public async Task GetStudentSubGroupInvalidId(int studentSubGroupId)
    {
        _mockRepository.Setup(r => r.GetSubGroupByIdAsync(studentSubGroupId)).ReturnsAsync((StudentSubGroup?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetStudentSubGroupById(studentSubGroupId));

        _mockRepository.Verify(r => r.GetSubGroupByIdAsync(studentSubGroupId), Times.Once);
        _mockMapper.Verify(m => m.Map<StudentSubGroupResponseDTO>(It.IsAny<StudentSubGroup>()), Times.Never);
    }

    [Theory]
    [InlineData(1, 1)]
    public async Task GetUserEnrollmentValid(int userId, int subGroupId)
    {
        var dto = new EnrollmentPostDTO { UserId = userId, SubGroupId = subGroupId };
        var user = new User
        {
            Id = userId, FirstName = "Andrei", LastName = "Rotaru", Email = "andrei@gmail.com", Password = "111222ppa",
            PhoneNumber = "+40777301089", Role = Enum.Parse<UserRole>("Admin")
        };
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        var specialisation = new Specialisation { Id = 1, Name = "Computer Science", Faculty = faculty };
        var groupYear = new GroupYear { Id = 1, Year = "IR1", Specialisation = specialisation };
        var studentGroup = new StudentGroup { Id = 1, Name = "234", GroupYear = groupYear };
        var subGroup = new StudentSubGroup { Id = 1, Name = "234/1", StudentGroup = studentGroup };
        var userResponseDto = new UserResponseDTO
        {
            Id = userId, FirstName = "Andrei", LastName = "Rotaru", Email = "andrei@gmail.com", Password = "111222ppa",
            PhoneNumber = "+40777301089", Role = "Admin"
        };
        var subGroupDto = new StudentSubGroupResponseDTO { Id = subGroupId, Name = subGroup.Name };
        var enrollment = new Enrollment { Id = 1, User = user, SubGroup = subGroup };
        var enrollments = new List<Enrollment> { enrollment };

        var enrollmentDto = new EnrollmentResponseDTO
            { Id = 1, UserId = user.Id, SubGroupId = subGroup.Id, User = userResponseDto, SubGroup = subGroupDto };

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _mockRepository.Setup(r => r.GetEnrollmentsByUserId(userId)).ReturnsAsync(enrollments);

        _mockMapper.Setup(m => m.Map<List<EnrollmentResponseDTO>>(enrollments))
            .Returns(new List<EnrollmentResponseDTO> { enrollmentDto });

        var result = await _service.GetUserEnrollments(userId);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(enrollmentDto.Id, result[0].Id);
        Assert.Equal(enrollmentDto.UserId, result[0].UserId);
        Assert.Equal(enrollmentDto.SubGroupId, result[0].SubGroupId);

        _mockUserRepository.Verify(r => r.GetByIdAsync(userId), Times.Once);
        _mockRepository.Verify(r => r.GetEnrollmentsByUserId(userId), Times.Once);

        _mockRepository.Verify(r => r.AddEnrollmentAsync(It.IsAny<Enrollment>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }


    [Theory]
    [InlineData(0)]
    [InlineData(999)]
    public async Task GetUserEnrollmentsInvalidId(int userId)
    {
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetUserEnrollments(userId));

        _mockUserRepository.Verify(r => r.GetByIdAsync(userId), Times.Once);
        _mockRepository.Verify(r => r.GetEnrollmentsByUserId(It.IsAny<int>()), Times.Never);
    }

    [Theory]
    [InlineData(1)]
    public async Task GetTeacherValidId(int teacherId)
    {
        var user = new User
        {
            Id = 1, FirstName = "Andrei", LastName = "Rotaru", Email = "andrei@gmail.com", Password = "TestPassword",
            PhoneNumber = "+40777301089", Role = Enum.Parse<UserRole>("Teacher")
        };
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        var teacher = new Teacher { Id = 1, UserId = 1, User = user, FacultyId = 1, Faculty = faculty };
        var userResponseDto = new UserResponseDTO
        {
            Id = 1, FirstName = "Andrei", LastName = "Rotaru", Email = "andrei@gmail.com", Password = "TestPassword",
            PhoneNumber = "+40777301089", Role = "Teacher"
        };
        var responseDto = new TeacherResponseDTO { Id = 1, User = userResponseDto, UserId = 1, FacultyId = 1 };

        _mockRepository.Setup(r => r.GetTeacherById(teacherId)).ReturnsAsync(teacher);
        _mockMapper.Setup(m => m.Map<TeacherResponseDTO>(teacher)).Returns(responseDto);

        var result = await _service.GetTeacherById(teacherId);

        Assert.NotNull(result);
        Assert.Equal(teacherId, result.Id);
        _mockRepository.Verify(r => r.GetTeacherById(teacherId), Times.Once);
    }

    [Theory]
    [InlineData(99)]
    [InlineData(-1)]
    public async Task GetTeacherByIdInvalidId(int teacherId)
    {
        _mockRepository.Setup(r => r.GetTeacherById(teacherId)).ReturnsAsync((Teacher)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetTeacherById(teacherId));

        _mockRepository.Verify(r => r.GetTeacherById(teacherId), Times.Once);
    }
}