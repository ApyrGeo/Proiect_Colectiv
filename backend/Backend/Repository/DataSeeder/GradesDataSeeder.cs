using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackForUBB.Repository.Context;
using TrackForUBB.Repository.EFEntities;

namespace TrackForUBB.Repository.DataSeeder;

public class GradesDataSeeder(AcademicAppContext context)
{
    private readonly AcademicAppContext _context = context;

    public async Task SeedAsync()
    {
        if (await _context.Grades.AnyAsync())
            return;

        var contracts = await _context.Contracts
            .Include(c => c.Enrollment)
            .Include(c => c.Semester)
            .Include(c => c.Subjects)
            .ToListAsync();

        if (contracts.Count == 0)
            return;

        var existingKeys = await _context.Grades
            .Select(g => new { g.EnrollmentId, g.SubjectId, g.SemesterId })
            .ToListAsync();

        var existingSet = new HashSet<(int EnrollmentId, int SubjectId, int SemesterId)>(
            existingKeys.Select(k => (k.EnrollmentId, k.SubjectId, k.SemesterId))
        );

        var rnd = new Random();
        var gradesToAdd = new List<Grade>();

        foreach (var contract in contracts)
        {
            if (contract.Enrollment == null || contract.Semester == null || contract.Subjects == null)
                continue;

            foreach (var subject in contract.Subjects)
            {
                if (subject == null)
                    continue;

                var key = (contract.Enrollment.Id, subject.Id, contract.Semester.Id);
                if (existingSet.Contains(key))
                    continue; // skip if a grade already exists for the same enrollment+subject+semester

                var value = rnd.Next(5, 11);

                gradesToAdd.Add(new Grade
                {
                    Enrollment = contract.Enrollment,
                    Semester = contract.Semester,
                    Subject = subject,
                    Value = value
                });

                existingSet.Add(key);
            }
        }

        if (gradesToAdd.Count > 0)
        {
            await _context.Grades.AddRangeAsync(gradesToAdd);
            await _context.SaveChangesAsync();
        }
    }
}
