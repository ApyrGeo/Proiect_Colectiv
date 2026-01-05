using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Enums;
using TrackForUBB.Repository;
using TrackForUBB.Repository.AutoMapper;
using TrackForUBB.Repository.Context;
using TrackForUBB.Repository.EFEntities;
using Xunit;

namespace TrackForUBB.BackendTests;

public class GradeRepositoryTests : IDisposable
{
    private readonly AcademicAppContext _context;
    private readonly GradeRepository _repo;

    public GradeRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AcademicAppContext>()
            .UseInMemoryDatabase(databaseName: "GradeRepositoryTestsDB")
            .Options;
        var config = new MapperConfiguration(cfg => { cfg.AddProfile<EFEntitiesMappingProfile>(); },
            new NullLoggerFactory());

        IMapper mapper = config.CreateMapper();
        _context = new AcademicAppContext(options);
        _repo = new GradeRepository(_context, mapper);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Theory]
    [InlineData(1, 1, 1, 9.5)]
    [InlineData(2, 1, 1, 8.0)]
    public async Task AddGradeAsyncTest(int enrollmentId, int subjectId, int semesterId, int value)
    {
        var faculty = new Faculty { Name = "FMI" };
        var spec = new Specialisation { Name = "Informatica", Faculty = faculty };
        var promotion = new Promotion { StartYear = 2023, EndYear = 2025, Specialisation = spec };
        var group = new StudentGroup { Name = "IR1A", Promotion = promotion };
        var subGroup = new StudentSubGroup { Name = "235/1", StudentGroup = group };
        var user = new User
        {
            FirstName = "andrei",
            LastName = "rotaru",
            Email = "andrei@mail.com",
            PhoneNumber = "+40988301069",
            Role = UserRole.Admin
        };
        var enrollment = new Enrollment
        { Id = enrollmentId, UserId = user.Id, SubGroupId = subGroup.Id, User = user, SubGroup = subGroup };

        var teacherUser = new User
        {
            FirstName = "Radio",
            LastName = "Șan",
            Email = "radiosan@yahoo.com",
            PhoneNumber = "+09812987409",
            Role = UserRole.Teacher,
        };
        var teacher = new Teacher
        {
            Faculty = faculty,
            FacultyId = faculty.Id,
            User = teacherUser,
            UserId = teacherUser.Id,
            Id = 19,
        };


        var gradeDto = new GradePostDTO
        {
            SemesterId = semesterId,
            EnrollmentId = enrollmentId,
            SubjectId = subjectId,
            Value = value
        };
        var semester = new PromotionSemester
        { Id = semesterId, SemesterNumber = 1, PromotionId = promotion.Id, Promotion = promotion };
        var subject = new Subject
        {
            Id = subjectId,
            Name = "TestSubject",
            NumberOfCredits = 5,
            Semester = semester,
            SemesterId = semester.Id,
            Type = SubjectType.Required,
            SubjectCode = "MLR101",
            HolderTeacher = teacher,
            HolderTeacherId = teacher.Id,
        };

        await _context.Users.AddAsync(user);
        await _context.Users.AddAsync(teacherUser);
        await _context.Teachers.AddAsync(teacher);
        await _context.Groups.AddAsync(group);
        await _context.SubGroups.AddAsync(subGroup);
        await _context.Enrollments.AddAsync(enrollment);
        await _context.Subjects.AddAsync(subject);
        await _context.PromotionSemesters.AddAsync(semester);
        var result = await _repo.AddGradeAsync(gradeDto);
        await _repo.SaveChangesAsync();


        var dbGrade =
            await _context.Grades.FirstOrDefaultAsync(g => g.EnrollmentId == enrollmentId && g.SubjectId == subjectId);
        Assert.NotNull(dbGrade);
        Assert.Equal(value, dbGrade!.Value);
        Assert.Equal(enrollmentId, dbGrade.EnrollmentId);
        Assert.Equal(subjectId, dbGrade.SubjectId);

        Assert.NotNull(result);
        Assert.Equal(value, result.Value);
    }

    [Theory]
    [InlineData(1, 1, null, "Informatica")]
    [InlineData(2, null, 1, null)]
    public async Task GetGradesFilteredAsyncTest(int userId, int? yearOfStudy, int? semester, string specialisation)
    {
        var faculty = new Faculty { Id = 1, Name = "FMI" };
        var spec = new Specialisation { Id = 1, Name = "Informatica", Faculty = faculty };
        var promotion = new Promotion { Id = 1, StartYear = 2023, EndYear = 2025, Specialisation = spec };
        var group = new StudentGroup { Id = 1, Name = "IR1A", Promotion = promotion };
        var subGroup = new StudentSubGroup { Id = 1, Name = "235/1", StudentGroup = group };
        var user = new User
        {
            FirstName = "andrei",
            LastName = "rotaru",
            Email = "andrei@mail.com",
            PhoneNumber = "+40988301069",
            Role = UserRole.Admin
        };
        var enrollment = new Enrollment
        { Id = 1, UserId = user.Id, SubGroupId = subGroup.Id, User = user, SubGroup = subGroup };
        var teacherUser = new User
        {
            FirstName = "radio",
            LastName = "san",
            Email = "radiosan@ubbcluj.ro",
            PhoneNumber = "-0912098409",
            Role = UserRole.Teacher,
        };
        var teacher = new Teacher
        {
            Id = 12,
            User = teacherUser,
            UserId = teacherUser.Id,
            Faculty = faculty,
            FacultyId = faculty.Id,
        };
        var semesterP = new PromotionSemester
        { Id = 1, SemesterNumber = 1, PromotionId = promotion.Id, Promotion = promotion };
        var subject = new Subject
        {
            Id = 1,
            Name = "Matematica",
            NumberOfCredits = 5,
            HolderTeacher = teacher,
            HolderTeacherId = teacher.Id,
            Semester = semesterP,
            SemesterId = semesterP.Id,
            SubjectCode = "MLR1010",
        };

        var grade = new Grade
        {
            Id = 1,
            Value = 10,
            Enrollment = enrollment,
            EnrollmentId = enrollment.Id,
            Subject = subject,
            SubjectId = subject.Id
        };

        await _context.Grades.AddAsync(grade);
        await _context.SaveChangesAsync();


        var result = await _repo.GetGradesFilteredAsync(userId, yearOfStudy, semester, specialisation);

        Assert.NotNull(result);
        Assert.All(result, g => Assert.Equal(userId, g.Enrollment.UserId));
    }

    [Theory]
    [InlineData(1, 1)]
    public async Task GetGradesForStudentInSemesterAsyncTest(int enrollmentId, int semesterId)
    {
        var faculty = new Faculty { Name = "FMI" };
        var spec = new Specialisation { Name = "Informatica", Faculty = faculty };
        var promotion = new Promotion { StartYear = 2023, EndYear = 2025, Specialisation = spec };
        var group = new StudentGroup { Name = "IR1A", Promotion = promotion };
        var subGroup = new StudentSubGroup { Name = "235/1", StudentGroup = group };
        var semester = new PromotionSemester
        {
            Id = semesterId,
            SemesterNumber = 1,
            PromotionId = promotion.Id,
            Promotion = promotion
        };
        var subject = new Subject
        {
            Id = 1,
            Name = "TestSubject",
            NumberOfCredits = 5,
            HolderTeacherId = 1,
            HolderTeacher = new()
            {
                Id = 1,
                Faculty = faculty,
                FacultyId = faculty.Id,
                UserId = 2,
                User = new()
                {
                    Id = 2,
                    FirstName = "radio",
                    LastName = "san",
                    Email = "radiosan@yahoo.com",
                    Role = UserRole.Teacher,
                    PhoneNumber = "+0981208743",
                }
            },
            Semester = semester,
            SemesterId = semester.Id,
            SubjectCode = "MLR1010",
        };
        var user = new User
        {
            FirstName = "andrei",
            LastName = "rotaru",
            Email = "andrei@mail.com",
            PhoneNumber = "+40988301069",
            Role = UserRole.Admin
        };
        var enrollment = new Enrollment
        { Id = enrollmentId, UserId = user.Id, SubGroupId = subGroup.Id, User = user, SubGroup = subGroup };

        var grade = new Grade
        {
            Id = 1,
            Value = 9,
            Enrollment = enrollment,
            EnrollmentId = enrollment.Id,
            Subject = subject,
            SubjectId = subject.Id
        };

        await _context.Grades.AddAsync(grade);
        await _context.SaveChangesAsync();

        var result = await _repo.GetGradesForStudentInSemesterAsync(enrollmentId, semesterId);

        Assert.Single(result);
        Assert.Equal(9, result.First().Value);
    }
}
