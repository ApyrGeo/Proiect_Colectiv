using Backend.Context;
using Backend.Domain;
using Backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository;

public class StudentSubGroupRepository(AcademicAppContext context, ILogger<StudentSubGroupRepository> logger) 
    : IStudentSubGroupRepository
{
    private readonly AcademicAppContext _context = context;
    private readonly ILogger<StudentSubGroupRepository> _logger = logger;

    public async Task<StudentSubGroup> AddAsync(StudentSubGroup studentSubGroup)
    {
        _logger.LogInformation("Adding new student sub-group: {SubGroupName}", studentSubGroup.Name);

        await _context.SubGroups.AddAsync(studentSubGroup);
        return studentSubGroup;
    }

    public async Task<StudentSubGroup?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Fetching student sub-group with ID: {SubGroupId}", id);

        return await _context.SubGroups
            .Include(sg => sg.StudentGroup)
                .ThenInclude(g => g.GroupYear)
                    .ThenInclude(gy => gy.Specialisation)
                        .ThenInclude(s => s.Faculty)
            .Include(sg => sg.Enrollments)
            .FirstOrDefaultAsync(sg => sg.Id == id);
    }

    public async Task<StudentSubGroup?> GetByNameAsync(string name)
    {
        _logger.LogInformation("Fetching student sub-group with Name: {SubGroupName}", name);

        return await _context.SubGroups.FirstOrDefaultAsync(sg => sg.Name == name);
    }

    public async Task SaveChangesAsync()
    {
        _logger.LogInformation("Saving changes to the database");

        await _context.SaveChangesAsync();
    }
}
