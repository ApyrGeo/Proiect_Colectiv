using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Exceptions.Custom;
using TrackForUBB.Service;
using TrackForUBB.Service.Validators;
using Moq;
using Xunit;
using IValidatorFactory = TrackForUBB.Service.Interfaces.IValidatorFactory;
using TrackForUBB.Service.EmailService.Interfaces;
using TrackForUBB.Repository.EFEntities;
using TrackForUBB.Domain.Enums;
using TrackForUBB.Service.Interfaces;

namespace TrackForUBB.BackendTests;

public class AcademicsServiceTests
{
    private readonly Mock<IAcademicRepository> _mockRepository = new();
    private readonly Mock<IUserRepository> _mockUserRepository = new();

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
        mockValidatorFactory.Setup(v => v.Get<PromotionPostDTO>()).Returns(groupYearValidator);
        mockValidatorFactory.Setup(v => v.Get<SpecialisationPostDTO>()).Returns(specialisationValidator);
        mockValidatorFactory.Setup(v => v.Get<StudentGroupPostDTO>()).Returns(studentGroupValidator);
        mockValidatorFactory.Setup(v => v.Get<StudentSubGroupPostDTO>()).Returns(studentSubGroupValidator);
        mockValidatorFactory.Setup(v => v.Get<EnrollmentPostDTO>()).Returns(enrollmentValidator);
        mockValidatorFactory.Setup(v => v.Get<TeacherPostDTO>()).Returns(teacherValidator);
        _validatorFactory = mockValidatorFactory.Object;

        _service = new AcademicsService(
            _mockRepository.Object,
            _mockUserRepository.Object,
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
        var responseDto = new FacultyResponseDTO { Id = 1, Name = name };
        _mockRepository.Setup(r => r.AddFacultyAsync(postDto)).ReturnsAsync(responseDto);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateFaculty(postDto);

        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
        _mockRepository.Verify(r => r.AddFacultyAsync(postDto), Times.Once);
    }

