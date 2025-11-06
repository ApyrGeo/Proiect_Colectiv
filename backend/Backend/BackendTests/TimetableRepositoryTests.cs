using Backend.Domain.Enums;
using Xunit;

namespace BackendTests;
using Microsoft.EntityFrameworkCore;
using Backend.Context;
using Backend.Domain;
using Backend.Repository;
using System.Threading.Tasks;

public class TimetableRepositoryTests
{
    private AcademicAppContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AcademicAppContext>()
            .UseInMemoryDatabase(databaseName: "TimetableRepositoryTestsDB")
            .Options;

        return new AcademicAppContext(options);
    }
    
    [Theory]
    [InlineData("L001")]
    public async Task AddClassroomAsyncTest(string name)
    {
        using var context = CreateInMemoryContext();
        var repo = new TimetableRepository(context);

        var location = new Location { Name = "Fsega", Address = "Str. Mihail Kogalniceanu" }; 
        context.Locations.Add(location);
        await context.SaveChangesAsync();

        var classroom = new Classroom { Name = name, Location = location, LocationId = location.Id };
        await repo.AddClassroomAsync(classroom);
        await context.SaveChangesAsync();

        var result = await context.Classrooms.FirstOrDefaultAsync(c => c.Name == name);
        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
    }
    
    [Theory]
    [InlineData("Cladire Centru", "Strada Universitatii")]
    public async Task AddLocationAsyncTest(string name, string address)
    {
        using var context = CreateInMemoryContext();
        var repo = new TimetableRepository(context);

        var location = new Location { Name = name, Address = address };
        await repo.AddLocationAsync(location);
        await context.SaveChangesAsync();

        var result = await context.Locations.FirstOrDefaultAsync(l => l.Name == name);
        Assert.NotNull(result);
        Assert.Equal(address, result.Address);
    }
    
    [Theory]
    [InlineData(2, "SDA",6)]
    [InlineData(1,"Sport",2)]
    public async Task AddSubjectAsyncTest(int id,string name, int numberOfCredits)
    {
        using var context = CreateInMemoryContext();
        var repo = new TimetableRepository(context);
        var faculty = new Faculty {  Name = "Facultate de Mate-Info" };
        
        var specialisation = new Specialisation { Name = "Computer Science", Faculty = faculty};
       
        var groupYear = new GroupYear {  Year = "IR1", Specialisation = specialisation};

        var subject = new Subject { Name = name, NumberOfCredits = numberOfCredits, GroupYearId = groupYear.Id, GroupYear = groupYear };
        await repo.AddSubjectAsync(subject);
        await context.SaveChangesAsync();

        var result = await context.Subjects.FirstOrDefaultAsync(s => s.Name == name);
        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
    }
    
    [Theory]
    [InlineData(1)]
    public async Task GetClassroomByIdAsyncExistingId(int id)
    {
        using var context = CreateInMemoryContext();
        var repo = new TimetableRepository(context);

        var location = new Location { Name = "Fsega", Address = "Str Goldis" };
        var classroom = new Classroom { Name = "A303", Location = location };

        context.Locations.Add(location);
        context.Classrooms.Add(classroom);
        await context.SaveChangesAsync();

        var result = await repo.GetClassroomByIdAsync(id);
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }
    
    [Theory]
    [InlineData(999)]
    public async Task GetClassroomByIdAsyncNonExisting(int id)
    {
        using var context = CreateInMemoryContext();
        var repo = new TimetableRepository(context);

        var result = await repo.GetClassroomByIdAsync(id);
        Assert.Null(result);
    }
    
    [Theory]
    [InlineData("Fsega", "Str Goldis")]
    public async Task GetLocationByIdAsyncExistingId(string name, string address)
    {
        using var context = CreateInMemoryContext();
        var repo = new TimetableRepository(context);

        var location = new Location { Name = name, Address = address };
        context.Locations.Add(location);
        await context.SaveChangesAsync();

        var result = await repo.GetLocationByIdAsync(location.Id);

        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
        Assert.Equal(address, result.Address);
    }

    [Theory]
    [InlineData("OOP")]
    public async Task GetSubjectByNameAsyncExistingName(string name)
    {
        using var context = CreateInMemoryContext();
        var repo = new TimetableRepository(context);
        var faculty = new Faculty {  Name = "Facultate de Mate-Info" };
        
        var specialisation = new Specialisation {  Name = "Computer Science", Faculty = faculty};
       
        var groupYear = new GroupYear { Year = "IR1", Specialisation = specialisation};

        var subject = new Subject { Name = name, NumberOfCredits = 4, GroupYearId = groupYear.Id, GroupYear = groupYear };
        
        subject.Name = name;

        context.Subjects.Add(subject);
        await context.SaveChangesAsync();

        var result = await repo.GetSubjectByNameAsync(name);
        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
    }
    
    [Theory]
    [InlineData("None")]
    public async Task GetSubjectByNameAsyncNonExisting(string name)
    {
        using var context = CreateInMemoryContext();
        var repo = new TimetableRepository(context);

        var result = await repo.GetSubjectByNameAsync(name);
        Assert.Null(result);
    }
    
    [Theory]
    [InlineData(1)]
    public async Task GetHourByIdAsyncExistingId(int id)
    {
        using var context = CreateInMemoryContext();
        var repo = new TimetableRepository(context);
        var faculty = new Faculty {  Name = "Facultate de Mate-Info" };
        
        var specialisation = new Specialisation {  Name = "Computer Science", Faculty = faculty};
       
        var groupYear = new GroupYear { Year = "IR1", Specialisation = specialisation};

        var subject = new Subject { Name = "FP", NumberOfCredits = 4, GroupYearId = groupYear.Id, GroupYear = groupYear };
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

        context.Hours.Add(hour);
        await context.SaveChangesAsync();

        var result = await repo.GetHourByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal("10:00-12:00", result.HourInterval);
    }
    
}