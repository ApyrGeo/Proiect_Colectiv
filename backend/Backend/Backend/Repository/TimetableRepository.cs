using Backend.Context;
using Backend.Domain;
using Backend.Domain.Enums;
using Backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository;

public class TimetableRepository(AcademicAppContext context) : ITimetableRepository
{
    private readonly AcademicAppContext _context = context;
    
    public async Task<Subject> AddSubjectAsync(Subject subject)
    {
        await _context.Subjects.AddAsync(subject);
        return subject;
    }
    
    public async Task<Subject?> GetSubjectByIdAsync(int id)
    {
        return await _context.Subjects.SingleOrDefaultAsync(f => f.Id == id);
    }
    
    public async Task<Subject?> GetSubjectByNameAsync(string name)
    {
        return await _context.Subjects.SingleOrDefaultAsync(f => f.Name == name);
    }
    
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    

}