using Moq;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Enums;
using TrackForUBB.Domain.Exceptions.Custom;
using TrackForUBB.Repository.EFEntities;
using TrackForUBB.Service;
using TrackForUBB.Service.EmailService.Interfaces;
using TrackForUBB.Service.Interfaces;
using TrackForUBB.Service.Validators;
using Xunit;

namespace TrackForUBB.BackendTests;

public class GradeServiceTests
{
    private readonly Mock<IGradeRepository> _mockRepository = new();
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<ITimetableRepository> _mockTimetableRepository = new();
    private readonly Mock<IAcademicRepository> _mockAcademicRepository = new();

    private readonly Mock<IEmailProvider> _mockEmailProvider = new();
    private readonly IValidatorFactory _validatorFactory;
    private readonly GradeService _service;

    public GradeServiceTests()
    {
        var gradeValidator = new GradePostDTOValidator(_mockRepository.Object, _mockAcademicRepository.Object,
            _mockTimetableRepository.Object);
        var mockValidatorFactory = new Mock<IValidatorFactory>();

        mockValidatorFactory.Setup(v => v.Get<GradePostDTO>()).Returns(gradeValidator);
        _validatorFactory = mockValidatorFactory.Object;

        _service = new GradeService(
            _mockRepository.Object,
            _mockUserRepository.Object,
            _mockAcademicRepository.Object,
            _mockTimetableRepository.Object,
            _validatorFactory,
            _mockEmailProvider.Object
        );
    }

    [Theory]
    [InlineData(1, 1, 1, 10)]
    [InlineData(2, 3, 2, 8)]
    public async Task CreateGradeValidData(
        int teacherUserId, int subjectId, int enrollmentId, int value)
    {
        var postDto = new GradePostDTO
        {
            SubjectId = subjectId,
            EnrollmentId = enrollmentId,
            Value = value
        };

        var teacherUser = new UserResponseDTO
        {
            Id = 1,
            FirstName = "Andrei",
            LastName = "Rotaru",
            Email = "andrei@gmail.com",
            PhoneNumber = "+40777301089",
            Role = UserRole.Teacher,
            TenantEmail = "rats, there are rats in the walls",
            Owner = ""
        };

        var teacherId = teacherUserId * 3 + 1;

        var teacher = new TeacherResponseDTO
        {
            Id = teacherId,
            User = teacherUser,
            UserId = teacherUser.Id,
            FacultyId = 91,
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(teacherUserId))
            .ReturnsAsync(teacherUser);

        _mockRepository.Setup(r => r.TeacherTeachesSubjectAsync(teacherId, subjectId))
            .ReturnsAsync(true);

        _mockAcademicRepository.Setup(r => r.GetTeacherByUserId(teacherUserId))
            .ReturnsAsync(teacher);

        var promotion = new PromotionResponseDTO { Id = 1, StartYear = 2023, EndYear = 2025 };
        var year = new PromotionYearResponseDTO { Id = 1, Promotion = promotion, YearNumber = 2 };
        var subGroup = new StudentSubGroupResponseDTO { Id = 1, Name = "235/1", };
        var semester = new PromotionSemesterResponseDTO { Id = 1, SemesterNumber = 1, PromotionYear = year };
        _mockAcademicRepository.Setup(r => r.GetSemesterByIdAsync(semester.Id)).ReturnsAsync(semester);
        var subject = new SubjectResponseDTO { Id = subjectId, Name = "TestSubject", NumberOfCredits = 5, Code = "TestCode", Type = "Facultative", };
        _mockTimetableRepository.Setup(r => r.GetSubjectByIdAsync(subject.Id)).ReturnsAsync(subject);
        var subgroupDto = new StudentSubGroupResponseDTO
        {
            Id = subGroup.Id,
            Name = subGroup.Name
        };

        var userDto = new UserResponseDTO
        {
            Id = 100,
            FirstName = "Gigel",
            LastName = "Popescu",
            Email = "gigel@student.com",
            PhoneNumber = "+40777301089",
            Role = UserRole.Student,
            TenantEmail = "he is watching you",
            Owner = ""
        };

        var enrollmentDto = new EnrollmentResponseDTO
        {
            Id = enrollmentId,
            UserId = userDto.Id,
            SubGroupId = subgroupDto.Id,
            User = userDto,
            SubGroup = subgroupDto
        };
        _mockAcademicRepository.Setup(r => r.GetEnrollmentByIdAsync(enrollmentDto.Id)).ReturnsAsync(enrollmentDto);
        var gradeResponse = new GradeResponseDTO
        {
            Id = 10,
            Value = value,
            Subject = subject,
            Enrollment = enrollmentDto,
            Semester = semester,
        };
        _mockAcademicRepository.Setup(r => r.GetEnrollmentsByUserId(userDto.Id))
            .ReturnsAsync(new List<EnrollmentResponseDTO>());
        _mockRepository
            .Setup(r => r.TeacherTeachesSubjectAsync(teacherUserId, subjectId))
            .ReturnsAsync(true);

        _mockRepository
            .Setup(r => r.AddGradeAsync(postDto))
            .ReturnsAsync(gradeResponse);

        _mockRepository
            .Setup(r => r.GetSubjectsForSemesterAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<SubjectResponseDTO>());

        _mockRepository
            .Setup(r => r.GetGradesForStudentInSemesterAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new List<GradeResponseDTO>());


        var result = await _service.CreateGrade(teacherUserId, postDto);

        Assert.NotNull(result);
        Assert.Equal(value, result.Value);

        _mockRepository.Verify(r => r.AddGradeAsync(postDto), Times.Once);
    }

