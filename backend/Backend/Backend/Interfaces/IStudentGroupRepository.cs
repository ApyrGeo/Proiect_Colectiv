using Backend.Domain;
using Backend.Domain.DTOs;

namespace Backend.Interfaces;

public interface IStudentGroupRepository
{
    Task<StudentGroup> AddAsync(StudentGroup studentGroup);
    Task<StudentGroup?> GetByIdAsync(int id);
    Task<StudentGroup?> GetByNameAsync(string name);
    Task SaveChangesAsync();
}
