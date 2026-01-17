using TrackForUBB.Repository.Context;
using Microsoft.EntityFrameworkCore;
using TrackForUBB.Repository.EFEntities;

namespace TrackForUBB.Repository.DataSeeder;

public class SubjectDataSeeder(AcademicAppContext context)
{
	private readonly AcademicAppContext _context = context;
	private static readonly Random _random = new Random(6767);

	public async Task SeedAsync()
	{
		if (await _context.Subjects.AnyAsync())
			return;

		var faculties = await _context.Faculties
			.Include(f => f.Specialisations)
				.ThenInclude(s => s.Promotions)
                    .ThenInclude(y => y.Semesters)
			.ToListAsync();

		if (faculties.Count == 0)
			return;

		var pools = new Dictionary<string, List<string>>
		{
			["Matematică și Informatică"] = new()
			{
				// Year 1 subjects
				"Introducere în programare", "Analiză matematică I", "Algebră liniară", "Fizică", "Limba engleză",
				"Programare orientată pe obiecte", "Analiză matematică II", "Geometrie", "Logică matematică", "Metodologia cercetării",
				// Year 2 subjects
				"Algoritmi și structuri de date", "Baze de date", "Sisteme de operare", "Probabilități și statistică", "Metode numerice",
				"Programare avansată", "Arhitectura calculatoarelor", "Rețele de calculatoare", "Ingineria software", "Proiectare web",
				// Year 3 subjects
				"Inteligență artificială", "Compilatoare", "Învățare automată", "Securitate informatică", "Grafică pe calculator",
				"Cloud computing", "Baze de date avansate", "Programare mobilă", "Testare software", "Proiect de licență"
			},
			["Chimie"] = new()
			{
				"Chimie generală", "Chimie organică", "Chimie anorganică", "Chimie fizică", "Chimie analitică",
				"Laborator de chimie I", "Laborator de chimie II", "Chimie organică avansată", "Spectroscopie", "Electrochimie",
				"Chimia compușilor organici", "Chimia mediului", "Biochimie", "Chimie industrială", "Nanochimie",
				"Chimia polimerilor", "Cataliza chimică", "Chimia materialelor", "Chimie computațională", "Metode de analiză"
			},
			["Fizică"] = new()
			{
				"Fizică generală", "Mecanică", "Termodinamică", "Electricitate și magnetism", "Optică",
				"Fizică cuantică", "Fizică atomică", "Fizică nucleară", "Fizică statistică", "Mecanica fluidelor",
				"Fizică teoretică", "Fizica solidului", "Astrofizică", "Fizica plasmei", "Mecanica cuantică",
				"Relativitate", "Fizică moleculară", "Spectroscopie optică", "Fizica laserului", "Fizica particulelor"
			},
			["Economie"] = new()
			{
				"Microeconomie", "Macroeconomie", "Introducere în economie", "Matematici pentru economiști", "Statistică economică",
				"Contabilitate financiară", "Contabilitate de gestiune", "Finanțe publice", "Finanțe corporative", "Marketing",
				"Management", "Econometrie", "Economie internațională", "Drept comercial", "Analiză financiară",
				"Resurse umane", "Antreprenoriat", "Piețe financiare", "Banking", "Fiscalitate"
			},
			["Teatru"] = new()
			{
				"Istoria teatrului", "Teoria dramei", "Actorie I", "Actorie II", "Regie teatrală",
				"Scenografie", "Teatru practic", "Teatrologie", "Dramaturgie", "Interpretare",
				"Teatru contemporan", "Teatru experimental", "Tehnici de joc", "Mișcare scenică", "Voce și dicție",
				"Regie avansată", "Teatru și societate", "Critique dramatică", "Performance art", "Teatru muzical"
			},
			["Drept"] = new()
			{
				"Drept constituțional", "Drept civil I", "Drept civil II", "Drept penal I", "Drept penal II",
				"Drept administrativ", "Drept comercial", "Drept procesual civil", "Drept procesual penal", "Dreptul muncii",
				"Drept fiscal", "Drept internațional public", "Drept internațional privat", "Drept european", "Dreptul proprietății intelectuale",
				"Drept financiar", "Dreptul consumatorului", "Dreptul mediului", "Arbitraj și mediere", "Etică juridică"
			}
		};

		var subjectsToAdd = new List<Subject>();

		// For each faculty, generate subjects per year per semester
		foreach (var faculty in faculties)
		{
			string[]? facultyPool = null;
			foreach (var key in pools.Keys)
			{
				if (faculty.Name.Contains(key, StringComparison.OrdinalIgnoreCase))
				{
					facultyPool = pools[key].ToArray();
					break;
				}
			}

			if (facultyPool == null || facultyPool.Length == 0)
				continue;

			// Get all unique semesters across all promotions in this faculty's specialisations
			var allSemesters = faculty.Specialisations
				.SelectMany(s => s.Promotions)
				.SelectMany(y => y.Semesters)
				.GroupBy(ps => ps.SemesterNumber)
				.Order()
				.ToList();

			// Track used subject names to avoid duplicates
			var usedSubjects = new HashSet<string>();
			var availableSubjects = facultyPool.ToList();

			// For each year-semester combination
			foreach (var semesterKey in allSemesters)
			{
				// Pick 5 unique subjects for this semester
				var semesterSubjects = new List<string>();
				var attempts = 0;

				while (semesterSubjects.Count < 5 && attempts < 100)
				{
					attempts++;

					// If we've run out of unused subjects, reset the pool
					if (availableSubjects.Count == 0)
					{
						availableSubjects = facultyPool.Where(s => !usedSubjects.Contains(s)).ToList();
						if (availableSubjects.Count == 0)
						{
							// All subjects used, reset completely
							usedSubjects.Clear();
							availableSubjects = facultyPool.ToList();
						}
					}

					var subject = availableSubjects[_random.Next(availableSubjects.Count)];
					availableSubjects.Remove(subject);

					if (!usedSubjects.Contains(subject))
					{
						semesterSubjects.Add(subject);
						usedSubjects.Add(subject);
					}
				}

				// Create subject entities
				foreach (var subjectName in semesterSubjects)
				{
					// Only add if not already in the list (avoid global duplicates)
					if (!subjectsToAdd.Any(s => s.Name == subjectName))
					{
                        // TODO_SEBI
                        /*
						subjectsToAdd.Add(new Subject
						{
                            HolderTeacher = null,
                            Name = subjectName,
							NumberOfCredits = _random.Next(3, 7)
						});
                        */
					}
				}
			}
		}

		if (subjectsToAdd.Any())
		{
			await _context.Subjects.AddRangeAsync(subjectsToAdd);
			await _context.SaveChangesAsync();
		}
	}
}
