using Backend.Context;
using Backend.Domain;
using Backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository;

public class FacultyRepository (AcademicAppContext context, ILogger<FacultyRepository> logger) 
    : IFacultyRepository
{
    private readonly AcademicAppContext _context = context;
    private readonly ILogger<FacultyRepository> _logger = logger;

    public async Task<Faculty> AddAsync(Faculty faculty)
    { 
        _logger.LogInformation("Adding new faculty: {FacultyName}", faculty.Name);

        await _context.Faculties.AddAsync(faculty);
        return faculty;
    }

    public async Task<Faculty?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Fetching faculty with ID: {FacultyId}", id);

        return await _context.Faculties
            .Include(f => f.Specialisations)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public Task<Faculty?> GetByNameAsync(string name)
    {
        _logger.LogInformation("Fetching faculty with Name: {FacultyName}", name);

        return _context.Faculties
            .Include(f => f.Specialisations)
            .FirstOrDefaultAsync(f => f.Name == name);
    }

    public async Task SaveChangesAsync()
    {
        _logger.LogInformation("Saving changes to the database");

        await _context.SaveChangesAsync();
    }
}
