using TrackForUBB.Repository.Context;
using Microsoft.EntityFrameworkCore;
using TrackForUBB.Repository.EFEntities;

namespace TrackForUBB.Repository.DataSeeder;

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

			var specs = new List<Specialisation>();
			int specNr = 1;
			foreach (var specName in specialisationNames)
			{
				var spec = new Specialisation
				{
					Name = specName,
					Faculty = f
				};
				spec.Promotions = GeneratePromotions(facultyCode, spec, specNr);
				specNr++;
				specs.Add(spec);
			}

			f.Specialisations = specs;
		});

		await _context.Faculties.AddRangeAsync(faculties);
        await _context.SaveChangesAsync();
    }

    private static List<Promotion> GeneratePromotions(char facultyCode, Specialisation spec, int spec_nr)
    {
        var promotions = new List<Promotion>();
        for (int promoNum = 1; promoNum <= 3; promoNum++)
        {
            string promocode = $"{facultyCode}{spec.Name.Split()[0][0]}{spec.Name.Split()[1][0]}{promoNum}";
            var promotion = new Promotion()
            {
                StartYear = 2023 + promoNum - 1,
                EndYear = 2026 + promoNum - 1,
                Specialisation = spec
            };
            promotion.StudentGroups = GenerateGroups(spec_nr, (DateTime.Now.Year - promotion.StartYear + (DateTime.Now.Month < 7 ? 0 : 1)), promotion);
            promotion.Years = GenerateYears(promotion);
			promotions.Add(promotion);
        }
        return promotions;
	}

	private static List<PromotionYear> GenerateYears(Promotion promotion)
	{
		var years = new List<PromotionYear>();
		for (int i = 1; i <= 3; i++)
		{
			var year = new PromotionYear
			{
				YearNumber = i,
				Promotion = promotion
			};
			year.PromotionSemesters = new List<PromotionSemester>
		{
			new PromotionSemester { SemesterNumber = 1, PromotionYear = year },
			new PromotionSemester { SemesterNumber = 2, PromotionYear = year }
		};
			years.Add(year);
		}
		return years;
	}

	private static List<StudentGroup> GenerateGroups(int specialisationNr, int yearNum, Promotion promotion)
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
				Promotion = promotion
			};

			group.StudentSubGroups = new List<StudentSubGroup>
		{
			new StudentSubGroup { Name = $"{groupCode}-1", StudentGroup = group },
			new StudentSubGroup { Name = $"{groupCode}-2", StudentGroup = group }
		};

			groups.Add(group);
		}
		return groups;
	}
}
