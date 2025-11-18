using Repository.Context;
using Domain;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DataSeeder;

public class TeacherDataSeeder(AcademicAppContext context)
{
    private readonly AcademicAppContext _context = context;

    public async Task SeedAsync()
    {
        if (await _context.Teachers.AnyAsync())
            return;

        var userTeachers = await _context.Users.Where(u => u.Role == UserRole.Teacher).ToListAsync();
        var faculties = await _context.Faculties.ToListAsync();

        var teachers = new List<Teacher>();

        var random = new Random();
        int count = faculties.Count;

        userTeachers.ForEach(ut =>
        {
            var teacher = new Teacher
            {
                User = ut,
                Faculty = faculties[random.Next(count)],
            };
            teachers.Add(teacher);
        });

        await _context.Teachers.AddRangeAsync(teachers);
        await _context.SaveChangesAsync();
    }
}