    [Theory]
    [InlineData(-1, 1, 1, 10)]
    [InlineData(1, 1, 1, 11)]
    [InlineData(1, 1, 1, 0)]
    [InlineData(1, 99, 1, 8)]
    [InlineData(1, 1, 1, 8)]
    public async Task CreateGradeInvalidData(
        int teacherId, int subjectId, int enrollmentId, int value)
    {
        var postDto = new GradePostDTO
        {
            SubjectId = subjectId,
            EnrollmentId = enrollmentId,
            Value = value
        };

        if (teacherId == -1)
        {
            _mockUserRepository
                .Setup(r => r.GetByIdAsync(teacherId))
                .ReturnsAsync((UserResponseDTO?)null);

            await Assert.ThrowsAsync<EntityValidationException>(() =>
                _service.CreateGrade(teacherId, postDto));

            return;
        }

        var teacher = new UserResponseDTO
        {
            Id = teacherId,
            FirstName = "Test",
            LastName = "Teacher",
            Email = "t@test.com",
            PhoneNumber = "+40123456789",
            TenantEmail = "do not walk outside on 27/07/2027",
            Role = teacherId == 1 ? UserRole.Teacher : UserRole.Student,
            Owner = ""
        };

        _mockUserRepository
            .Setup(r => r.GetByIdAsync(teacherId))
            .ReturnsAsync(teacher);

        if (teacher.Role != UserRole.Teacher)
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.CreateGrade(teacherId, postDto));
            return;
        }

        if (subjectId == 99)
        {
            _mockRepository
                .Setup(r => r.TeacherTeachesSubjectAsync(teacherId, subjectId))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<EntityValidationException>(() =>
                _service.CreateGrade(teacherId, postDto));

            return;
        }

        if (value < 1 || value > 10)
        {
            _mockRepository
                .Setup(r => r.TeacherTeachesSubjectAsync(teacherId, subjectId))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<EntityValidationException>(() =>
                _service.CreateGrade(teacherId, postDto));

            return;
        }

        _mockRepository.Verify(r => r.AddGradeAsync(It.IsAny<GradePostDTO>()), Times.Never);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    public async Task GetGradeByIdAsyncValidId(int id)
    {
        var promotion = new PromotionResponseDTO { Id = 1, StartYear = 2023, EndYear = 2025 };
        var year = new PromotionYearResponseDTO { Id = 1, Promotion = promotion, YearNumber = 2 };
        var subGroup = new StudentSubGroupResponseDTO { Id = 1, Name = "235/1", };
        var semester = new PromotionSemesterResponseDTO { Id = 1, SemesterNumber = 1, PromotionYear = year };
        _mockAcademicRepository.Setup(r => r.GetSemesterByIdAsync(semester.Id)).ReturnsAsync(semester);
        var subject = new SubjectResponseDTO { Id = 1, Name = "TestSubject", NumberOfCredits = 5, Code = "TestCode", Type = "Required", };
        _mockTimetableRepository.Setup(r => r.GetSubjectByIdAsync(subject.Id)).ReturnsAsync(subject);
        var subgroupDto = new StudentSubGroupResponseDTO
        {
            Id = subGroup.Id,
            Name = subGroup.Name
        };

        var userDto = new UserResponseDTO
        {
            Id = 100,
            FirstName = "Gigel",
            LastName = "Popescu",
            Email = "gigel@student.com",
            PhoneNumber = "+40777301089",
            TenantEmail = "the cake is a lie",
            Role = UserRole.Student,
            Owner = ""
        };

        var enrollmentDto = new EnrollmentResponseDTO
        {
            Id = 1,
            UserId = userDto.Id,
            SubGroupId = subgroupDto.Id,
            User = userDto,
            SubGroup = subgroupDto
        };
        _mockAcademicRepository.Setup(r => r.GetEnrollmentByIdAsync(enrollmentDto.Id)).ReturnsAsync(enrollmentDto);
        var gradeResponse = new GradeResponseDTO
        {
            Id = id,
            Value = 9,
            Subject = subject,
            Enrollment = enrollmentDto,
            Semester = semester,
        };
        _mockRepository.Setup(r => r.GetGradeByIdAsync(id))
            .ReturnsAsync(gradeResponse);

        var result = await _service.GetGradeByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Theory]
    [InlineData(999)]
    public async Task GetGradeByIdAsyncNotFound(int id)
    {
        _mockRepository.Setup(r => r.GetGradeByIdAsync(id))
            .ReturnsAsync((GradeResponseDTO?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.GetGradeByIdAsync(id)
        );
    }

    [Theory]
    [InlineData(1, 1, 1, "Informatica")]
    [InlineData(1, null, 2, "Mate")]
    public async Task GetGradesFilteredAsyncValidInput(
        int userId, int? year, int? sem, string spec)
    {
        var teacher = new UserResponseDTO
        {
            Id = 1,
            FirstName = "Andrei",
            LastName = "Rotaru",
            Email = "andrei@gmail.com",
            PhoneNumber = "+40777301089",
            TenantEmail = "CAN YOU HEAR ME?",
            Role = UserRole.Teacher,
            Owner = ""
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(teacher.Id))
            .ReturnsAsync(teacher);

        var promotion = new PromotionResponseDTO { Id = 1, StartYear = 2023, EndYear = 2025 };
        var yearResponse = new PromotionYearResponseDTO { Id = 1, Promotion = promotion, YearNumber = 2 };
        var subGroup = new StudentSubGroupResponseDTO { Id = 1, Name = "235/1", };
        var semester = new PromotionSemesterResponseDTO { Id = 1, SemesterNumber = 1, PromotionYear = yearResponse };
        _mockAcademicRepository.Setup(r => r.GetSemesterByIdAsync(semester.Id)).ReturnsAsync(semester);
        var subject = new SubjectResponseDTO
        {
            Id = 1,
            Name = "TestSubject",
            NumberOfCredits = 5,
            Code = "TestCode",
            Type = "Optional",
        };
        _mockTimetableRepository.Setup(r => r.GetSubjectByIdAsync(subject.Id)).ReturnsAsync(subject);
        var subgroupDto = new StudentSubGroupResponseDTO
        {
            Id = subGroup.Id,
            Name = subGroup.Name
        };
        _mockRepository.Setup(r => r.TeacherTeachesSubjectAsync(teacher.Id, subject.Id))
            .ReturnsAsync(true);

        var userDto = new UserResponseDTO
        {
            Id = 100,
            FirstName = "Gigel",
            LastName = "Popescu",
            Email = "gigel@student.com",
            PhoneNumber = "+40777301089",
            Role = UserRole.Student,
            TenantEmail = "At the crossroads, don't turn left",
            Owner = ""
        };

        var enrollmentDto = new EnrollmentResponseDTO
        {
            Id = 1,
            UserId = userDto.Id,
            SubGroupId = subgroupDto.Id,
            User = userDto,
            SubGroup = subgroupDto
        };
        _mockAcademicRepository.Setup(r => r.GetEnrollmentByIdAsync(enrollmentDto.Id)).ReturnsAsync(enrollmentDto);

        var gradeResponse = new GradeResponseDTO
        {
            Id = 10,
            Value = 10,
            Subject = subject,
            Enrollment = enrollmentDto,
            Semester = semester,
        };
        var gradeList = new List<GradeResponseDTO>
        {
            gradeResponse
        };
        _mockAcademicRepository.Setup(r => r.GetEnrollmentsByUserId(userDto.Id))
            .ReturnsAsync(new List<EnrollmentResponseDTO>());

        _mockRepository.Setup(r =>
                r.GetGradesFilteredAsync(userId, year, sem, spec))
            .ReturnsAsync(gradeList);

        var result = await _service.GetGradesFiteredAsync(userId, year, sem, spec);

        Assert.NotNull(result);
        Assert.Single(result);
    }
}
