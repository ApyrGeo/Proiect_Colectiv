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

public class AcademicRepositoryTests : IDisposable
{
    private readonly AcademicAppContext _context;
    private readonly AcademicRepository _repo;

    public AcademicRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AcademicAppContext>()
            .UseInMemoryDatabase(databaseName: "AcademicRepositoryTestsDB")
            .Options;
        var config = new MapperConfiguration(cfg => { cfg.AddProfile<EFEntitiesMappingProfile>(); },
            new NullLoggerFactory());

        IMapper mapper = config.CreateMapper();
        _context = new AcademicAppContext(options);
        _repo = new AcademicRepository(_context, mapper);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Theory]
    [InlineData("Facultatea de Drept")]
    [InlineData("Facultatea de Geografie")]
    public async Task AddFacultyAsyncTest(string name)
    {
        var faculty = new FacultyPostDTO { Name = name };
        await _repo.AddFacultyAsync(faculty);
        await _repo.SaveChangesAsync();

        var result = await _context.Faculties.FirstOrDefaultAsync(f => f.Name == name);
        Assert.NotNull(result);
        Assert.Equal(name, result!.Name);
    }


    [Theory]
    [InlineData("Computer-Science")]
    [InlineData("Inginerie")]
    public async Task AddSpecialisationAsyncTest(string name)
    {
        var faculty = new Faculty { Name = "FMI" };
        await _context.Faculties.AddAsync(faculty);
        await _context.SaveChangesAsync();

        var spec = new SpecialisationPostDTO { Name = name, FacultyId = faculty.Id };
        await _repo.AddSpecialisationAsync(spec);
        await _repo.SaveChangesAsync();

        var result = await _context.Specialisations.FirstOrDefaultAsync(s => s.Name == name);
        Assert.NotNull(result);
        Assert.Equal(name, result!.Name);
    }

    [Theory]
    [InlineData(2023, 2025)]
    [InlineData(2024, 2027)]
    public async Task AddPromotionAsyncTest(int sy, int ey)
    {
        var faculty = new Faculty { Name = "FMI" };
        var spec = new Specialisation { Name = "Informatica", Faculty = faculty };
        await _context.Specialisations.AddAsync(spec);
        await _context.SaveChangesAsync();

        var promotion = new PromotionPostDTO { StartYear = sy, EndYear = ey, SpecialisationId = spec.Id };
        await _repo.AddPromotionAsync(promotion);
        await _repo.SaveChangesAsync();

        var result = await _context.Promotions.FirstOrDefaultAsync(g => g.StartYear == sy);
        Assert.NotNull(result);
        Assert.Equal(sy, result!.StartYear);
    }


    [Theory]
    [InlineData("236")]
    [InlineData("833")]
    public async Task AddGroupAsyncTest(string name)
    {
        var faculty = new Faculty { Name = "FMI" };
        var spec = new Specialisation { Name = "Informatica", Faculty = faculty };
        var promotion = new Promotion { StartYear = 2023, EndYear = 2025, Specialisation = spec };

        await _context.Promotions.AddAsync(promotion);
        await _context.SaveChangesAsync();

        var group = new StudentGroupPostDTO { Name = name, GroupYearId = promotion.Id };
        await _repo.AddGroupAsync(group);
        await _repo.SaveChangesAsync();

        var result = await _context.Groups.FirstOrDefaultAsync(g => g.Name == name);
        Assert.NotNull(result);
        Assert.Equal(name, result!.Name);
    }

    [Theory]
    [InlineData("235/1")]
    [InlineData("237/2")]
    public async Task AddSubGroupAsyncTest(string name)
    {
        var faculty = new Faculty { Name = "FMI" };
        var spec = new Specialisation { Name = "Informatica", Faculty = faculty };
        var promotion = new Promotion { StartYear = 2023, EndYear = 2025, Specialisation = spec };
        var group = new StudentGroup { Name = "IR1A", Promotion = promotion };

        await _context.Groups.AddAsync(group);
        await _context.SaveChangesAsync();

        var subGroup = new StudentSubGroupPostDTO { Name = name, StudentGroupId = group.Id };
        await _repo.AddSubGroupAsync(subGroup);
        await _repo.SaveChangesAsync();

        var result = await _context.SubGroups.FirstOrDefaultAsync(sg => sg.Name == name);
        Assert.NotNull(result);
        Assert.Equal(name, result!.Name);
    }

    [Theory]
    [InlineData("Istvan", "Csibula")]
    [InlineData("Dan", "Suciu")]
    public async Task AddTeacherAsyncTest(string firstName, string lastName)
    {
        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = $"{firstName}@mail.com",
            PhoneNumber = "+40988301069",
            Role = UserRole.Admin,
            Password = "1234567"
        };
        var faculty = new Faculty { Name = "FMI" };

        await _context.Users.AddAsync(user);
        await _context.Faculties.AddAsync(faculty);
        await _context.SaveChangesAsync();

        var teacher = new TeacherPostDTO { UserId = user.Id, FacultyId = faculty.Id };
        await _repo.AddTeacherAsync(teacher);
        await _repo.SaveChangesAsync();

        var result = await _context.Teachers.Include(t => t.User).FirstOrDefaultAsync(t => t.UserId == user.Id);
        Assert.NotNull(result);
        Assert.Equal(firstName, result!.User.FirstName);
    }

    [Fact]
    public async Task GetFacultyByIdAsyncExistingId()
    {
        var faculty = new Faculty { Name = "FSEGA" };
        await _context.Faculties.AddAsync(faculty);
        await _context.SaveChangesAsync();

        var result = await _repo.GetFacultyByIdAsync(faculty.Id);

        Assert.NotNull(result);
        Assert.Equal("FSEGA", result!.Name);
    }

    [Theory]
    [InlineData("Istvan", "Csibula")]
    [InlineData("Dan", "Suciu")]
    public async Task GetTeacherByIdTest(string firstName, string lastName)
    {
        var faculty = new Faculty { Name = "FMI" };
        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = $"{firstName}@mail.com",
            PhoneNumber = "+40988301069",
            Role = UserRole.Admin,
            Password = "1234567"
        };
        var teacher = new Teacher { User = user, Faculty = faculty };

        await _context.Teachers.AddAsync(teacher);
        await _context.SaveChangesAsync();

        var result = await _repo.GetTeacherById(teacher.Id);

        Assert.NotNull(result);
        Assert.Equal(teacher.User.FirstName, result!.User.FirstName);
    }

    [Theory]
    [InlineData("Computer-Science")]
    [InlineData("Inginerie")]
    public async Task GetSpecialisationByIdAsyncTest(string name)
    {
        var faculty = new Faculty { Name = "FMI" };
        var spec = new Specialisation { Name = name, Faculty = faculty };
        await _context.Specialisations.AddAsync(spec);
        await _context.SaveChangesAsync();

        var result = await _repo.GetSpecialisationByIdAsync(spec.Id);

        Assert.NotNull(result);
        Assert.Equal(spec.Name, result!.Name);
        Assert.NotNull(result.Name);
    }

    [Theory]
    [InlineData(2023)]
    [InlineData(2024)]
    public async Task GetPromotionByIdAsyncTest(int year)
    {
        var faculty = new Faculty { Name = "FMI" };
        var specialisation = new Specialisation { Name = "Informatica", Faculty = faculty };
        var promotion = new Promotion { StartYear = year, EndYear = year + 3, Specialisation = specialisation };

        _context.Promotions.Add(promotion);
        await _context.SaveChangesAsync();

        var result = await _repo.GetPromotionByIdAsync(promotion.Id);

        Assert.NotNull(result);
        Assert.Equal(year, result.StartYear);
    }

    [Theory]
    [InlineData("IR1", "312", "FMI")]
    public async Task GetGroupByIdAsyncTest(string year, string groupName, string facultyName)
    {
        var faculty = new Faculty { Name = facultyName };
        var specialisation = new Specialisation { Name = "Informatica", Faculty = faculty };
        var promotion = new Promotion { StartYear = 2023, EndYear = 2025, Specialisation = specialisation };
        var group = new StudentGroup { Name = groupName, Promotion = promotion };

        _context.Groups.Add(group);
        await _context.SaveChangesAsync();

        var result = await _repo.GetGroupByIdAsync(group.Id);

        Assert.NotNull(result);
        Assert.Equal(groupName, result.Name);
    }

    [Theory]
    [InlineData(2023, 2024, "312", "312/2", "FMI")]
    public async Task GetSubGroupByIdAsync_ReturnsCorrectSubGroup(int startYear, int endYear, string groupName, string subGroupName,
        string facultyName)
    {
        var faculty = new Faculty { Name = facultyName };
        var specialisation = new Specialisation { Name = "Informatica", Faculty = faculty };
        var promotion = new Promotion { StartYear= startYear, EndYear = endYear, Specialisation = specialisation };
        var group = new StudentGroup { Name = groupName, Promotion = promotion };
        var subGroup = new StudentSubGroup { Name = subGroupName, StudentGroup = group };

        _context.SubGroups.Add(subGroup);
        await _context.SaveChangesAsync();

        var result = await _repo.GetSubGroupByIdAsync(subGroup.Id);

        Assert.NotNull(result);
        Assert.Equal(subGroupName, result.Name);
    }

    [Theory]
    [InlineData(999)]
    public async Task GetFacultyByIdAsyncNonExisting(int invalidId)
    {
        var result = await _repo.GetFacultyByIdAsync(invalidId);

        Assert.Null(result);
    }

    [Theory]
    [InlineData(999)]
    public async Task GetSpecialisationIdAsyncNonExisting(int invalidId)
    {
        var result = await _repo.GetSpecialisationByIdAsync(invalidId);

        Assert.Null(result);
    }

    [Theory]
    [InlineData(999)]
    public async Task GetPromotionByIdAsyncNonExisting(int invalidId)
    {
        var result = await _repo.GetPromotionByIdAsync(invalidId);

        Assert.Null(result);
    }

    [Theory]
    [InlineData(999)]
    public async Task GetGroupByIdAsyncNonExisting(int invalidId)
    {
        var result = await _repo.GetGroupByIdAsync(invalidId);

        Assert.Null(result);
    }

    [Theory]
    [InlineData(999)]
    public async Task GetSubGroupByIdAsyncNonExisting(int invalidId)
    {
        var result = await _repo.GetSubGroupByIdAsync(invalidId);

        Assert.Null(result);
    }

    [Theory]
    [InlineData(999)]
    public async Task GetTeacherAsyncNonExisting(int invalidId)
    {
        var result = await _repo.GetTeacherById(invalidId);

        Assert.Null(result);
    }
}