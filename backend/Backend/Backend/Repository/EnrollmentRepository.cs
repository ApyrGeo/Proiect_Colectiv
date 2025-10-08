
using Backend.Context;
using Backend.Domain;
using Backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository;

public class EnrollmentRepository (AcademicAppContext context, ILogger<EnrollmentRepository> logger) : IEnrollmentRepository
{
    private readonly AcademicAppContext _context = context;
    private readonly ILogger _logger = logger;

    public async Task<Enrollment> AddAsync(Enrollment enrollment)
    {
        _logger.LogInformation("Adding new enrollment for user ID: {UserId}", enrollment.UserId);
        await _context.Enrollments.AddAsync(enrollment);
        return enrollment;
    }

    public async Task<Enrollment?> GetEnrollmentByUserId(int userId)
    {
        _logger.LogInformation("Fetching enrollment for user ID: {UserId}", userId);
        return await _context.Enrollments
            .Include(e => e.SubGroup)
                .ThenInclude(sg => sg.StudentGroup)
                    .ThenInclude(g => g.GroupYear)
                        .ThenInclude(gy => gy.Specialisation)
                            .ThenInclude(s => s.Faculty)
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.UserId == userId);
    }

    public async Task SaveChangesAsync()
    {
        _logger.LogInformation("Saving changes");
        await _context.SaveChangesAsync();
    }
}
