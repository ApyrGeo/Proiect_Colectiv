using Backend.Domain;

namespace Backend.Interfaces;

public interface IFacultyRepository
{
    Task<Faculty> AddAsync(Faculty faculty);
    Task<Faculty?> GetByIdAsync(int id);
    Task<Faculty?> GetByNameAsync(string name);
    Task SaveChangesAsync();
}
