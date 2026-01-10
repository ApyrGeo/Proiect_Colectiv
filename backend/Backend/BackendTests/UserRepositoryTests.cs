using AutoMapper;
using TrackForUBB.Repository.Context;
using TrackForUBB.Repository.EFEntities;
using TrackForUBB.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using TrackForUBB.Domain.DTOs;
using Xunit;
using TrackForUBB.Domain.Enums;
using TrackForUBB.Repository.AutoMapper;

namespace TrackForUBB.BackendTests;

public class UserRepositoryTests : IDisposable
{
    private readonly AcademicAppContext _context;
    private readonly UserRepository _repo;
   

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AcademicAppContext>()
            .UseInMemoryDatabase(databaseName: "UserRepositoryTestsDB")
            .Options;
        
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<EFEntitiesMappingProfile>(); 
        },new NullLoggerFactory());

        IMapper mapper = config.CreateMapper();

        _context = new AcademicAppContext(options);
        _repo = new UserRepository(_context,mapper);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }


    [Theory]
    [InlineData("andrei@gmail.com")]
    public async Task GetByEmailAsync(string email)
    {
        _context.Users.Add(new User
        {
            Email = email,
            FirstName = "Andrei",
            LastName = "Rotaru",
            PhoneNumber = "+4077",
            TenantEmail = "andrei.rotaru@trackforubb.onmicrosoft.com",
            Role = UserRole.Student
        });
        await _context.SaveChangesAsync();

        var result = await _repo.GetByEmailAsync(email);

        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
    }

    [Theory]
    [InlineData("andrei@gmail.com")]
    public async Task GetByEmailAsyncWithoutUser(string email)
    {
        var result = await _repo.GetByEmailAsync(email);

        Assert.Null(result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    public async Task GetByIdAsyncExistingUser(int id)
    {
        var user = new User
        {
            Email = $"user{id}@mail.com",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "+400",
            TenantEmail = "test.user@trackforubb.onmicrosoft.com",
            Role = UserRole.Student
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var result = await _repo.GetByIdAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(999)]
    public async Task GetByIdAsyncUserNotFound(int id)
    {
        var result = await _repo.GetByIdAsync(id);

        Assert.Null(result);
    }

    [Theory]
    [InlineData("Vanya", "Doktorovic", "0759305094", "vandok@gmail.com", UserRole.Admin)]
    [InlineData("Andrei", "Horo", "0779725710", "horo@gmail.com", UserRole.Student)]
    public async Task AddAsyncValidUser(string firstName, string lastName, string phoneNumber, string email,
        UserRole role)
    {
        var user = new InternalUserPostDTO
        {
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber,
            Role = role.ToString()
        };

        await _repo.AddAsync(user);
        await _repo.SaveChangesAsync();

        var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        Assert.NotNull(dbUser);
        Assert.Equal(email, dbUser.Email);
    }

    [Fact]
    public async Task GetAll_ReturnsAllUsers()
    {
        _context.Users.AddRange(
            new User
            {
                Id = 1,
                Email = "a@a.com",
                FirstName = "A",
                LastName = "A",
                PhoneNumber = "+40779725710",
                TenantEmail = "a.a@trackforubb.onmicrosoft.com",
                Role = UserRole.Student
            },
            new User
            {
                Id = 2,
                Email = "b@b.com",
                FirstName = "B",
                LastName = "B",
                PhoneNumber = "+40779725710",
                TenantEmail = "b.b@trackforubb.onmicrosoft.com",
                Role = UserRole.Admin
            }
        );
        await _context.SaveChangesAsync();

        var result = await _repo.GetAll(null);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }
}