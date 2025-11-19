using TrackForUBB.Repository.Context;
using TrackForUBB.Domain;
using Microsoft.EntityFrameworkCore;

namespace TrackForUBB.Service.DataSeeder;

public class UniversityDataSeeder(AcademicAppContext context)
{
    private readonly AcademicAppContext _context = context;

    public async Task SeedAsync()
    {
        // only populate empty db
        if (await _context.Faculties.AnyAsync())
            return;

        var faculties = new List<Faculty>
        {
            new() {Name = "Facultatea de Matematică și Informatică"},
            new() {Name = "Facultatea de Chimie"},
            new() {Name = "Facultatea de Fizică"},
            new() {Name = "Facultatea de Economie"},
            new() {Name = "Facultatea de Teatru"},
            new() {Name = "Facultatea de Drept"}
        };

        faculties.ForEach(f =>
        {
            var facultyCode = f.Name.Split()[^1][0];
            var languages = new[] { "Romana", "Engleza", "Germana", "Maghiara" };

            var specialisationNames = new List<string>();
            foreach (var language in languages)
            {
                specialisationNames.Add($"{f.Name.Split()[^1]} {language}");
            }

            int spec_nr = 1;
            f.Specialisations = [.. specialisationNames.Select(s => {
                var spec = new Specialisation
                {
                    Name = s,
                    Faculty = f
                };
                spec.GroupYears = GenerateYears(facultyCode, spec, spec_nr);
                spec_nr ++;
                return spec;
                })];
        });

        await _context.Faculties.AddRangeAsync(faculties);
        await _context.SaveChangesAsync();
    }

    private static List<GroupYear> GenerateYears(char facultyCode, Specialisation spec, int spec_nr)
    {
        var years = new List<GroupYear>();

        for (int yearNum = 1; yearNum <= 3; yearNum++)
        {
            string yearcode = $"{spec.Name.Split()[0][0]}{spec.Name.Split()[1][0]}{yearNum}";

            var year = new GroupYear()
            {
                Year = yearcode,
                Specialisation = spec
            };
            year.StudentGroups = GenerateGroups(spec_nr, yearNum, year);

            years.Add(year);
        }

        return years;
    }

    private static List<StudentGroup> GenerateGroups(int specialisationNr, int yearNum, GroupYear year)
    {
        var random = new Random();
        var groups = new List<StudentGroup>();
        int numGroups = random.Next(3, 8);

        for (int i = 1; i <= numGroups; i++)
        {
            string groupCode = $"{specialisationNr}{yearNum}{i}";

            var group = new StudentGroup
            {
                Name = groupCode,
                GroupYear = year
            };

            group.StudentSubGroups =
                [
                    new() {Name = $"{groupCode}-1", StudentGroup = group},
                    new() {Name = $"{groupCode}-2", StudentGroup = group}
                ];
            groups.Add(group);

        }
        return groups;
    }
}
