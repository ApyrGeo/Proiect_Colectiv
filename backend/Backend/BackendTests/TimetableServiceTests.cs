using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Exceptions.Custom;
using TrackForUBB.Service;
using TrackForUBB.Service.Validators;
using Moq;
using Xunit;
using IValidatorFactory = TrackForUBB.Service.Interfaces.IValidatorFactory;
using TrackForUBB.Domain.Utils;
using TrackForUBB.Repository.EFEntities;
using TrackForUBB.Domain.Enums;
using TrackForUBB.Service.Interfaces;

namespace TrackForUBB.BackendTests;

public class TimetableServiceTests
{
    private readonly Mock<ITimetableRepository> _mockRepository = new();
    private readonly IValidatorFactory _validatorFactory;
    private readonly Mock<IAcademicRepository> _mockAcademicRepository = new();
    private readonly Mock<IUserRepository> _mockUserRepository = new();

    private readonly TimetableService _service;

    public TimetableServiceTests()
    {
        var subjectValidator = new SubjectPostDTOValidator(_mockAcademicRepository.Object, _mockUserRepository.Object);
        var classroomValidator = new ClassroomPostDTOValidator(_mockRepository.Object);
        var hourValidator = new HourPostDTOValidator(_mockRepository.Object, _mockAcademicRepository.Object);
        var locationValidator = new LocationPostDTOValidator();
        var hourfilterValidator = new HourFilterValidator(_mockAcademicRepository.Object, _mockRepository.Object);
        var mockValidatorFactory = new Mock<IValidatorFactory>();

        mockValidatorFactory.Setup(v => v.Get<SubjectPostDTO>()).Returns(subjectValidator);
        mockValidatorFactory.Setup(v => v.Get<LocationPostDTO>()).Returns(locationValidator);
        mockValidatorFactory.Setup(v => v.Get<ClassroomPostDTO>()).Returns(classroomValidator);
        mockValidatorFactory.Setup(v => v.Get<HourPostDTO>()).Returns(hourValidator);
        mockValidatorFactory.Setup(v => v.Get<HourFilter>()).Returns(hourfilterValidator);
        _validatorFactory = mockValidatorFactory.Object;

        _service = new TimetableService(
            _mockRepository.Object,
            _mockAcademicRepository.Object,
            _validatorFactory
        );
    }

    [Theory]
    [InlineData("Analiza", 6, 1)]
    [InlineData("FP", 5, 2)]
    public async Task CreateSubjectValidData(string name,
                                             int credits,
                                             int semesterId,
                                             int groupYearId = 9,
                                             string type = "Required",
                                             string code = "Code"
    )
    {
        var promotionResponseDTO = new PromotionResponseDTO
        {
            Id = groupYearId,
            StartYear = 2023,
            EndYear = 2027,
        };

        _mockAcademicRepository
            .Setup(r => r.GetPromotionByIdAsync(groupYearId))
            .ReturnsAsync(promotionResponseDTO);

        var semesterResponseDTO = new PromotionSemesterResponseDTO
        {
            Id = semesterId,
            SemesterNumber = 1,
            PromotionYear = new PromotionYearResponseDTO
            {
                Id = semesterId,
                Promotion = promotionResponseDTO,
                YearNumber = 1,
            }
        };

        _mockAcademicRepository
            .Setup(r => r.GetSemesterByIdAsync(semesterId))
            .ReturnsAsync(semesterResponseDTO);

        var teacherUser = new UserResponseDTO
        {
            Id = 1,
            FirstName = "Andrei",
            LastName = "Rotaru",
            Email = "andrei@gmail.com",
            Role = UserRole.Teacher,
            PhoneNumber = "+40777301089",
            TenantEmail = "andrei.rotaru@trackforubb.onmicrosoft.com",
            Owner = ""
        };

        _mockUserRepository
            .Setup(r => r.GetByIdAsync(teacherUser.Id))
            .ReturnsAsync(teacherUser);

        _mockAcademicRepository
            .Setup(r => r.GetTeacherById(teacherUser.Id))
            .ReturnsAsync(new TeacherResponseDTO
            {
                Id = teacherUser.Id,
                User = teacherUser,
                UserId = teacherUser.Id,
                FacultyId = 8,
            });

        var postDto = new SubjectPostDTO
        {
            Name = name,
            NumberOfCredits = credits,
            SemesterId = semesterId,
            HolderTeacherId = teacherUser.Id,
            Code = code,
            Type = type,
            FormationType = "Course_Seminar",
        };

        _mockRepository.Setup(r => r.AddSubjectAsync(postDto))
            .ReturnsAsync(new SubjectResponseDTO
            {
                Id = 1,
                Name = name,
                NumberOfCredits = credits,
                Code = code,
                Type = type,
            });

        var responseDto = new SubjectResponseDTO
        {
            Id = 1,
            Name = name,
            NumberOfCredits = credits,
            Code = code,
            Type = type,
        };


        _mockRepository.Setup(r => r.AddSubjectAsync(postDto)).ReturnsAsync(responseDto);

        var result = await _service.CreateSubject(postDto);

        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
        Assert.Equal(credits, result.NumberOfCredits);

        _mockRepository.Verify(r => r.AddSubjectAsync(postDto), Times.Once);
    }

