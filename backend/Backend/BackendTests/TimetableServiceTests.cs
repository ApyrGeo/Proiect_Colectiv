using AutoMapper;
using Backend.Domain;
using Backend.Domain.DTOs;
using Backend.Domain.Enums;
using Backend.Exceptions.Custom;
using Backend.Interfaces;
using Backend.Service;
using Backend.Service.Validators;
using Backend.Utils;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;
using IValidatorFactory = Backend.Interfaces.IValidatorFactory;

namespace BackendTests;

public class TimetableServiceTests
{
    private readonly Mock<ITimetableRepository> _mockRepository = new();
    private readonly Mock<IMapper> _mockMapper = new();
    private readonly IValidatorFactory _validatorFactory;
    private readonly Mock<IAcademicRepository> _mockAcademicRepository = new();

    private readonly TimetableService _service;
    
    public TimetableServiceTests()
    {
        var subjectValidator = new SubjectPostDTOValidator(_mockRepository.Object,_mockAcademicRepository.Object);
        var classroomValidator = new ClassroomPostDTOValidator(_mockRepository.Object);
        var hourValidator = new HourPostDTOValidator(_mockRepository.Object,_mockAcademicRepository.Object);
        var locationValidator = new LocationPostDTOValidator();
        
        var mockValidatorFactory = new Mock<IValidatorFactory>();
        mockValidatorFactory.Setup(v => v.Get<SubjectPostDTO>()).Returns(subjectValidator);
        mockValidatorFactory.Setup(v => v.Get<LocationPostDTO>()).Returns(locationValidator);
        mockValidatorFactory.Setup(v => v.Get<ClassroomPostDTO>()).Returns(classroomValidator);
        mockValidatorFactory.Setup(v => v.Get<HourPostDTO>()).Returns(hourValidator);
        _validatorFactory = mockValidatorFactory.Object;

        _service = new TimetableService(
            _mockRepository.Object,
            _mockAcademicRepository.Object,
            _mockMapper.Object,
            _validatorFactory
        );
    }
    
