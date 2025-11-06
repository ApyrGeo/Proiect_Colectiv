using Backend.Context;
using Backend.Domain;
using Backend.Domain.Enums;
using Backend.Repository;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BackendTests;

public class AcademicRepositoryTests
{
    private AcademicAppContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AcademicAppContext>()
            .UseInMemoryDatabase(databaseName: "AcademicRepositoryTestsDB")
            .Options;

        return new AcademicAppContext(options);
    }
    
    [Theory]
    [InlineData("Facultatea de Drept")]
    [InlineData("Facultatea de Geografie")]
    public async Task AddFacultyAsyncTest(string name)
    {
        using var context = CreateInMemoryContext();
        var repo = new AcademicRepository(context);

        var faculty = new Faculty { Name = name };
        await repo.AddFacultyAsync(faculty);
        await repo.SaveChangesAsync();

        var result = await context.Faculties.FirstOrDefaultAsync(f => f.Name == name);
        Assert.NotNull(result);
        Assert.Equal(name, result!.Name);
    }
    
    [Theory]
    [InlineData("Computer-Science")]
    [InlineData("Inginerie")]
    public async Task AddSpecialisationAsyncTest(string name)
    {
        using var context = CreateInMemoryContext();
        var repo = new AcademicRepository(context);

        var faculty = new Faculty { Name = "FMI" };
        await context.Faculties.AddAsync(faculty);
        await context.SaveChangesAsync();

        var spec = new Specialisation { Name = name, Faculty = faculty };
        await repo.AddSpecialisationAsync(spec);
        await repo.SaveChangesAsync();

        var result = await context.Specialisations.FirstOrDefaultAsync(s => s.Name == name);
        Assert.NotNull(result);
        Assert.Equal(name, result!.Name);
    }
    
    [Theory]
    [InlineData("IR1")]
    [InlineData("IR2")]
    public async Task AddGroupYearAsyncTest(string year)
    {
        using var context = CreateInMemoryContext();
        var repo = new AcademicRepository(context);

        var faculty = new Faculty { Name = "FMI" };
        var spec = new Specialisation { Name = "Informatica", Faculty = faculty };
        await context.Specialisations.AddAsync(spec);
        await context.SaveChangesAsync();

        var groupYear = new GroupYear { Year = year, Specialisation = spec };
        await repo.AddGroupYearAsync(groupYear);
        await repo.SaveChangesAsync();

        var result = await context.GroupYears.FirstOrDefaultAsync(g => g.Year == year);
        Assert.NotNull(result);
        Assert.Equal(year, result!.Year);
    }
    
    
    [Theory]
    [InlineData("236")]
    [InlineData("833")]
    public async Task AddGroupAsyncTest(string name)
    {
        using var context = CreateInMemoryContext();
        var repo = new AcademicRepository(context);

        var faculty = new Faculty { Name = "FMI" };
        var spec = new Specialisation { Name = "Informatica", Faculty = faculty };
        var year = new GroupYear { Year = "IR1", Specialisation = spec };

        await context.GroupYears.AddAsync(year);
        await context.SaveChangesAsync();

        var group = new StudentGroup { Name = name, GroupYear = year };
        await repo.AddGroupAsync(group);
        await repo.SaveChangesAsync();

        var result = await context.Groups.FirstOrDefaultAsync(g => g.Name == name);
        Assert.NotNull(result);
        Assert.Equal(name, result!.Name);
    }
    
    [Theory]
    [InlineData("235/1")]
    [InlineData("237/2")]
    public async Task AddSubGroupAsyncTest(string name)
    {
        using var context = CreateInMemoryContext();
        var repo = new AcademicRepository(context);

        var faculty = new Faculty { Name = "FMI" };
        var spec = new Specialisation { Name = "Informatica", Faculty = faculty };
        var year = new GroupYear { Year = "IR1", Specialisation = spec };
        var group = new StudentGroup { Name = "IR1A", GroupYear = year };

        await context.Groups.AddAsync(group);
        await context.SaveChangesAsync();

        var subGroup = new StudentSubGroup { Name = name, StudentGroup= group };
        await repo.AddSubGroupAsync(subGroup);
        await repo.SaveChangesAsync();

        var result = await context.SubGroups.FirstOrDefaultAsync(sg => sg.Name == name);
        Assert.NotNull(result);
        Assert.Equal(name, result!.Name);
    }
    
    [Theory]
    [InlineData("Istvan", "Csibula")]
    [InlineData("Dan", "Suciu")]
    public async Task AddTeacherAsyncTest(string firstName, string lastName)
    {
        using var context = CreateInMemoryContext();
        var repo = new AcademicRepository(context);

        var user = new User { FirstName = firstName, LastName = lastName, Email = $"{firstName}@mail.com", PhoneNumber = "+40988301069", Role = UserRole.Admin, Password = "1234567"};
        var faculty = new Faculty { Name = "FMI" };

        await context.Users.AddAsync(user);
        await context.Faculties.AddAsync(faculty);
        await context.SaveChangesAsync();

        var teacher = new Teacher { User = user, Faculty = faculty };
        await repo.AddTeacherAsync(teacher);
        await repo.SaveChangesAsync();

        var result = await context.Teachers.Include(t => t.User).FirstOrDefaultAsync(t => t.UserId == user.Id);
        Assert.NotNull(result);
        Assert.Equal(firstName, result!.User.FirstName);
    }
    
    [Theory]
    [InlineData(1)]
    public async Task GetFacultyByIdAsyncExistingId(int id)
    {
        using var context = CreateInMemoryContext();
        var repo = new AcademicRepository(context);

       
        var faculty = new Faculty { Name = "FSEGA" };
        await context.Faculties.AddAsync(faculty);
        await context.SaveChangesAsync();

        var result = await repo.GetFacultyByIdAsync(faculty.Id);

        Assert.NotNull(result);
        Assert.Equal("FSEGA", result!.Name);
    }
    
    [Theory]
    [InlineData("Istvan", "Csibula")]
    [InlineData("Dan", "Suciu")]
    public async Task GetTeacherByIdTest(string firstName, string lastName)
    {
        using var context = CreateInMemoryContext();
        var repo = new AcademicRepository(context);

        var faculty = new Faculty { Name = "FMI" };
        var user =new User { FirstName = firstName, LastName = lastName, Email = $"{firstName}@mail.com", PhoneNumber = "+40988301069", Role = UserRole.Admin, Password = "1234567"};
        var teacher = new Teacher { User = user, Faculty = faculty };

        await context.Teachers.AddAsync(teacher);
        await context.SaveChangesAsync();

        var result = await repo.GetTeacherById(teacher.Id);

        Assert.NotNull(result);
        Assert.Equal(teacher.User.FirstName, result!.User.FirstName);
    }
    
    [Theory]
    [InlineData("Computer-Science")]
    [InlineData("Inginerie")]
    public async Task GetSpecialisationByIdAsyncTest(string name)
    {
        using var context = CreateInMemoryContext();
        var repo = new AcademicRepository(context);

        var faculty = new Faculty { Name = "FMI" };
        var spec = new Specialisation { Name = name, Faculty = faculty };
        await context.Specialisations.AddAsync(spec);
        await context.SaveChangesAsync();

        var result = await repo.GetSpecialisationByIdAsync(spec.Id);

        Assert.NotNull(result);
        Assert.Equal(spec.Name, result!.Name);
        Assert.NotNull(result.Faculty);
    }
    
    [Theory]
    [InlineData("IR3")]
    [InlineData("IE1")]
    public async Task GetGroupYearByIdAsyncTest(string year)
    {
        using var context = CreateInMemoryContext();
        var repo = new AcademicRepository(context);

        var faculty = new Faculty { Name = "FMI" };
        var specialisation = new Specialisation { Name = "Informatica", Faculty = faculty };
        var groupYear = new GroupYear { Year = year, Specialisation = specialisation };

        context.GroupYears.Add(groupYear);
        await context.SaveChangesAsync();

        var result = await repo.GetGroupYearByIdAsync(groupYear.Id);

        Assert.NotNull(result);
        Assert.Equal(year, result.Year);
        Assert.Equal("Informatica", result.Specialisation.Name);
    }
    
    [Theory]
    [InlineData("IR1", "312", "FMI")]
    public async Task GetGroupByIdAsyncTest(string year, string groupName, string facultyName)
    {
        using var context = CreateInMemoryContext();
        var repo = new AcademicRepository(context);

        var faculty = new Faculty { Name = facultyName };
        var specialisation = new Specialisation { Name = "Informatica", Faculty = faculty };
        var groupYear = new GroupYear { Year = year, Specialisation = specialisation };
        var group = new StudentGroup { Name = groupName, GroupYear = groupYear };

        context.Groups.Add(group);
        await context.SaveChangesAsync();

        var result = await repo.GetGroupByIdAsync(group.Id);

        Assert.NotNull(result);
        Assert.Equal(groupName, result.Name);
        Assert.Equal(facultyName, result.GroupYear.Specialisation.Faculty.Name);
    }

    [Theory]
    [InlineData("IR1", "312", "312/2", "FMI")]
    public async Task GetSubGroupByIdAsync_ReturnsCorrectSubGroup(string year, string groupName, string subGroupName, string facultyName)
    {
        using var context = CreateInMemoryContext();
        var repo = new AcademicRepository(context);

        var faculty = new Faculty { Name = facultyName };
        var specialisation = new Specialisation { Name = "Informatica", Faculty = faculty };
        var groupYear = new GroupYear { Year = year, Specialisation = specialisation };
        var group = new StudentGroup { Name = groupName, GroupYear = groupYear };
        var subGroup = new StudentSubGroup { Name = subGroupName, StudentGroup = group };

        context.SubGroups.Add(subGroup);
        await context.SaveChangesAsync();

        var result = await repo.GetSubGroupByIdAsync(subGroup.Id);

        Assert.NotNull(result);
        Assert.Equal(subGroupName, result.Name);
        Assert.Equal(groupName, result.StudentGroup.Name);
        Assert.Equal(facultyName, result.StudentGroup.GroupYear.Specialisation.Faculty.Name);
    }
    
    [Theory]
    [InlineData(999)]
    public async Task GetFacultyByIdAsyncNonExisting(int invalidId)
    {
        using var context = CreateInMemoryContext();
        var repo = new AcademicRepository(context);

        var result = await repo.GetFacultyByIdAsync(invalidId);

        Assert.Null(result);
    }
    
    [Theory]
    [InlineData(999)]
    public async Task GetSpecialisationIdAsyncNonExisting(int invalidId)
    {
        using var context = CreateInMemoryContext();
        var repo = new AcademicRepository(context);

        var result = await repo.GetSpecialisationByIdAsync(invalidId);

        Assert.Null(result);
    }
    
    [Theory]
    [InlineData(999)]
    public async Task GetGroupYearByIdAsyncNonExisting(int invalidId)
    {
        using var context = CreateInMemoryContext();
        var repo = new AcademicRepository(context);

        var result = await repo.GetGroupYearByIdAsync(invalidId);

        Assert.Null(result);
    }
    
    [Theory]
    [InlineData(999)]
    public async Task GetGroupByIdAsyncNonExisting(int invalidId)
    {
        using var context = CreateInMemoryContext();
        var repo = new AcademicRepository(context);

        var result = await repo.GetGroupByIdAsync(invalidId);

        Assert.Null(result);
    }
    
    [Theory]
    [InlineData(999)]
    public async Task GetSubGroupByIdAsyncNonExisting(int invalidId)
    {
        using var context = CreateInMemoryContext();
        var repo = new AcademicRepository(context);

        var result = await repo.GetSubGroupByIdAsync(invalidId);

        Assert.Null(result);
    }
    
    [Theory]
    [InlineData(999)]
    public async Task GetTeacherAsyncNonExisting(int invalidId)
    {
        using var context = CreateInMemoryContext();
        var repo = new AcademicRepository(context);

        var result = await repo.GetTeacherById(invalidId);

        Assert.Null(result);
    }
    
    


    
}