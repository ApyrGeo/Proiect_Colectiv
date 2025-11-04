using Backend.Context;
using Backend.Domain;
using Backend.Interfaces;
using log4net;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository;

public class UserRepository(AcademicAppContext context) 
    : IUserRepository
{
    private readonly AcademicAppContext _context = context;
    private readonly ILog _logger = LogManager.GetLogger(typeof(UserRepository));

    public async Task<User?> GetByEmailAsync(string email)
    {
        _logger.InfoFormat("Fetching user by email: {0}", email);
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> AddAsync(User user)
    {
        _logger.InfoFormat("Adding new user with email: {0}", user.Email);
        await _context.Users.AddAsync(user);

        return user;
    }

    public Task<User?> GetByIdAsync(int id)
    {
        _logger.InfoFormat("Fetching user by ID: {0}", id);
        return _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        _logger.InfoFormat("Saving changes");
        await _context.SaveChangesAsync();
    }

    public Task<List<User>> GetAll()
    {
        _logger.InfoFormat("Fetching all users");
        return _context.Users.ToListAsync();
    }
}
