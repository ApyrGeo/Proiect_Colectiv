using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TrackForUBB.Domain.Enums;
using TrackForUBB.Repository;
using TrackForUBB.Repository.Context;
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

        _context = new AcademicAppContext(options);
        _repo = new AcademicRepository(_context);

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
        var faculty = new Faculty { Name = name };
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

        var spec = new Specialisation { Name = name, Faculty = faculty };
        await _repo.AddSpecialisationAsync(spec);
        await _repo.SaveChangesAsync();

        var result = await _context.Specialisations.FirstOrDefaultAsync(s => s.Name == name);
        Assert.NotNull(result);
        Assert.Equal(name, result!.Name);
    }

    [Theory]
    [InlineData("IR1")]
    [InlineData("IR2")]
    public async Task AddGroupYearAsyncTest(string year)
    {

        var faculty = new Faculty { Name = "FMI" };
        var spec = new Specialisation { Name = "Informatica", Faculty = faculty };
        await _context.Specialisations.AddAsync(spec);
        await _context.SaveChangesAsync();

        var groupYear = new GroupYear { Year = year, Specialisation = spec };
        await _repo.AddGroupYearAsync(groupYear);
        await _repo.SaveChangesAsync();

        var result = await _context.GroupYears.FirstOrDefaultAsync(g => g.Year == year);
        Assert.NotNull(result);
        Assert.Equal(year, result!.Year);
    }


    [Theory]
    [InlineData("236")]
    [InlineData("833")]
    public async Task AddGroupAsyncTest(string name)
    {
        var faculty = new Faculty { Name = "FMI" };
        var spec = new Specialisation { Name = "Informatica", Faculty = faculty };
        var year = new GroupYear { Year = "IR1", Specialisation = spec };

        await _context.GroupYears.AddAsync(year);
        await _context.SaveChangesAsync();

        var group = new StudentGroup { Name = name, GroupYear = year };
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
        var year = new GroupYear { Year = "IR1", Specialisation = spec };
        var group = new StudentGroup { Name = "IR1A", GroupYear = year };

        await _context.Groups.AddAsync(group);
        await _context.SaveChangesAsync();

        var subGroup = new StudentSubGroup { Name = name, StudentGroup = group };
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

        var teacher = new Teacher { User = user, Faculty = faculty };
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
        Assert.NotNull(result.Faculty);
    }

    [Theory]
    [InlineData("IR3")]
    [InlineData("IE1")]
    public async Task GetGroupYearByIdAsyncTest(string year)
    {
        var faculty = new Faculty { Name = "FMI" };
        var specialisation = new Specialisation { Name = "Informatica", Faculty = faculty };
        var groupYear = new GroupYear { Year = year, Specialisation = specialisation };

        _context.GroupYears.Add(groupYear);
        await _context.SaveChangesAsync();

        var result = await _repo.GetGroupYearByIdAsync(groupYear.Id);

        Assert.NotNull(result);
        Assert.Equal(year, result.Year);
        Assert.Equal("Informatica", result.Specialisation.Name);
    }

    [Theory]
    [InlineData("IR1", "312", "FMI")]
    public async Task GetGroupByIdAsyncTest(string year, string groupName, string facultyName)
    {
        var faculty = new Faculty { Name = facultyName };
        var specialisation = new Specialisation { Name = "Informatica", Faculty = faculty };
        var groupYear = new GroupYear { Year = year, Specialisation = specialisation };
        var group = new StudentGroup { Name = groupName, GroupYear = groupYear };

        _context.Groups.Add(group);
        await _context.SaveChangesAsync();

        var result = await _repo.GetGroupByIdAsync(group.Id);

        Assert.NotNull(result);
        Assert.Equal(groupName, result.Name);
        Assert.Equal(facultyName, result.GroupYear.Specialisation.Faculty.Name);
    }

    [Theory]
    [InlineData("IR1", "312", "312/2", "FMI")]
    public async Task GetSubGroupByIdAsync_ReturnsCorrectSubGroup(string year, string groupName, string subGroupName,
        string facultyName)
    {

        var faculty = new Faculty { Name = facultyName };
        var specialisation = new Specialisation { Name = "Informatica", Faculty = faculty };
        var groupYear = new GroupYear { Year = year, Specialisation = specialisation };
        var group = new StudentGroup { Name = groupName, GroupYear = groupYear };
        var subGroup = new StudentSubGroup { Name = subGroupName, StudentGroup = group };

        _context.SubGroups.Add(subGroup);
        await _context.SaveChangesAsync();

        var result = await _repo.GetSubGroupByIdAsync(subGroup.Id);

        Assert.NotNull(result);
        Assert.Equal(subGroupName, result.Name);
        Assert.Equal(groupName, result.StudentGroup.Name);
        Assert.Equal(facultyName, result.StudentGroup.GroupYear.Specialisation.Faculty.Name);
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
    public async Task GetGroupYearByIdAsyncNonExisting(int invalidId)
    {

        var result = await _repo.GetGroupYearByIdAsync(invalidId);

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