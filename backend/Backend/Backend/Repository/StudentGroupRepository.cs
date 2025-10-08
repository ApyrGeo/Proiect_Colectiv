using Backend.Context;
using Backend.Domain;
using Backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository;

public class StudentGroupRepository (AcademicAppContext context, ILogger<StudentGroupRepository> logger) 
    : IStudentGroupRepository
{
    private readonly AcademicAppContext _context = context;
    private readonly ILogger<StudentGroupRepository> _logger = logger;

    public async Task<StudentGroup> AddAsync(StudentGroup studentGroup)
    {
        _logger.LogInformation("Adding new student group: {GroupName}", studentGroup.Name);

        await _context.Groups.AddAsync(studentGroup);
        return studentGroup;
    }

    public async Task<StudentGroup?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Fetching student group with ID: {GroupId}", id);

        return await _context.Groups
            .Include(g => g.StudentSubGroups)
            .Include(g => g.GroupYear)
                .ThenInclude(gy => gy.Specialisation)
                    .ThenInclude(s => s.Faculty)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public Task<StudentGroup?> GetByNameAsync(string name)
    {
        _logger.LogInformation("Fetching student group with Name: {GroupName}", name);

        return _context.Groups.FirstOrDefaultAsync(g => g.Name == name);
    }

    public async Task SaveChangesAsync()
    {
        _logger.LogInformation("Saving changes to the database");

        await _context.SaveChangesAsync();
    }
}
