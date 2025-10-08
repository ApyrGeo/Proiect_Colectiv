using Backend.Context;
using Backend.Domain;
using Backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository;

public class UserRepository(AcademicAppContext context, ILogger<UserRepository> logger) 
    : IUserRepository
{
    private readonly AcademicAppContext _context = context;
    private readonly ILogger _logger = logger;

    public async Task<User?> GetByEmailAsync(string email)
    {
        _logger.LogInformation("Fetching user by email: {Email}", email);
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> AddAsync(User user)
    {
        _logger.LogInformation("Adding new user with email: {Email}", user.Email);
        await _context.Users.AddAsync(user);

        return user;
    }

    public Task<User?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Fetching user by ID: {Id}", id);
        return _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        _logger.LogInformation("Saving changes");
        await _context.SaveChangesAsync();
    }

    public Task<List<User>> GetAll()
    {
        _logger.LogInformation("Fetching all users");
        return _context.Users.ToListAsync();
    }
}
