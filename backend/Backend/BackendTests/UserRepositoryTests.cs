using Backend.Context;
using Backend.Domain;
using Backend.Domain.Enums;
using Backend.Repository;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BackendTests;

public class UserRepositoryTests
{
    private AcademicAppContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AcademicAppContext>()
            .UseInMemoryDatabase(databaseName: "UserRepositoryTestsDB")
            .Options;

        return new AcademicAppContext(options);
    }
    
    [Theory]
    [InlineData("andrei@gmail.com")]
    public async Task GetByEmailAsync(string email)
    {
        using var context = CreateInMemoryContext();
        context.Users.Add(new User {  Email = email, FirstName = "Andrei", LastName = "Rotaru", Password = "1234", PhoneNumber = "+4077", Role = UserRole.Student });
        await context.SaveChangesAsync();

        var repo = new UserRepository(context);
        
        var result = await repo.GetByEmailAsync(email);
        
        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
    }
    
    [Theory]
    [InlineData("andrei@gmail.com")]
    public async Task GetByEmailAsyncWithoutUser(string email)
    {
        using var context = CreateInMemoryContext();
        var repo = new UserRepository(context);

        var result = await repo.GetByEmailAsync(email);

        Assert.Null(result);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    public async Task GetByIdAsyncExistingUser(int id)
    {
        using var context = CreateInMemoryContext();
        context.Users.Add(new User {  Email = $"user{id}@mail.com", FirstName = "Test", LastName = "User", Password = "111", PhoneNumber = "+400", Role = UserRole.Student });
        await context.SaveChangesAsync();

        var repo = new UserRepository(context);

        var result = await repo.GetByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(999)]
    public async Task GetByIdAsyncUserNotFound(int id)
    {
        using var context = CreateInMemoryContext();
        var repo = new UserRepository(context);

        var result = await repo.GetByIdAsync(id);

        Assert.Null(result);
    }
    
    [Theory]
    [InlineData( "Vanya", "Doktorovic", "0759305094", "vandok@gmail.com", "pass1234", UserRole.Admin)]
    [InlineData( "Andrei", "Horo", "0779725710", "horo@gmail.com", "password", UserRole.Student)]
    public async Task AddAsyncValidUser(string firstName, string lastName ,string phoneNumber, string email, string password,UserRole role )
    {
        using var context = CreateInMemoryContext();
        var repo = new UserRepository(context);

        var user = new User { Email = email, FirstName = firstName, LastName = lastName, Password = password, PhoneNumber = phoneNumber, Role = role };

        await repo.AddAsync(user);
        await repo.SaveChangesAsync();

        var dbUser = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        Assert.NotNull(dbUser);
        Assert.Equal(email, dbUser.Email);
    }
    
    [Fact]
    public async Task GetAll_ReturnsAllUsers()
    {
        using var context = CreateInMemoryContext();
        context.Users.AddRange(
            new User { Id = 1, Email = "a@a.com", FirstName = "A", LastName = "A", Password = "p", PhoneNumber = "+40779725710", Role = UserRole.Student },
            new User { Id = 2, Email = "b@b.com", FirstName = "B", LastName = "B", Password = "p", PhoneNumber = "+40779725710", Role = UserRole.Admin }
        );
        await context.SaveChangesAsync();

        var repo = new UserRepository(context);

        var result = await repo.GetAll();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }
}