    [Theory]
    [InlineData("")]
    public async Task CreateFacultyInvalidData(string name)
    {
        var postDto = new FacultyPostDTO { Name = name };

        await Assert.ThrowsAsync<EntityValidationException>(() => _service.CreateFaculty(postDto));
        _mockRepository.Verify(r => r.AddFacultyAsync(postDto), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory]
    [InlineData("IR1", 1)]
    [InlineData("IE3", 1)]
    public async Task CreatePromotionValid(string year, int specialisationId)
    {
        var dto = new PromotionPostDTO { StartYear = 2023, EndYear = 2025, SpecialisationId = specialisationId };
        var specialisationResponse = new SpecialisationResponseDTO { Id = 1, Name = "Computer Science" };
        _mockRepository
            .Setup(r => r.GetSpecialisationByIdAsync(specialisationId))
            .ReturnsAsync(specialisationResponse);
        var responseDto = new PromotionResponseDTO { Id = 1, StartYear = 2023, EndYear = 2025 };

        _mockRepository.Setup(r => r.AddPromotionAsync(dto)).ReturnsAsync(responseDto);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreatePromotion(dto);

        Assert.NotNull(result);
        Assert.Equal(2023, result.StartYear);
        Assert.Equal(2025, result.EndYear);
        _mockRepository.Verify(r => r.AddPromotionAsync(dto), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }


    [Theory]
    [InlineData("", 1)]
    [InlineData("IE3", -1)]
    public async Task CreatePromotionInvalidData(string year, int specialisationId)
    {
        var dto = new PromotionPostDTO { StartYear = 2023, EndYear = 2025, SpecialisationId = specialisationId };
        var specialisationResponse = new SpecialisationResponseDTO { Id = 1, Name = "Computer Science" };
        _mockRepository
            .Setup(r => r.GetSpecialisationByIdAsync(specialisationId))
            .ReturnsAsync(specialisationResponse);
        var responseDto = new PromotionResponseDTO { Id = 1, StartYear = 2023, EndYear = 2025 };

        _mockRepository.Setup(r => r.AddPromotionAsync(dto)).ReturnsAsync(responseDto);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        await Assert.ThrowsAsync<EntityValidationException>(() => _service.CreatePromotion(dto));

        _mockRepository.Verify(r => r.AddPromotionAsync(dto), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
    
    [Theory]
    [InlineData("Computer-Science", 1)]
    [InlineData("AI", 1)]
    public async Task CreateSpecialisationValidData(string name, int facultyId)
    {
        var dto = new SpecialisationPostDTO { Name = name, FacultyId = facultyId };
        var faculty = new FacultyResponseDTO { Id = 1, Name = "Facultate de Mate-Info" };
        _mockRepository
            .Setup(r => r.GetFacultyByIdAsync(facultyId))
            .ReturnsAsync(faculty);
        var responseDto = new SpecialisationResponseDTO { Id = 1, Name = name };
        
        _mockRepository.Setup(r => r.AddSpecialisationAsync(dto)).ReturnsAsync(responseDto);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        
        var result = await _service.CreateSpecialisation(dto);

        Assert.NotNull(result);
        Assert.Equal(name, result.Name);

        _mockRepository.Verify(r => r.AddSpecialisationAsync(dto), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Theory]
    [InlineData("", 1)]
    [InlineData("AI", -1)]
    public async Task CreateSpecialisationInvalidData(string name, int facultyId)
    {
        var dto = new SpecialisationPostDTO { Name = name, FacultyId = facultyId };
        var faculty = new FacultyResponseDTO { Id = 1, Name = "Facultate de Mate-Info" };
        _mockRepository
            .Setup(r => r.GetFacultyByIdAsync(facultyId))
            .ReturnsAsync(faculty);
        var responseDto = new SpecialisationResponseDTO { Id = 1, Name = name };

        _mockRepository.Setup(r => r.AddSpecialisationAsync(dto)).ReturnsAsync(responseDto);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        await Assert.ThrowsAsync<EntityValidationException>(() => _service.CreateSpecialisation(dto));

        _mockRepository.Verify(r => r.AddSpecialisationAsync(dto), Times.Never);
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
        var groupYear = new Promotion { Id = groupYearId, StartYear = 2023, EndYear = 2025, Specialisation = specialisation };
        var groupYearResponse = new PromotionResponseDTO { Id = 1, StartYear = groupYear.StartYear, EndYear = groupYear.EndYear };
        _mockRepository
            .Setup(r => r.GetPromotionByIdAsync(groupYearId))
            .ReturnsAsync(groupYearResponse);

        var responseDto = new StudentGroupResponseDTO { Id = 1, Name = name };

        _mockRepository.Setup(r => r.AddGroupAsync(dto)).ReturnsAsync(responseDto);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        
        var result = await _service.CreateStudentGroup(dto);

        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
        _mockRepository.Verify(r => r.AddGroupAsync(dto), Times.Once);
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
        var groupYear = new Promotion { Id = groupYearId, StartYear = 2023, EndYear = 2025, Specialisation = specialisation };
        var groupYearResponse = new PromotionResponseDTO { Id = groupYearId, StartYear = groupYear.StartYear, EndYear = groupYear.EndYear };
        _mockRepository
            .Setup(r => r.GetPromotionByIdAsync(groupYearId))
            .ReturnsAsync(groupYearResponse);

        var responseDto = new StudentGroupResponseDTO { Id = 1, Name = name };
        
        _mockRepository.Setup(r => r.AddGroupAsync(dto)).ReturnsAsync(responseDto);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        
        await Assert.ThrowsAsync<EntityValidationException>(() => _service.CreateStudentGroup(dto));

        _mockRepository.Verify(r => r.AddGroupAsync(dto), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory]
    [InlineData("236/1", 1)]
    public async Task CreateStudentSubGroupValidData(string name, int studentGroupId)
    {
        var dto = new StudentSubGroupPostDTO { Name = name, StudentGroupId = studentGroupId };
        var studentGroupResponse = new StudentGroupResponseDTO { Id = studentGroupId, Name = name };
        _mockRepository
            .Setup(r => r.GetGroupByIdAsync(studentGroupId))
            .ReturnsAsync(studentGroupResponse);

        var responseDto = new StudentSubGroupResponseDTO { Id = 1, Name = name };
        
        _mockRepository.Setup(r => r.AddSubGroupAsync(dto)).ReturnsAsync(responseDto);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        
        var result = await _service.CreateStudentSubGroup(dto);

        Assert.NotNull(result);
        Assert.Equal(name, result.Name);

        _mockRepository.Verify(r => r.AddSubGroupAsync(dto), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Theory]
    [InlineData("", 1)]
    [InlineData("", -1)]
    public async Task CreateStudentSubGroupInvalidData(string name, int studentGroupId)
    {
        var dto = new StudentSubGroupPostDTO { Name = name, StudentGroupId = studentGroupId };
        var studentGroupResponse = new StudentGroupResponseDTO { Id = studentGroupId, Name = name };
        _mockRepository
            .Setup(r => r.GetGroupByIdAsync(studentGroupId))
            .ReturnsAsync(studentGroupResponse);

        var responseDto = new StudentSubGroupResponseDTO { Id = 1, Name = name };
        
        _mockRepository.Setup(r => r.AddSubGroupAsync(dto)).ReturnsAsync(responseDto);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        
        await Assert.ThrowsAsync<EntityValidationException>(() => _service.CreateStudentSubGroup(dto));

        _mockRepository.Verify(r => r.AddSubGroupAsync(dto), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory]
    [InlineData(1, 1)]
    public async Task CreateUserEnrollmentValidData(int userId, int subGroupId)
    {
        var dto = new EnrollmentPostDTO { UserId = userId, SubGroupId = subGroupId };
        var userResponse = new UserResponseDTO
        {
            Id = 1,
            FirstName = "Andrei",
            LastName = "Rotaru",
            Email = "andrei@gmail.com",
            Password = "111222ppa",
            PhoneNumber = "+40777301089",
            Role = "Student"
        };
        _mockUserRepository
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(userResponse);
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        var specialisation = new Specialisation { Id = 1, Name = "Computer Science", Faculty = faculty };
        var promotion = new Promotion { Id = 1, StartYear = 2023, EndYear = 2025, Specialisation = specialisation };
        var studentGroup = new StudentGroup { Id = 1, Name = "234", Promotion = promotion };
        var subGroup = new StudentSubGroup { Id = 1, Name = "234/1", StudentGroup = studentGroup };
        var subGroupResponse = new StudentSubGroupResponseDTO { Id = 1, Name = subGroup.Name };
        _mockRepository
            .Setup(r => r.GetSubGroupByIdAsync(subGroupId))
            .ReturnsAsync(subGroupResponse);
        var userResponseDto = new UserResponseDTO
        {
            Id = userId,
            FirstName = "Andrei",
            LastName = "Rotaru",
            Email = "andrei@gmail.com",
            Password = "111222ppa",
            PhoneNumber = "+40777301089",
            Role = "Student"
        };
        var subGroupDto = new StudentSubGroupResponseDTO { Id = subGroupId, Name = subGroup.Name };
        var responseDto = new EnrollmentResponseDTO
            { Id = 1, UserId = userId, SubGroupId = subGroupId, User = userResponseDto, SubGroup = subGroupDto };
        
        _mockRepository.Setup(r => r.AddEnrollmentAsync(dto)).ReturnsAsync(responseDto);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        
        var result = await _service.CreateUserEnrollment(dto);

        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);

        _mockRepository.Verify(r => r.AddEnrollmentAsync(dto), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Theory]
    [InlineData(-1, 1)]
    [InlineData(1, -1)]
    public async Task CreateUserEnrollmentInvalidData(int userId, int subGroupId)
    {
        var dto = new EnrollmentPostDTO { UserId = userId, SubGroupId = subGroupId };
        var userResponseDto = new UserResponseDTO
        {
            Id = userId,
            FirstName = "Andrei",
            LastName = "Rotaru",
            Email = "andrei@gmail.com",
            Password = "111222ppa",
            PhoneNumber = "+40777301089",
            Role = "Student"
        };
        _mockUserRepository
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(userResponseDto);
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        var specialisation = new Specialisation { Id = 1, Name = "Computer Science", Faculty = faculty };
        var promotion = new Promotion { Id = 1, StartYear = 2023, EndYear = 2025, Specialisation = specialisation };
        var studentGroup = new StudentGroup { Id = 1, Name = "234", Promotion = promotion };
        var subGroup = new StudentSubGroup { Id = 1, Name = "234/1", StudentGroup = studentGroup };
        var subGroupResponse = new StudentSubGroupResponseDTO { Id = 1, Name = subGroup.Name };
        _mockRepository
            .Setup(r => r.GetSubGroupByIdAsync(subGroupId))
            .ReturnsAsync(subGroupResponse);

        var subGroupDto = new StudentSubGroupResponseDTO { Id = subGroupId, Name = subGroup.Name };
        var responseDto = new EnrollmentResponseDTO
            { Id = 1, UserId = userId, SubGroupId = subGroupId, User = userResponseDto, SubGroup = subGroupDto };
        
        _mockRepository.Setup(r => r.AddEnrollmentAsync(dto)).ReturnsAsync(responseDto);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        
        await Assert.ThrowsAsync<EntityValidationException>(() => _service.CreateUserEnrollment(dto));

        _mockRepository.Verify(r => r.AddEnrollmentAsync(dto), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }


    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    public async Task CreateTeacherValidData(int userId, int facultyId)
    {
        var dto = new TeacherPostDTO { UserId = userId, FacultyId = facultyId };
        var userResponseDto = new UserResponseDTO
        {
            Id = userId,
            FirstName = "Andrei",
            LastName = "Rotaru",
            Email = "andrei@gmail.com",
            Password = "TestPassword",
            PhoneNumber = "+40777301089",
            Role = "Teacher"
        };
        _mockUserRepository
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(userResponseDto);
        var faculty = new FacultyResponseDTO { Id = 1, Name = "Facultate de Mate-Info" };
        _mockRepository
            .Setup(r => r.GetFacultyByIdAsync(facultyId))
            .ReturnsAsync(faculty);
        
        var responseDto = new TeacherResponseDTO
            { Id = 1, User = userResponseDto, UserId = userId, FacultyId = facultyId };

        _mockRepository.Setup(r => r.GetFacultyByIdAsync(facultyId)).ReturnsAsync(faculty);
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(userResponseDto);

        _mockRepository.Setup(r => r.AddTeacherAsync(dto)).ReturnsAsync(responseDto);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        
        var result = await _service.CreateTeacher(dto);

        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(facultyId, result.FacultyId);
        _mockRepository.Verify(r => r.AddTeacherAsync(dto), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Theory]
    [InlineData(99, 1)]
    [InlineData(1, 99)]
    public async Task CreateTeacherInvalidData(int userId, int facultyId)
    {
        var dto = new TeacherPostDTO { UserId = userId, FacultyId = facultyId };
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((UserResponseDTO?)null);
        _mockRepository.Setup(r => r.GetFacultyByIdAsync(facultyId))
            .ReturnsAsync((FacultyResponseDTO?)null);

        await Assert.ThrowsAsync<EntityValidationException>(() => _service.CreateTeacher(dto));

        _mockRepository.Verify(r => r.AddTeacherAsync(It.IsAny<TeacherPostDTO>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory]
    [InlineData(1, "Facultatea de Mate-Info")]
    [InlineData(2, "Facultatea de Drept")]
    public async Task GetFacultyByIdValidId(int id, string name)
    {
        var dto = new FacultyResponseDTO { Id = id, Name = name };

        _mockRepository.Setup(r => r.GetFacultyByIdAsync(id)).ReturnsAsync(dto);

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
        _mockRepository.Setup(r => r.GetFacultyByIdAsync(id)).ReturnsAsync((FacultyResponseDTO?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetFacultyById(id));

        _mockRepository.Verify(r => r.GetFacultyByIdAsync(id), Times.Once);
    }

    [Theory]
    [InlineData(1, 2023, 2025)]
    [InlineData(2, 2024, 2027)]
    public async Task GetPromotionByIdValid(int id, int startYear, int endYear)
    {
        var dto = new PromotionResponseDTO { Id = id, StartYear = startYear, EndYear = endYear };

        _mockRepository.Setup(r => r.GetPromotionByIdAsync(id)).ReturnsAsync(dto);

        var result = await _service.GetPromotionById(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(startYear, result.StartYear);
        Assert.Equal(endYear, result.EndYear);
		_mockRepository.Verify(r => r.GetPromotionByIdAsync(id), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(999)]
    public async Task GetPromotionByIdInvalidId(int id)
    {
        _mockRepository.Setup(r => r.GetPromotionByIdAsync(id)).ReturnsAsync((PromotionResponseDTO?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetPromotionById(id));

        _mockRepository.Verify(r => r.GetPromotionByIdAsync(id), Times.Once);
    }

    [Theory]
    [InlineData("Computer-Science", 1)]
    [InlineData("AI", 1)]
    public async Task GetSpecializationValidId(string name, int specializationId)
    {
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        var specialisation = new Specialisation { Id = specializationId, Name = "Computer Science", Faculty = faculty };
        var dto = new SpecialisationResponseDTO { Id = specializationId, Name = name };

        _mockRepository.Setup(r => r.GetSpecialisationByIdAsync(specializationId)).ReturnsAsync(dto);

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
        _mockRepository.Setup(r => r.GetSpecialisationByIdAsync(specializationId))
            .ReturnsAsync((SpecialisationResponseDTO?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetSpecialisationById(specializationId));

        _mockRepository.Verify(r => r.GetSpecialisationByIdAsync(specializationId), Times.Once);
    }

    [Theory]
    [InlineData("IR1", 1)]
    [InlineData("IR2", 1)]
    public async Task GetStudentGroupValidId(string name, int studentGroupId)
    {
        var dto = new StudentGroupResponseDTO { Id = studentGroupId, Name = name };

        _mockRepository.Setup(r => r.GetGroupByIdAsync(studentGroupId)).ReturnsAsync(dto);

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
        _mockRepository.Setup(r => r.GetGroupByIdAsync(studentGroupId)).ReturnsAsync((StudentGroupResponseDTO?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetStudentGroupById(studentGroupId));

        _mockRepository.Verify(r => r.GetGroupByIdAsync(studentGroupId), Times.Once);
    }

    [Theory]
    [InlineData("236/1", 1)]
    public async Task GetStudentSubGroupValidId(string name, int studentSubGroupId)
    {
        var dto = new StudentSubGroupResponseDTO { Id = studentSubGroupId, Name = name };

        _mockRepository.Setup(r => r.GetSubGroupByIdAsync(studentSubGroupId)).ReturnsAsync(dto);

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
        _mockRepository.Setup(r => r.GetSubGroupByIdAsync(studentSubGroupId))
            .ReturnsAsync((StudentSubGroupResponseDTO?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetStudentSubGroupById(studentSubGroupId));

        _mockRepository.Verify(r => r.GetSubGroupByIdAsync(studentSubGroupId), Times.Once);
    }

    [Theory]
    [InlineData(1, 1)]
    public async Task GetUserEnrollmentValid(int userId, int subGroupId)
    {
        var dto = new EnrollmentPostDTO { UserId = userId, SubGroupId = subGroupId };

        var userResponseDto = new UserResponseDTO
        {
            Id = userId,
            FirstName = "Andrei",
            LastName = "Rotaru",
            Email = "andrei@gmail.com",
            Password = "111222ppa",
            PhoneNumber = "+40777301089",
            Role = "Admin"
        };

        var subGroupDto = new StudentSubGroupResponseDTO { Id = subGroupId, Name = "234/1" };


        var enrollmentDto = new EnrollmentResponseDTO
        {
            Id = 1,
            UserId = userId,
            SubGroupId = subGroupId,
            User = userResponseDto,
            SubGroup = subGroupDto
        };
        var enrollments = new List<EnrollmentResponseDTO> { enrollmentDto };
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(userResponseDto);

        _mockRepository.Setup(r => r.GetEnrollmentsByUserId(userId))
            .ReturnsAsync(enrollments);


        var result = await _service.GetUserEnrollments(userId);

        Assert.NotNull(result);
        Assert.Single(result);

        Assert.Equal(enrollmentDto.Id, result[0].Id);
        Assert.Equal(enrollmentDto.UserId, result[0].UserId);
        Assert.Equal(enrollmentDto.SubGroupId, result[0].SubGroupId);

        _mockUserRepository.Verify(r => r.GetByIdAsync(userId), Times.Once);
        _mockRepository.Verify(r => r.GetEnrollmentsByUserId(userId), Times.Once);
    }


    [Theory]
    [InlineData(0)]
    [InlineData(999)]
    public async Task GetUserEnrollmentsInvalidId(int userId)
    {
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((UserResponseDTO?)null);

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
            Id = 1,
            FirstName = "Andrei",
            LastName = "Rotaru",
            Email = "andrei@gmail.com",
            Password = "TestPassword",
            PhoneNumber = "+40777301089",
            Role = Enum.Parse<UserRole>("Teacher")
        };
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        var teacher = new Teacher { Id = 1, UserId = 1, User = user, FacultyId = 1, Faculty = faculty };
        var userResponseDto = new UserResponseDTO
        {
            Id = 1,
            FirstName = "Andrei",
            LastName = "Rotaru",
            Email = "andrei@gmail.com",
            Password = "TestPassword",
            PhoneNumber = "+40777301089",
            Role = "Teacher"
        };
        var responseDto = new TeacherResponseDTO { Id = 1, User = userResponseDto, UserId = 1, FacultyId = 1 };

        _mockRepository.Setup(r => r.GetTeacherById(teacherId)).ReturnsAsync(responseDto);

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
        _mockRepository.Setup(r => r.GetTeacherById(teacherId)).ReturnsAsync((TeacherResponseDTO?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetTeacherById(teacherId));

        _mockRepository.Verify(r => r.GetTeacherById(teacherId), Times.Once);
    }
}