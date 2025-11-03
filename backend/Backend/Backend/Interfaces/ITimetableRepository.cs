using Backend.Domain;

namespace Backend.Interfaces;

public interface ITimetableRepository
{
    Task<Subject> AddSubjectAsync(Subject subject);
    Task<Subject?> GetSubjectByIdAsync(int id);
    Task<Subject?> GetSubjectByNameAsync(string name);
    Task SaveChangesAsync();
}