    [Theory]
    [InlineData("Analiza", 6, 1)]
    [InlineData("FP", 5, 2)]
    public async Task CreateSubjectValidData(string name, int credits, int groupYearId)
    {
        var postDto = new SubjectPostDTO { Name = name, NumberOfCredits = credits, GroupYearId = groupYearId };
        
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        
        var specialisation = new Specialisation { Id = 1, Name = "Computer Science", Faculty = faculty};
       
        var groupYear = new GroupYear { Id = groupYearId, Year = "IR1", Specialisation = specialisation};
        
        _mockAcademicRepository
            .Setup(r => r.GetGroupYearByIdAsync(groupYearId))
            .ReturnsAsync(groupYear);
        
        var subjectEntity = new Subject { Id = 1, Name = name, NumberOfCredits = credits, GroupYearId = groupYearId, GroupYear = groupYear };

        var responseDto = new SubjectResponseDTO { Id = 1, Name = name, NumberOfCredits = credits };
        

        _mockMapper.Setup(m => m.Map<Subject>(postDto)).Returns(subjectEntity);
        _mockMapper.Setup(m => m.Map<SubjectResponseDTO>(subjectEntity)).Returns(responseDto);

        _mockRepository.Setup(r => r.AddSubjectAsync(subjectEntity)).ReturnsAsync(subjectEntity);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        
        var result = await _service.CreateSubject(postDto);
        
        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
        Assert.Equal(credits, result.NumberOfCredits);

        _mockRepository.Verify(r => r.AddSubjectAsync(subjectEntity), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Theory]
    [InlineData("Analiza", 10, 1)]
    [InlineData("", 5, 2)]
    [InlineData("FP", 0, 3)]
    public async Task CreateSubjectInvalidData(string name, int credits, int groupYearId)
    {
        var postDto = new SubjectPostDTO { Name = name, NumberOfCredits = credits, GroupYearId = groupYearId };
        
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        
        var specialisation = new Specialisation { Id = 1, Name = "Computer Science", Faculty = faculty};
       
        var groupYear = new GroupYear { Id = groupYearId, Year = "IR1", Specialisation = specialisation};
        
        _mockAcademicRepository
            .Setup(r => r.GetGroupYearByIdAsync(groupYearId))
            .ReturnsAsync(groupYear);
        
        var subjectEntity = new Subject { Id = 1, Name = name, NumberOfCredits = credits, GroupYearId = groupYearId, GroupYear = groupYear };

        var responseDto = new SubjectResponseDTO { Id = 1, Name = name, NumberOfCredits = credits };
        
        
        await Assert.ThrowsAsync<EntityValidationException>(() => _service.CreateSubject(postDto));
        
        _mockRepository.Verify(r => r.AddSubjectAsync(subjectEntity), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
    
    [Theory]
    [InlineData(1, "Analiza", 6)]
    [InlineData(2, "FP", 5)]
    public async Task GetSubjectByIdValidId(int id, string name, int credits)
    {
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        var specialisation = new Specialisation { Id = 1, Name = "Computer Science", Faculty = faculty };
        var groupYear = new GroupYear { Id = 1, Year = "IR1", Specialisation = specialisation };
        var subjectEntity = new Subject { Id = id, Name = name, NumberOfCredits = credits, GroupYear = groupYear, GroupYearId = groupYear.Id };
        var responseDto = new SubjectResponseDTO { Id = id, Name = name, NumberOfCredits = credits };

        _mockRepository.Setup(r => r.GetSubjectByIdAsync(id))
            .ReturnsAsync(subjectEntity);

        _mockMapper.Setup(m => m.Map<SubjectResponseDTO>(subjectEntity))
            .Returns(responseDto);
        
        var result = await _service.GetSubjectById(id);
        
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(name, result.Name);
        Assert.Equal(credits, result.NumberOfCredits);

        _mockRepository.Verify(r => r.GetSubjectByIdAsync(id), Times.Once);
        _mockMapper.Verify(m => m.Map<SubjectResponseDTO>(subjectEntity), Times.Once);
    }
    
    [Theory]
    [InlineData(999)]
    [InlineData(0)]
    public async Task GetSubjectByIdInvalidId(int invalidId)
    {
        _mockRepository.Setup(r => r.GetSubjectByIdAsync(invalidId))
            .ReturnsAsync((Subject?)null);
        
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetSubjectById(invalidId));

        _mockRepository.Verify(r => r.GetSubjectByIdAsync(invalidId), Times.Once);
        _mockMapper.Verify(m => m.Map<SubjectResponseDTO>(It.IsAny<Subject>()), Times.Never);
    }
    
    [Theory]
    [InlineData("Fsega", "Str. Vasile Goldis")]
    [InlineData("Centru", "Str. Universitatii")]
    public async Task CreateLocationValidData(string name, string address)
    {
        var dto = new LocationPostDTO { Name = name, Address = address };
        var entity = new Location { Id = 1, Name = name, Address = address };
        var response = new LocationResponseDTO { Id = 1, Name = name, Address = address };

        _mockMapper.Setup(m => m.Map<Location>(dto)).Returns(entity);
        _mockRepository.Setup(r => r.AddLocationAsync(entity)).ReturnsAsync(entity);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<LocationResponseDTO>(entity)).Returns(response);

        var result = await _service.CreateLocation(dto);

        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
        Assert.Equal(address, result.Address);
        _mockRepository.Verify(r => r.AddLocationAsync(entity), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
    
    [Theory]
    [InlineData("", "Str. Vasile Goldis")]
    [InlineData("Centru", "")]
    public async Task CreateLocationInvalidData(string name, string address)
    {
        var dto = new LocationPostDTO { Name = name, Address = address };

        await Assert.ThrowsAsync<EntityValidationException>(() => _service.CreateLocation(dto));

        _mockRepository.Verify(r => r.AddLocationAsync(It.IsAny<Location>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
    
    [Theory]
    [InlineData("L002", 1)]
    [InlineData("A2", 2)]
    public async Task CreateClassroomValidData(string name, int locationId)
    {
        var location = new Location { Id = locationId, Name = "Centru", Address = "Str. Universitatii" };
        var dto = new ClassroomPostDTO { Name = name, LocationId = locationId };
        var entity = new Classroom { Id = 1, Name = name, LocationId = locationId, Location = location };
        var response = new ClassroomResponseDTO { Id = 1, Name = name };

        _mockRepository.Setup(r => r.GetLocationByIdAsync(locationId)).ReturnsAsync(location);
        _mockMapper.Setup(m => m.Map<Classroom>(dto)).Returns(entity);
        _mockRepository.Setup(r => r.AddClassroomAsync(entity)).ReturnsAsync(entity);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<ClassroomResponseDTO>(entity)).Returns(response);

        var result = await _service.CreateClassroom(dto);

        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
        _mockRepository.Verify(r => r.AddClassroomAsync(entity), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
    
    [Theory]
    [InlineData("", 1)]
    [InlineData("Room 305", 0)]
    [InlineData("Room 305", 99)]
    public async Task CreateClassroomInvalidData(string name, int locationId)
    {
        var dto = new ClassroomPostDTO { Name = name, LocationId = locationId };

        _mockRepository.Setup(r => r.GetLocationByIdAsync(locationId)).ReturnsAsync((Location)null);

        await Assert.ThrowsAsync<EntityValidationException>(() => _service.CreateClassroom(dto));

        _mockRepository.Verify(r => r.AddClassroomAsync(It.IsAny<Classroom>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory]
    [InlineData("Monday", "08:00-10:00", "Weekly", "Lecture", "IR1")]
    [InlineData("Friday", "18:00-20:00", "SecondWeek", "Seminar", "231")]
    public async Task CreateHourValidData(
        string day, string hourInterval, string frequency, string category, string format)
    {
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        var specialisation = new Specialisation { Id = 1, Name = "Computer Science", Faculty = faculty };
        var groupYear = new GroupYear { Id = 1, Year = "IR1", Specialisation = specialisation };
        _mockAcademicRepository
            .Setup(r => r.GetGroupYearByIdAsync(groupYear.Id))
            .ReturnsAsync(groupYear);
        var subject = new Subject { Id = 1, Name = "FP", NumberOfCredits = 6, GroupYearId = 1, GroupYear = groupYear };
        var studentGroup = new StudentGroup { Id = 1, Name = "234", GroupYear = groupYear };
        _mockAcademicRepository
            .Setup(r => r.GetGroupByIdAsync(studentGroup.Id))
            .ReturnsAsync(studentGroup);
        var subGroup = new StudentSubGroup { Id = 1, Name = "234/1", StudentGroup = studentGroup };
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
        var classroom = new Classroom { Id = 1, Name = "A101", Location = location };
        _mockRepository
            .Setup(r => r.GetClassroomByIdAsync(classroom.Id))
            .ReturnsAsync(classroom);
        var user = new User
        {
            Id = 1, FirstName = "Andrei", LastName = "Rotaru", Email = "andrei@gmail.com", Password = "111222ppa",
            PhoneNumber = "+40777301089", Role = Enum.Parse<UserRole>("Student")
        };
        var teacher = new Teacher { Id = 1, User = user, Faculty = faculty };
        _mockAcademicRepository
            .Setup(r => r.GetTeacherById(teacher.Id))
            .ReturnsAsync(teacher);
        
        var entity = new Hour
        {
            Id = 1,
            Day = Enum.Parse<HourDay>(dto.Day),
            HourInterval = dto.HourInterval,
            Frequency = Enum.Parse<HourFrequency>(dto.Frequency),
            Category = Enum.Parse<HourCategory>(dto.Category),
            Subject = subject,
            Classroom = classroom,
            Teacher = teacher,
            SubjectId = 1,
            ClassroomId = 1,
            TeacherId = 1
        };

        var locationResponse = new LocationResponseDTO { Id = 1, Name = location.Name, Address = location.Address };
        var classroomResponse = new ClassroomResponseDTO { Id = 1, Name = classroom.Name };
        var subjectResponse = new SubjectResponseDTO { Id = 1, Name = subject.Name, NumberOfCredits = subject.NumberOfCredits };
        var userResponse = new UserResponseDTO { Id = teacher.User.Id, FirstName = teacher.User.FirstName,LastName = teacher.User.LastName,
        Email = teacher.User.Email, PhoneNumber = teacher.User.PhoneNumber, Role = teacher.User.Role.ToString() ,Password = teacher.User.Password };
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

        _mockMapper.Setup(m => m.Map<Hour>(dto)).Returns(entity);
        _mockRepository.Setup(r => r.AddHourAsync(entity)).ReturnsAsync(entity);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<HourResponseDTO>(entity)).Returns(response);
        
        var result = await _service.CreateHour(dto);
        
        Assert.NotNull(result);
        Assert.Equal(dto.HourInterval, result.HourInterval);
        _mockRepository.Verify(r => r.AddHourAsync(entity), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
    
    [Theory]
    [InlineData("Monday", "", "Weekly", "Lecture")]
    [InlineData("Monday", "09:00-11:00", "", "Lecture")]
    [InlineData("Monday", "09:00-11:00", "Weekly", "")]
    [InlineData("", "09:00-11:00", "Weekly", "Lecture")]
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

        _mockRepository.Verify(r => r.AddHourAsync(It.IsAny<Hour>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
    
    [Theory]
    [InlineData(1, "L001", "Marasti", "Groapa")]
    public async Task GetClassroomByIdValidId(int id, string name, string locName, string address)
    {
        var location = new Location { Id = 1, Name = locName, Address = address };
        var classroom = new Classroom { Id = id, Name = name, Location = location };
        var response = new ClassroomResponseDTO { Id = id, Name = name };

        _mockRepository.Setup(r => r.GetClassroomByIdAsync(id)).ReturnsAsync(classroom);
        _mockMapper.Setup(m => m.Map<ClassroomResponseDTO>(classroom)).Returns(response);

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
        _mockRepository.Setup(r => r.GetClassroomByIdAsync(id)).ReturnsAsync((Classroom?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetClassroomById(id));

        _mockRepository.Verify(r => r.GetClassroomByIdAsync(id), Times.Once);
    }
    
    [Theory]
    [InlineData(1, "Marasti", "Groapa")]
    public async Task GetLocationByIdValidId(int id, string name, string address)
    {
        var entity = new Location { Id = id, Name = name, Address = address };
        var response = new LocationResponseDTO { Id = id, Name = name, Address = address };

        _mockRepository.Setup(r => r.GetLocationByIdAsync(id)).ReturnsAsync(entity);
        _mockMapper.Setup(m => m.Map<LocationResponseDTO>(entity)).Returns(response);

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
        _mockRepository.Setup(r => r.GetLocationByIdAsync(id)).ReturnsAsync((Location?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetLocationById(id));

        _mockRepository.Verify(r => r.GetLocationByIdAsync(id), Times.Once);
    }
    
    [Theory]
    [InlineData(1)]
    public async Task GetHourByIdValidId(int hourId)
    {
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        var specialisation = new Specialisation { Id = 1, Name = "Computer Science", Faculty = faculty };
        var groupYear = new GroupYear { Id = 1, Year = "IR1", Specialisation = specialisation };
        var subject = new Subject { Id = 1, Name = "FP", NumberOfCredits = 6, GroupYearId = 1, GroupYear = groupYear };
        var studentGroup = new StudentGroup { Id = 1, Name = "234", GroupYear = groupYear };
        var subGroup = new StudentSubGroup { Id = 1, Name = "234/1", StudentGroup = studentGroup };
        var location = new Location { Id = 1, Name = "Marasti", Address = "Groapa" };
        var classroom = new Classroom { Id = 1, Name = "A101", Location = location };
        var user = new User
        {
            Id = 1, FirstName = "Andrei", LastName = "Rotaru", Email = "andrei@gmail.com", Password = "111222ppa",
            PhoneNumber = "+40777301089", Role = Enum.Parse<UserRole>("Student")
        };
        var teacher = new Teacher { Id = 1, User = user, Faculty = faculty };
        var entity = new Hour
        {
            Id = hourId,
            Day = HourDay.Monday,
            HourInterval = "08:00-10:00",
            Frequency = HourFrequency.Weekly,
            Category = HourCategory.Lecture,
            Subject = subject,
            Classroom = classroom,
            Teacher = teacher
        };
        
        var locationResponse = new LocationResponseDTO { Id = 1, Name = location.Name, Address = location.Address };
        var classroomResponse = new ClassroomResponseDTO { Id = 1, Name = classroom.Name };
        var subjectResponse = new SubjectResponseDTO { Id = 1, Name = subject.Name, NumberOfCredits = subject.NumberOfCredits };
        var userResponse = new UserResponseDTO { Id = teacher.User.Id, FirstName = teacher.User.FirstName,LastName = teacher.User.LastName,
            Email = teacher.User.Email, PhoneNumber = teacher.User.PhoneNumber, Role = teacher.User.Role.ToString() ,Password = teacher.User.Password };
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

        _mockRepository.Setup(r => r.GetHourByIdAsync(hourId)).ReturnsAsync(entity);
        _mockMapper.Setup(m => m.Map<HourResponseDTO>(entity)).Returns(response);

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
        _mockRepository.Setup(r => r.GetHourByIdAsync(hourId)).ReturnsAsync((Hour?)null);

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
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        var specialisation = new Specialisation { Id = 1, Name = "Computer Science", Faculty = faculty };
        var groupYear = new GroupYear { Id = 1, Year = "IR1", Specialisation = specialisation };
        var subject = new Subject { Id = 1, Name = "FP", NumberOfCredits = 6, GroupYearId = 1, GroupYear = groupYear };
        var studentGroup = new StudentGroup { Id = 1, Name = "234", GroupYear = groupYear };
        var subGroup = new StudentSubGroup { Id = 1, Name = "234/1", StudentGroup = studentGroup };
        var location = new Location { Id = 1, Name = "Marasti", Address = "Groapa" };
        var classroom = new Classroom { Id = 1, Name = "A101", Location = location };
        var user = new User
        {
            Id = 1, FirstName = "Andrei", LastName = "Rotaru", Email = "andrei@gmail.com", Password = "111222ppa",
            PhoneNumber = "+40777301089", Role = Enum.Parse<UserRole>("Student")
        };
        var teacher = new Teacher { Id = 1, User = user, Faculty = faculty };
        _mockAcademicRepository.Setup(r => r.GetEnrollmentsByUserId(userId))
            .ReturnsAsync(new List<Enrollment> { new Enrollment{User=user ,SubGroup = subGroup }});
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
        _mockAcademicRepository.Setup(r => r.GetGroupYearByIdAsync(groupYearId))
            .ReturnsAsync(groupYear);

        var hours = new List<Hour>
        {
            new Hour
            {
                Id = 1,
                Day = HourDay.Monday,
                HourInterval = "08:00-10:00",
                Frequency = HourFrequency.Weekly,
                Category = HourCategory.Lecture,
                Subject = subject,
                Classroom = classroom,
                Teacher = teacher
            }
        };
        var locationResponse = new LocationResponseDTO { Id = 1, Name = location.Name, Address = location.Address };
        var classroomResponse = new ClassroomResponseDTO { Id = 1, Name = classroom.Name };
        var subjectResponse = new SubjectResponseDTO { Id = 1, Name = subject.Name, NumberOfCredits = subject.NumberOfCredits };
        var userResponse = new UserResponseDTO { Id = teacher.User.Id, FirstName = teacher.User.FirstName,LastName = teacher.User.LastName,
            Email = teacher.User.Email, PhoneNumber = teacher.User.PhoneNumber, Role = teacher.User.Role.ToString() ,Password = teacher.User.Password };
        var teacherResponse = new TeacherResponseDTO
            { Id = teacher.Id, User = userResponse, UserId = userResponse.Id, FacultyId = faculty.Id };

        var response = new HourResponseDTO
        {
            Id = 1,
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

        _mockRepository.Setup(r => r.GetHoursAsync(filter)).ReturnsAsync(hours);
        _mockMapper.Setup(m => m.Map<List<HourResponseDTO>>(hours)).Returns(new List<HourResponseDTO> { response });

        var result = await _service.GetHourByFilter(filter);

        Assert.NotNull(result);
        Assert.Single(result.Hours);
        Assert.Equal("Monday", result.Hours.First().Day);

        _mockRepository.Verify(r => r.GetHoursAsync(filter), Times.Once);
    }
    
    [Theory]
    [InlineData(5)]
    public async Task GetHourByFilterClassroomNotFound(int classroomId)
    {
        var filter = new HourFilter { ClassroomId = classroomId };

        _mockRepository.Setup(r => r.GetClassroomByIdAsync(classroomId))
            .ReturnsAsync((Classroom?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetHourByFilter(filter));

        _mockRepository.Verify(r => r.GetClassroomByIdAsync(classroomId), Times.Once);
    }
    
    [Theory]
    [InlineData(3)]
    public async Task GetHourByFilterTeacherNotFound(int teacherId)
    {
        var filter = new HourFilter { TeacherId = teacherId };

        _mockAcademicRepository.Setup(r => r.GetTeacherById(teacherId))
            .ReturnsAsync((Teacher?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetHourByFilter(filter));

        _mockAcademicRepository.Verify(r => r.GetTeacherById(teacherId), Times.Once);
    }
    
    [Theory]
    [InlineData(10)]
    public async Task GetHourByFilterUserHasNoEnrollments(int userId)
    {
        var filter = new HourFilter { UserId = userId };

        _mockAcademicRepository.Setup(r => r.GetEnrollmentsByUserId(userId))
            .ReturnsAsync(new List<Enrollment>());

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetHourByFilter(filter));

        _mockAcademicRepository.Verify(r => r.GetEnrollmentsByUserId(userId), Times.Once);
    }
}