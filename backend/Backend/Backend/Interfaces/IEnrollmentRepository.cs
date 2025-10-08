using Backend.Domain;

namespace Backend.Interfaces;

public interface IEnrollmentRepository
{
    Task<Enrollment> AddAsync(Enrollment enrollment);
    Task<Enrollment?> GetEnrollmentByUserId(int userId);
    Task SaveChangesAsync();
}
