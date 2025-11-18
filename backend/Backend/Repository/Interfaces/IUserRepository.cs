using Domain;

namespace Repository.Interfaces;
public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<User> AddAsync(User user);
    Task SaveChangesAsync();
    Task<List<User>> GetAll();
}
