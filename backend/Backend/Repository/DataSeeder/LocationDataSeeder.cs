using TrackForUBB.Repository.Context;
using Bogus;
using Microsoft.EntityFrameworkCore;
using TrackForUBB.Repository.EFEntities;

namespace TrackForUBB.Repository.DataSeeder;

public class LocationDataSeeder(AcademicAppContext context)
{
	private readonly AcademicAppContext _context = context;
	private static readonly Random _random = new Random(6767);

	public async Task SeedAsync()
	{
		if (await _context.Locations.AnyAsync())
			return;

		var locations = new List<Location>
		{
			new()
			{
				Name = "Facultatea de Stiințe Economice și Gestiunea Afacerii",
				Address = "Strada Teodor Mihali 58-60, Cluj-Napoca 400591",
				GoogleMapsData = new GoogleMapsData
				{
					Id = "Facultatea+de+Științe+Economice+și+Gestiunea+Afacerilor",
					Latitude = 46.7731036,
					Longitude = 23.6196481
				}
			},
			new()
			{
				Name = "Clădirea Centrală",
				Address = "Strada Mihail Kogălniceanu 1, Cluj-Napoca 400347",
				GoogleMapsData = new GoogleMapsData
				{
					Id = "Facultatea+De+Matematică+și+Informatică\u0083\r\n",
					Latitude = 46.7675665,
					Longitude = 23.5912619
				}
			},
			new()
			{
				Name = "Facultatea de Drept",
				Address = "Str. Avram Iancu nr. 11, Cluj-Napoca 400089",
				GoogleMapsData = new GoogleMapsData
				{
					Id = "Str.+Avram+Iancu+11",
					Latitude = 46.7664603,
					Longitude = 23.5891871
				}
			},
			new()
			{
				Name = "Facultatea de Litere",
				Address = "Strada Horea 31, Cluj-Napoca 400394",
				GoogleMapsData = new GoogleMapsData
				{
					Id = "UBB+Facultatea+de+Litere",
					Latitude = 46.7783176,
					Longitude = 23.5840111
				}
			},
			new()
			{
				Name = "Facultatea de Chimie",
				Address = "Strada Arany János 11, Cluj-Napoca 400028",
				GoogleMapsData = new GoogleMapsData
				{
					Id = "Facultatea+de+Chimie+și+Inginerie+Chimică",
					Latitude = 46.7707332,
					Longitude = 23.5796035
				}
			},
			new()
			{
				Name = "Facultatea de Teatru",
				Address = "Strada Mihail Kogălniceanu 4, Cluj-Napoca 400084",
				GoogleMapsData = new GoogleMapsData
				{
					Id = "Facultatea+de+Teatru+și+Film",
					Latitude = 46.7673405,
					Longitude = 23.5896027
				}
			},
			new()
			{
				Name = "Mathematica",
				Address = "Strada Ploiești 23-25, Cluj-Napoca 400157",
				GoogleMapsData = new GoogleMapsData
				{
					Id = "Mathematica",
					Latitude = 46.7765142,
					Longitude = 23.5902034
				}
			},
		};

		locations.ForEach(location =>
		{
			int no_rooms = _random.Next(30, 60);

			location.Classrooms = GenerateClassrooms(location, no_rooms);
		});

		await _context.Locations.AddRangeAsync(locations);
		await _context.SaveChangesAsync();
	}

	private static List<Classroom> GenerateClassrooms(Location location, int no_rooms)
	{
		var classroomFaker = new Faker<Classroom>("ro").UseSeed(6767);
		var no_stories = _random.Next(1, 7);

		int counter = 1;
		classroomFaker = classroomFaker
			.RuleFor(u => u.Name, f =>
			{
				string name = $"{f.Random.Number(1, no_stories)}{(counter % 100):D2}";
				counter++;
				return name;
			})
			.RuleFor(u => u.Location, _ => location);

		var classrooms = classroomFaker.Generate(no_rooms);

		return classrooms;
	}
}