    [Theory]
    [InlineData("Analiza", 10, 1)]
    [InlineData("", 5, 2)]
    [InlineData("FP", 0, 3)]
    public async Task CreateSubjectInvalidData(string name, int credits, int semesterId)
    {
        var postDto = new SubjectPostDTO
        {
            Name = name,
            NumberOfCredits = credits,
            SemesterId = semesterId,
            Code = "MLR101",
            Type = "Required",
            FormationType = "Course_Seminar"
        };

        var promotionResponseDTO = new PromotionResponseDTO
        {
            Id = 9,
            StartYear = 2023,
            EndYear = 2027,
        };
        _mockAcademicRepository
            .Setup(r => r.GetPromotionByIdAsync(9))
            .ReturnsAsync(promotionResponseDTO);

        var subjectEntity = new SubjectPostDTO
        {
            Name = name,
            NumberOfCredits = credits,
            SemesterId = semesterId,
            Code = "MLR101",
            FormationType = "Course_Seminar",
            Type = "Required",
        };


        await Assert.ThrowsAsync<EntityValidationException>(() => _service.CreateSubject(postDto));

        _mockRepository.Verify(r => r.AddSubjectAsync(subjectEntity), Times.Never);
    }

    [Theory]
    [InlineData(1, "Analiza", 6)]
    [InlineData(2, "FP", 5)]
    public async Task GetSubjectByIdValidId(int id, string name, int credits)
    {
        var responseDto = new SubjectResponseDTO
        {
            Id = id,
            Name = name,
            NumberOfCredits = credits,
            Type = "Required",
            Code = name,
        };

        _mockRepository.Setup(r => r.GetSubjectByIdAsync(id))
            .ReturnsAsync(responseDto);

        var result = await _service.GetSubjectById(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(name, result.Name);
        Assert.Equal(credits, result.NumberOfCredits);

        _mockRepository.Verify(r => r.GetSubjectByIdAsync(id), Times.Once);
    }

    [Theory]
    [InlineData(999)]
    [InlineData(0)]
    public async Task GetSubjectByIdInvalidId(int invalidId)
    {
        _mockRepository.Setup(r => r.GetSubjectByIdAsync(invalidId))
            .ReturnsAsync((SubjectResponseDTO?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetSubjectById(invalidId));

        _mockRepository.Verify(r => r.GetSubjectByIdAsync(invalidId), Times.Once);
    }

    [Theory]
    [InlineData("Fsega", "Str. Vasile Goldis")]
    [InlineData("Centru", "Str. Universitatii")]
    public async Task CreateLocationValidData(string name, string address)
    {
        var dto = new LocationPostDTO { Name = name, Address = address };
        var googleData = new GoogleMapsDataResponseDTO { Id = "1", Latitude = 2, Longitude = 3 };
        var response = new LocationResponseDTO { Id = 1, Name = name, Address = address, GoogleMapsData = googleData };

        _mockRepository.Setup(r => r.AddLocationAsync(dto)).ReturnsAsync(response);


        var result = await _service.CreateLocation(dto);

        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
        Assert.Equal(address, result.Address);
        _mockRepository.Verify(r => r.AddLocationAsync(dto), Times.Once);
    }

    [Theory]
    [InlineData("", "Str. Vasile Goldis")]
    [InlineData("Centru", "")]
    public async Task CreateLocationInvalidData(string name, string address)
    {
        var dto = new LocationPostDTO { Name = name, Address = address };

        await Assert.ThrowsAsync<EntityValidationException>(() => _service.CreateLocation(dto));

        _mockRepository.Verify(r => r.AddLocationAsync(It.IsAny<LocationPostDTO>()), Times.Never);
    }

    [Theory]
    [InlineData("L002", 1)]
    [InlineData("A2", 2)]
    public async Task CreateClassroomValidData(string name, int locationId)
    {
        var locationResponse = new LocationResponseDTO
        {
            Id = locationId,
            Name = "Centru",
            Address = "Str. Universitatii",
            GoogleMapsData = new GoogleMapsDataResponseDTO
            {
                Id = "1",
                Latitude = 2,
                Longitude = 3
            }
        };

        var postDto = new ClassroomPostDTO
        {
            Name = name,
            LocationId = locationId
        };

        var responseDto = new ClassroomResponseDTO
        {
            Id = 1,
            Name = name,
            LocationId = locationId
        };

        _mockRepository.Setup(r => r.GetLocationByIdAsync(locationId))
            .ReturnsAsync(locationResponse);
        _mockRepository.Setup(r => r.AddClassroomAsync(postDto))
            .ReturnsAsync(responseDto);

        var result = await _service.CreateClassroom(postDto);

        Assert.NotNull(result);
        Assert.Equal(name, result.Name);

        _mockRepository.Verify(r => r.AddClassroomAsync(postDto), Times.Once);
    }

    [Theory]
    [InlineData("", 1)]
    [InlineData("Room 305", 0)]
    [InlineData("Room 305", 99)]
    public async Task CreateClassroomInvalidData(string name, int locationId)
    {
        var dto = new ClassroomPostDTO { Name = name, LocationId = locationId };

        _mockRepository.Setup(r => r.GetLocationByIdAsync(locationId)).ReturnsAsync((LocationResponseDTO?)null);

        await Assert.ThrowsAsync<EntityValidationException>(() => _service.CreateClassroom(dto));

        _mockRepository.Verify(r => r.AddClassroomAsync(It.IsAny<ClassroomPostDTO>()), Times.Never);
    }

    [Theory]
    [InlineData("Monday", "08-10", "Weekly", "Lecture", "IR1")]
    [InlineData("Friday", "18-20", "SecondWeek", "Seminar", "231")]
    public async Task CreateHourValidData(
        string day, string hourInterval, string frequency, string category, string format)
    {
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        var promotion = new PromotionResponseDTO { Id = 1, StartYear = 2023, EndYear = 2027 };
        _mockAcademicRepository
            .Setup(r => r.GetPromotionByIdAsync(promotion.Id))
            .ReturnsAsync(promotion);
        var subject = new SubjectResponseDTO { Id = 1, Name = "FP", NumberOfCredits = 6, Code = "FP01", Type = "Optional", };
        var studentGroup = new StudentGroupResponseDTO { Id = 1, Name = "234" };
        _mockAcademicRepository
            .Setup(r => r.GetGroupByIdAsync(studentGroup.Id))
            .ReturnsAsync(studentGroup);
        var subGroup = new StudentSubGroupResponseDTO { Id = 1, Name = "234/1" };
        _mockAcademicRepository
            .Setup(r => r.GetSubGroupByIdAsync(subGroup.Id))
            .ReturnsAsync(subGroup);

        var dto = new HourPostDTO
        {
            Day = day,
            HourInterval = hourInterval,
            Frequency = frequency,
            Category = category,
            SubjectId = 1,
            ClassroomId = 1,
            TeacherId = 1,
            GroupYearId = null,
            StudentGroupId = null,
            StudentSubGroupId = 1
        };

        _mockRepository
            .Setup(r => r.GetSubjectByIdAsync(subject.Id))
            .ReturnsAsync(subject);


        var location = new Location { Id = 1, Name = "Marasti", Address = "Groapa" };
        var classroom = new ClassroomResponseDTO { Id = 1, Name = "A101", LocationId = location.Id };
        _mockRepository
            .Setup(r => r.GetClassroomByIdAsync(classroom.Id))
            .ReturnsAsync(classroom);
        var user = new User
        {
            Id = 1,
            FirstName = "Andrei",
            LastName = "Rotaru",
            Email = "andrei@gmail.com",
            PhoneNumber = "+40777301089",
            TenantEmail = "i am inside your walls",
            Role = UserRole.Teacher
        };
        var userResponse = new UserResponseDTO
        {
            Id = 1,
            FirstName = "Andrei",
            LastName = "Rotaru",
            Email = "andrei@gmail.com",
            PhoneNumber = "+40777301089",
            TenantEmail = "open your window",
            Role = UserRole.Teacher,
            Owner = ""
        };
        var teacher = new TeacherResponseDTO { Id = 1, UserId = user.Id, FacultyId = faculty.Id, User = userResponse };
        _mockAcademicRepository
            .Setup(r => r.GetTeacherById(teacher.Id))
            .ReturnsAsync(teacher);


        var googleData = new GoogleMapsDataResponseDTO { Id = "1", Latitude = 2, Longitude = 3 };
        var locationResponse = new LocationResponseDTO
        { Id = 1, Name = location.Name, Address = location.Address, GoogleMapsData = googleData };
        var classroomResponse = new ClassroomResponseDTO { Id = 1, Name = classroom.Name, LocationId = locationResponse.Id };
        var subjectResponse = new SubjectResponseDTO
        {
            Id = 1,
            Name = subject.Name,
            NumberOfCredits = subject.NumberOfCredits,
            Type = subject.Type,
            Code = subject.Type,
        };
        var teacherResponse = new TeacherResponseDTO
        { Id = teacher.Id, User = userResponse, UserId = userResponse.Id, FacultyId = faculty.Id };
        var response = new HourResponseDTO
        {
            Id = 1,
            Day = dto.Day,
            HourInterval = dto.HourInterval,
            Frequency = dto.Frequency,
            Category = dto.Category,
            Format = format,
            Location = locationResponse,
            Classroom = classroomResponse,
            Subject = subjectResponse,
            Teacher = teacherResponse,
        };


        _mockRepository.Setup(r => r.AddHourAsync(dto)).ReturnsAsync(response);


        var result = await _service.CreateHour(dto);

        Assert.NotNull(result);
        Assert.Equal(dto.HourInterval, result.HourInterval);
        _mockRepository.Verify(r => r.AddHourAsync(dto), Times.Once);
    }

    [Theory]
    [InlineData("Monday", "", "Weekly", "Lecture")]
    [InlineData("Monday", "09-11", "", "Lecture")]
    [InlineData("Monday", "09-11", "Weekly", "")]
    [InlineData("", "09-11", "Weekly", "Lecture")]
    public async Task CreateHourInvalidData(
        string day, string hourInterval, string frequency, string category)
    {
        var dto = new HourPostDTO
        {
            Day = day,
            HourInterval = hourInterval,
            Frequency = frequency,
            Category = category,
            SubjectId = 1,
            ClassroomId = 1,
            TeacherId = 1
        };


        await Assert.ThrowsAsync<EntityValidationException>(() => _service.CreateHour(dto));

        _mockRepository.Verify(r => r.AddHourAsync(It.IsAny<HourPostDTO>()), Times.Never);
    }

    [Theory]
    [InlineData(1, "L001", "Marasti", "Groapa")]
    public async Task GetClassroomByIdValidId(int id, string name, string locName, string address)
    {
        var location = new Location { Id = 1, Name = locName, Address = address };
        var classroom = new Classroom { Id = id, Name = name, Location = location };
        var response = new ClassroomResponseDTO { Id = id, Name = name, LocationId = location.Id };

        _mockRepository.Setup(r => r.GetClassroomByIdAsync(id)).ReturnsAsync(response);


        var result = await _service.GetClassroomById(id);

        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
        _mockRepository.Verify(r => r.GetClassroomByIdAsync(id), Times.Once);
    }

    [Theory]
    [InlineData(999)]
    [InlineData(0)]
    public async Task GetClassroomByIdInvalidId(int id)
    {
        _mockRepository.Setup(r => r.GetClassroomByIdAsync(id)).ReturnsAsync((ClassroomResponseDTO?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetClassroomById(id));

        _mockRepository.Verify(r => r.GetClassroomByIdAsync(id), Times.Once);
    }

    [Theory]
    [InlineData(1, "Marasti", "Groapa")]
    public async Task GetLocationByIdValidId(int id, string name, string address)
    {
        var googleData = new GoogleMapsDataResponseDTO { Id = "1", Latitude = 2, Longitude = 3 };
        var response = new LocationResponseDTO { Id = id, Name = name, Address = address, GoogleMapsData = googleData };

        _mockRepository.Setup(r => r.GetLocationByIdAsync(id)).ReturnsAsync(response);


        var result = await _service.GetLocationById(id);

        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
        _mockRepository.Verify(r => r.GetLocationByIdAsync(id), Times.Once);
    }

    [Theory]
    [InlineData(999)]
    [InlineData(-1)]
    public async Task GetLocationByIdInvalidId(int id)
    {
        _mockRepository.Setup(r => r.GetLocationByIdAsync(id)).ReturnsAsync((LocationResponseDTO?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetLocationById(id));

        _mockRepository.Verify(r => r.GetLocationByIdAsync(id), Times.Once);
    }

    [Theory]
    [InlineData(1)]
    public async Task GetHourByIdValidId(int hourId)
    {
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        var specialisation = new Specialisation { Id = 1, Name = "Computer Science", Faculty = faculty };
        var promotion = new Promotion { Id = 1, StartYear = 2023, EndYear = 2027, Specialisation = specialisation };
        var semester = new PromotionSemester { Id = 1, Promotion = promotion, PromotionId = promotion.Id, SemesterNumber = 3 };
        var user = new User
        {
            Id = 1,
            FirstName = "Andrei",
            LastName = "Rotaru",
            Email = "andrei@gmail.com",
            TenantEmail = "at the crossroads",
            PhoneNumber = "+40777301089",
            Role = Enum.Parse<UserRole>("Student")
        };
        var teacher = new Teacher { Id = 1, User = user, Faculty = faculty };
        var subject = new Subject { Id = 1, Name = "FP", NumberOfCredits = 6, SubjectCode = "MLR1010", HolderTeacher = teacher, HolderTeacherId = teacher.Id, Semester = semester, SemesterId = semester.Id };
        var studentGroup = new StudentGroup { Id = 1, Name = "234", Promotion = promotion };
        var subGroup = new StudentSubGroup { Id = 1, Name = "234/1", StudentGroup = studentGroup };
        var location = new Location { Id = 1, Name = "Marasti", Address = "Groapa" };
        var classroom = new Classroom { Id = 1, Name = "A101", Location = location };

        var googleData = new GoogleMapsDataResponseDTO { Id = "1", Latitude = 2, Longitude = 3 };
        var locationResponse = new LocationResponseDTO
        { Id = 1, Name = location.Name, Address = location.Address, GoogleMapsData = googleData };
        var classroomResponse = new ClassroomResponseDTO { Id = 1, Name = classroom.Name, LocationId = locationResponse.Id };
        var subjectResponse = new SubjectResponseDTO
        {
            Id = 1,
            Name = subject.Name,
            NumberOfCredits = subject.NumberOfCredits,
            Code = subject.SubjectCode,
            Type = subject.Type.ToString(),
        };
        var userResponse = new UserResponseDTO
        {
            Id = teacher.User.Id,
            FirstName = teacher.User.FirstName,
            LastName = teacher.User.LastName,
            Email = teacher.User.Email,
            PhoneNumber = teacher.User.PhoneNumber,
            Role = teacher.User.Role,
            TenantEmail = "he is watching, always",
            Owner = ""
        };
        var teacherResponse = new TeacherResponseDTO
        { Id = teacher.Id, User = userResponse, UserId = userResponse.Id, FacultyId = faculty.Id };

        var response = new HourResponseDTO
        {
            Id = hourId,
            Day = "Monday",
            HourInterval = "08:00-10:00",
            Frequency = "Weekly",
            Category = "Lecture",
            Format = "231/1",
            Location = locationResponse,
            Classroom = classroomResponse,
            Subject = subjectResponse,
            Teacher = teacherResponse
        };

        _mockRepository.Setup(r => r.GetHourByIdAsync(hourId)).ReturnsAsync(response);


        var result = await _service.GetHourById(hourId);

        Assert.NotNull(result);
        Assert.Equal(hourId, result.Id);
        Assert.Equal("Monday", result.Day);
        _mockRepository.Verify(r => r.GetHourByIdAsync(hourId), Times.Once);
    }


    [Theory]
    [InlineData(999)]
    [InlineData(0)]
    public async Task GetHourByIdInvalidId(int hourId)
    {
        _mockRepository.Setup(r => r.GetHourByIdAsync(hourId)).ReturnsAsync((HourResponseDTO?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetHourById(hourId));

        _mockRepository.Verify(r => r.GetHourByIdAsync(hourId), Times.Once);
    }

    [Theory]
    [InlineData(1, 1, 1, 1, 1, 1, 1)]
    public async Task GetHourByFilterValidFilter(
        int userId, int classroomId, int teacherId, int subjectId, int facultyId, int specialisationId, int groupYearId)
    {
        var filter = new HourFilter
        {
            UserId = userId,
            ClassroomId = classroomId,
            TeacherId = teacherId,
            SubjectId = subjectId,
            FacultyId = facultyId,
            SpecialisationId = specialisationId,
            GroupYearId = groupYearId
        };
        var faculty = new FacultyResponseDTO { Id = 1, Name = "Facultate de Mate-Info" };
        var specialisation = new SpecialisationResponseDTO { Id = 1, Name = "Computer Science" };
        var promotion = new PromotionResponseDTO { Id = 1, StartYear = 2023, EndYear = 2027 };
        var subject = new SubjectResponseDTO
        {
            Id = 1,
            Name = "FP",
            NumberOfCredits = 6,
            Type = "Required",
            Code = "FP01",
        };
        var studentGroup = new StudentGroupResponseDTO { Id = 1, Name = "234" };
        var subGroup = new StudentSubGroupResponseDTO { Id = 1, Name = "234/1" };
        var googleData = new GoogleMapsDataResponseDTO { Id = "1", Latitude = 2, Longitude = 3 };
        var locationResponse = new LocationResponseDTO
        { Id = 1, Name = "Marasti", Address = "Groapa", GoogleMapsData = googleData };
        var classroom = new ClassroomResponseDTO { Id = 1, Name = "A101", LocationId = locationResponse.Id };
        var user = new UserResponseDTO
        {
            Id = 1,
            FirstName = "Andrei",
            LastName = "Rotaru",
            Email = "andrei@gmail.com",
            PhoneNumber = "+40777301089",
            TenantEmail = "i see you",
            Role = UserRole.Student,
            Owner = ""
        };
        var teacher = new TeacherResponseDTO { Id = 1, User = user, FacultyId = faculty.Id, UserId = userId };
        _mockAcademicRepository.Setup(r => r.GetEnrollmentsByUserId(userId))
            .ReturnsAsync(
                new List<EnrollmentResponseDTO>
                {
                    new EnrollmentResponseDTO
                    {
                        Id = 1,
                        UserId = userId,
                        SubGroupId = subGroup.Id,
                        User = user,
                        SubGroup = subGroup
                    }
                }
            );
        _mockRepository.Setup(r => r.GetClassroomByIdAsync(classroomId))
            .ReturnsAsync(classroom);
        _mockAcademicRepository.Setup(r => r.GetTeacherById(teacherId))
            .ReturnsAsync(teacher);
        _mockRepository.Setup(r => r.GetSubjectByIdAsync(subjectId))
            .ReturnsAsync(subject);
        _mockAcademicRepository.Setup(r => r.GetFacultyByIdAsync(facultyId))
            .ReturnsAsync(faculty);
        _mockAcademicRepository.Setup(r => r.GetSpecialisationByIdAsync(specialisationId))
            .ReturnsAsync(specialisation);
        _mockAcademicRepository.Setup(r => r.GetPromotionByIdAsync(groupYearId))
            .ReturnsAsync(promotion);


        var response = new HourResponseDTO
        {
            Id = 1,
            Day = "Monday",
            HourInterval = "08:00-10:00",
            Frequency = "Weekly",
            Category = "Lecture",
            Format = "231/1",
            Location = locationResponse,
            Classroom = classroom,
            Subject = subject,
            Teacher = teacher
        };

        _mockRepository.Setup(r => r.GetHoursAsync(filter)).ReturnsAsync(new List<HourResponseDTO> { response });

        var result = await _service.GetHourByFilter(filter);

        Assert.NotNull(result);
        Assert.Single(result.Hours);
        Assert.Equal("Monday", result.Hours.First().Day);

        _mockRepository.Verify(r => r.GetHoursAsync(filter), Times.Once);
    }


    [Theory]
    [InlineData(0, 0, 0, 0, 0, 0, 0)]
    [InlineData(-1, -1, -1, -1, -1, -1, -1)]
    public async Task GetHourByFilterInvalidFilter(
        int userId, int classroomId, int teacherId, int subjectId, int facultyId, int specialisationId, int groupYearId)
    {
        var filter = new HourFilter
        {
            UserId = userId,
            ClassroomId = classroomId,
            TeacherId = teacherId,
            SubjectId = subjectId,
            FacultyId = facultyId,
            SpecialisationId = specialisationId,
            GroupYearId = groupYearId
        };


        await Assert.ThrowsAsync<EntityValidationException>(() => _service.GetHourByFilter(filter));

        _mockRepository.Verify(r => r.GetHoursAsync(It.IsAny<HourFilter>()), Times.Never);
    }
}
