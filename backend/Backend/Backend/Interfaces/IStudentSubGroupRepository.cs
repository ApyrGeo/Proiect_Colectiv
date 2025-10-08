using Backend.Domain;

namespace Backend.Interfaces;

public interface IStudentSubGroupRepository
{
    Task<StudentSubGroup> AddAsync(StudentSubGroup studentSubGroup);
    Task<StudentSubGroup?> GetByIdAsync(int id);
    Task<StudentSubGroup?> GetByNameAsync(string name);
    Task SaveChangesAsync();
}
