using Backend.Context;
using Backend.Domain;
using Backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository;

public class AcademicRepository (AcademicAppContext context) : IAcademicRepository
{
    private readonly AcademicAppContext _context = context;

    public async Task<Enrollment> AddEnrollmentAsync(Enrollment enrollment)
    {
        await _context.Enrollments.AddAsync(enrollment);
        return enrollment;
    }

    public async Task<Faculty> AddFacultyAsync(Faculty faculty)
    {
        await _context.Faculties.AddAsync(faculty);
        return faculty;
    }

    public async Task<StudentGroup> AddGroupAsync(StudentGroup studentGroup)
    {
        await _context.Groups.AddAsync(studentGroup);
        return studentGroup;
    }

    public async Task<GroupYear> AddGroupYearAsync(GroupYear groupYear)
    {
        await _context.GroupYears.AddAsync(groupYear);
        return groupYear;
    }

    public async Task<Specialisation> AddSpecialisationAsync(Specialisation specialisation)
    {
        await _context.Specialisations.AddAsync(specialisation);
        return specialisation;
    }

    public async Task<StudentSubGroup> AddSubGroupAsync(StudentSubGroup studentSubGroup)
    {
        await _context.SubGroups.AddAsync(studentSubGroup);
        return studentSubGroup;
    }

    public async Task<List<Enrollment>> GetEnrollmentsByUserId(int userId)
    {
        return await _context.Enrollments
            .Include(e => e.SubGroup)
                .ThenInclude(sg => sg.StudentGroup)
                    .ThenInclude(g => g.GroupYear)
                        .ThenInclude(gy => gy.Specialisation)
                            .ThenInclude(s => s.Faculty)
            .Include(e => e.User)
            .Where(e => e.UserId == userId)
            .ToListAsync();
    }

    public async Task<Faculty?> GetFacultyByIdAsync(int id)
    {
        return await _context.Faculties
            .Include(f => f.Specialisations)
            .SingleOrDefaultAsync(f => f.Id == id);
    }

    public async Task<Faculty?> GetFacultyByNameAsync(string name)
    {
        return await _context.Faculties.FirstOrDefaultAsync(f => f.Name == name);
    }

    public async Task<StudentGroup?> GetGroupByIdAsync(int id)
    {
        return await _context.Groups
            .Include(g => g.StudentSubGroups)
            .Include(g => g.GroupYear)
                .ThenInclude(gy => gy.Specialisation)   
                    .ThenInclude(s => s.Faculty)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<GroupYear?> GetGroupYearByIdAsync(int id)
    {
        return await _context.GroupYears
            .Include(gy => gy.StudentGroups)
            .Include(gy => gy.Specialisation)
                .ThenInclude(s => s.Faculty)
            .FirstOrDefaultAsync(gy => gy.Id == id);
    }

    public async Task<Specialisation?> GetSpecialisationByIdAsync(int id)
    {
        return await _context.Specialisations
             .Include(s => s.Faculty)
             .Include(s => s.GroupYears)
             .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<StudentSubGroup?> GetSubGroupByIdAsync(int id)
    {
        return await _context.SubGroups
            .Include(sg => sg.StudentGroup)
                .ThenInclude(g => g.GroupYear)
                    .ThenInclude(gy => gy.Specialisation)
                        .ThenInclude(s => s.Faculty)
            .Include(sg => sg.Enrollments)
            .FirstOrDefaultAsync(sg => sg.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
