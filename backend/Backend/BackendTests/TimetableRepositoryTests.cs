using Xunit;

namespace BackendTests;
using Microsoft.EntityFrameworkCore;
using Backend.Context;
using Backend.Domain;
using Backend.Repository;
using System.Threading.Tasks;

public class TimetableRepositoryTests
{
    private readonly AcademicAppContext _context;
    private readonly TimetableRepository _repository;

    public TimetableRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AcademicAppContext>()
            .UseInMemoryDatabase(databaseName: "TimetableTestDb")
            .Options;
        _context = new AcademicAppContext(options);
        _repository = new TimetableRepository(_context);
    }

    [Fact]
    public async Task AddSubjectAsyncTest()
    {
        var groupYear = new GroupYear {  };
        await _context.GroupYears.AddAsync(groupYear);
        await _context.SaveChangesAsync();
        
        var subject = new Subject { Name = "LFTC", NumberOfCredits = 6 };
        await _repository.AddSubjectAsync(subject);

        var added = await _repository.AddSubjectAsync(subject);
        await _repository.SaveChangesAsync();

        var fetched = await _repository.GetSubjectByIdAsync(subject.Id);

        Assert.NotNull(fetched);
        Assert.Equal("LFTC", fetched.Name);
    }

    [Fact]
    public async Task GetSubjectByNameAsyncTest()
    {
        var subject = new Subject { Name = "FP", NumberOfCredits = 6};
        await _repository.AddSubjectAsync(subject);
        await _repository.SaveChangesAsync();

        var result = await _repository.GetSubjectByNameAsync("Physics");

        Assert.NotNull(result);
        Assert.Equal(subject.Id, result.Id);
    }

    [Fact]
    public async Task GetSubjectByIdAsyncWithInvalidIdTest()
    {
        var result = await _repository.GetSubjectByIdAsync(999);
        Assert.Null(result);
    }
}