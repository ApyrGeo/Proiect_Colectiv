using TrackForUBB.Repository.Context;
using Microsoft.EntityFrameworkCore;
using TrackForUBB.Repository.EFEntities;

namespace TrackForUBB.Repository.DataSeeder;

public class SubjectDataSeeder(AcademicAppContext context)
{
	private readonly AcademicAppContext _context = context;

	public async Task SeedAsync()
	{
		if (await _context.Subjects.AnyAsync())
			return;

		var specialisations = await _context.Specialisations
			.Include(s => s.Faculty)
			.ToListAsync();

		if (specialisations.Count == 0)
			return;

		var rand = new Random();

		var pools = new Dictionary<string, string[]>
		{
			["Matematică și Informatică"] = new[]
			{
				"Programare orientată pe obiecte", "Algoritmi și structuri de date",
				"Baze de date", "Analiză matematică", "Algebră liniară", "Inteligență artificială"
			},
			["Chimie"] = new[] { "Chimie organică", "Chimie analitică", "Chimie fizică", "Laborator de chimie" },
			["Fizică"] = new[] { "Mecanică", "Fizică cuantică", "Fizică teoretică", "Laborator de fizică" },
			["Economie"] = new[] { "Microeconomie", "Macroeconomie", "Contabilitate", "Finanțe" },
			["Teatru"] = new[] { "Istoria teatrului", "Teatru practic", "Regie", "Teatrologie" },
			["Drept"] = new[] { "Drept civil", "Drept constituțional", "Drept penal", "Drept comercial" },
			["generic"] = new[] { "Introducere în domeniu", "Metodologie", "Proiect" }
		};

		var subjectsToAdd = new List<Subject>();

		foreach (var spec in specialisations)
		{
			var facultyName = spec.Faculty?.Name ?? "";
			var specName = spec.Name ?? "";

			string[]? chosenPool = null;
			foreach (var key in pools.Keys.Where(k => k != "generic"))
			{
				if (facultyName.Contains(key, StringComparison.OrdinalIgnoreCase)
					|| specName.Contains(key, StringComparison.OrdinalIgnoreCase)
					|| specName.Split(' ').Any(t => key.Contains(t, StringComparison.OrdinalIgnoreCase)))
				{
					chosenPool = pools[key];
					break;
				}
			}
			chosenPool ??= pools["generic"];

			var allNames = chosenPool.Concat(pools["generic"]).Distinct();

			// create subjects for the specialisation (no semester)
			foreach (var name in allNames)
			{
				subjectsToAdd.Add(new Subject
				{
					Name = name,
					NumberOfCredits = rand.Next(3, 7),
					// If Subject has a SpecialisationId/Navigation, set it here:
					// Specialisation = spec, SpecialisationId = spec.Id
				});
			}
		}

		if (subjectsToAdd.Any())
		{
			await _context.Subjects.AddRangeAsync(subjectsToAdd);
			await _context.SaveChangesAsync();
		}
	}
}