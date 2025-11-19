using TrackForUBB.Domain.Enums;
using Xunit;
using Microsoft.EntityFrameworkCore;
using TrackForUBB.Repository.Context;
using TrackForUBB.Domain;
using TrackForUBB.Repository;
using System.Threading.Tasks;

namespace TrackForUBB.BackendTests;

public class TimetableRepositoryTests : IDisposable
{
    private readonly AcademicAppContext _context;
    private readonly TimetableRepository _repo;


    public TimetableRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AcademicAppContext>()
            .UseInMemoryDatabase(databaseName: "TimetableRepositoryTestsDB")
            .Options;
        _context = new AcademicAppContext(options);
        _repo = new TimetableRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Theory]
    [InlineData("L001")]
    public async Task AddClassroomAsyncTest(string name)
    {
        var location = new Location { Name = "Fsega", Address = "Str. Mihail Kogalniceanu" };
        _context.Locations.Add(location);
        await _context.SaveChangesAsync();

        var classroom = new Classroom { Name = name, Location = location, LocationId = location.Id };
        await _repo.AddClassroomAsync(classroom);
        await _context.SaveChangesAsync();

        var result = await _context.Classrooms.FirstOrDefaultAsync(c => c.Name == name);
        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
    }

    [Theory]
    [InlineData("Cladire Centru", "Strada Universitatii")]
    public async Task AddLocationAsyncTest(string name, string address)
    {
        var location = new Location { Name = name, Address = address };
        await _repo.AddLocationAsync(location);
        await _context.SaveChangesAsync();

        var result = await _context.Locations.FirstOrDefaultAsync(l => l.Name == name);
        Assert.NotNull(result);
        Assert.Equal(address, result.Address);
    }

    [Theory]
    [InlineData("SDA", 6)]
    [InlineData("Sport", 2)]
    public async Task AddSubjectAsyncTest(string name, int numberOfCredits)
    {
        var faculty = new Faculty { Name = "Facultate de Mate-Info" };

        var specialisation = new Specialisation { Name = "Computer Science", Faculty = faculty };

        var groupYear = new GroupYear { Year = "IR1", Specialisation = specialisation };

        var subject = new Subject
        { Name = name, NumberOfCredits = numberOfCredits, GroupYearId = groupYear.Id, GroupYear = groupYear };
        await _repo.AddSubjectAsync(subject);
        await _context.SaveChangesAsync();

        var result = await _context.Subjects.FirstOrDefaultAsync(s => s.Name == name);
        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
    }

    [Theory]
    [InlineData(1)]
    public async Task GetClassroomByIdAsyncExistingId(int id)
    {
        var location = new Location { Name = "Fsega", Address = "Str Goldis" };
        var classroom = new Classroom { Name = "A303", Location = location };

        _context.Locations.Add(location);
        _context.Classrooms.Add(classroom);
        await _context.SaveChangesAsync();

        var result = await _repo.GetClassroomByIdAsync(id);
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Theory]
    [InlineData(999)]
    public async Task GetClassroomByIdAsyncNonExisting(int id)
    {
        var result = await _repo.GetClassroomByIdAsync(id);
        Assert.Null(result);
    }

    [Theory]
    [InlineData("Fsega", "Str Goldis")]
    public async Task GetLocationByIdAsyncExistingId(string name, string address)
    {
        var location = new Location { Name = name, Address = address };
        _context.Locations.Add(location);
        await _context.SaveChangesAsync();

        var result = await _repo.GetLocationByIdAsync(location.Id);

        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
        Assert.Equal(address, result.Address);
    }

    [Theory]
    [InlineData("OOP")]
    public async Task GetSubjectByNameAsyncExistingName(string name)
    {
        var faculty = new Faculty { Name = "Facultate de Mate-Info" };

        var specialisation = new Specialisation { Name = "Computer Science", Faculty = faculty };

        var groupYear = new GroupYear { Year = "IR1", Specialisation = specialisation };

        var subject = new Subject
        { Name = name, NumberOfCredits = 4, GroupYearId = groupYear.Id, GroupYear = groupYear };

        subject.Name = name;

        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        var result = await _repo.GetSubjectByNameAsync(name);
        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
    }

    [Theory]
    [InlineData("None")]
    public async Task GetSubjectByNameAsyncNonExisting(string name)
    {
        var result = await _repo.GetSubjectByNameAsync(name);
        Assert.Null(result);
    }

    [Theory]
    [InlineData(1)]
    public async Task GetHourByIdAsyncExistingId(int id)
    {
        var faculty = new Faculty { Name = "Facultate de Mate-Info" };

        var specialisation = new Specialisation { Name = "Computer Science", Faculty = faculty };

        var groupYear = new GroupYear { Year = "IR1", Specialisation = specialisation };

        var subject = new Subject
        { Name = "FP", NumberOfCredits = 4, GroupYearId = groupYear.Id, GroupYear = groupYear };
        var location = new Location { Name = "Fsega", Address = "Str Goldis" };
        var classroom = new Classroom { Name = "A303", Location = location };

        var teacher = new Teacher
        {
            Id = 1,
            User = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@uni.com",
                Password = "pass",
                PhoneNumber = "+40770111222",
                Role = UserRole.Teacher
            },
            Faculty = faculty
        };

        var hour = new Hour
        {
            Id = id,
            Day = HourDay.Monday,
            HourInterval = "10:00-12:00",
            Frequency = HourFrequency.Weekly,
            Category = HourCategory.Lecture,
            Subject = subject,
            Classroom = classroom,
            Teacher = teacher
        };

        _context.Hours.Add(hour);
        await _context.SaveChangesAsync();

        var result = await _repo.GetHourByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal("10:00-12:00", result.HourInterval);
    }
}