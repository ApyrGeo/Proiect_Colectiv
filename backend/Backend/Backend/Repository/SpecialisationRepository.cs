using Backend.Context;
using Backend.Domain;
using Backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository;

public class SpecialisationRepository(AcademicAppContext context, ILogger<SpecialisationRepository> logger) 
    : ISpecialisationRepository
{
    private readonly ILogger<SpecialisationRepository> _logger = logger;
    private readonly AcademicAppContext _context = context;

    public async Task<Specialisation> AddAsync(Specialisation specialisation)
    {
        _logger.LogInformation("Adding new specialisation: {SpecialisationName}", specialisation.Name);

        _context.Specialisations.Add(specialisation);
        return specialisation;
    }

    public async Task<Specialisation?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Fetching specialisation with ID: {SpecialisationId}", id);

        return await _context.Specialisations
            .Include(s => s.Faculty)
            .Include(s => s.GroupYears)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Specialisation?> GetByNameAsync(string name)
    {
        _logger.LogInformation("Fetching specialisation with Name: {SpecialisationName}", name);

        return await _context.Specialisations
            .FirstOrDefaultAsync(s => s.Name == name);
    }

    public async Task SaveChangesAsync()
    {
        _logger.LogInformation("Saving changes to the database");

        await _context.SaveChangesAsync();
    }
}
