using Backend.Context;
using Backend.Domain;
using Backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository;

public class GroupYearRepository(AcademicAppContext context, ILogger<GroupYearRepository> logger) 
    : IGroupYearRepository
{
    private readonly ILogger<GroupYearRepository> _logger = logger;
    private AcademicAppContext _context = context;

    public async Task<GroupYear> AddAsync(GroupYear groupYear)
    {
        _logger.LogInformation("Adding new group year: {GroupYear}", groupYear.Year);

        await _context.GroupYears.AddAsync(groupYear);
        return groupYear;
    }

    public async Task<GroupYear?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Fetching group year with ID: {GroupYearId}", id);

        return await _context.GroupYears
            .Include(gy => gy.StudentGroups)
            .Include(gy => gy.Specialisation)
                .ThenInclude(s => s.Faculty)
            .FirstOrDefaultAsync(gy => gy.Id == id);
    }

    public async Task<GroupYear?> GetByYearAsync(string year)
    {
        _logger.LogInformation("Fetching group year with Year: {GroupYear}", year);

        return await _context.GroupYears
            .Include(gy => gy.StudentGroups)
            .Include(gy => gy.Specialisation)
                .ThenInclude(s => s.Faculty)
            .FirstOrDefaultAsync(gy => gy.Year == year);
    }

    public async Task SaveChangesAsync()
    {
        _logger.LogInformation("Saving changes to the database");

        await _context.SaveChangesAsync();
    }
}
