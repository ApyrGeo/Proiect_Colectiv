using TrackForUBB.Repository.Context;
using TrackForUBB.Domain;
using Bogus;
using Microsoft.EntityFrameworkCore;

namespace TrackForUBB.Service.DataSeeder;

public class LocationDataSeeder(AcademicAppContext context)
{
    private readonly AcademicAppContext _context = context;

    public async Task SeedAsync()
    {
        if (await _context.Locations.AnyAsync())
            return;

        var locations = new List<Location>
        {
            new() {Name = "Facultatea de Stiințe Economice și Gestiunea Afacerii", Address = "Strada Teodor Mihali 58-60, Cluj-Napoca 400591"},
            new() {Name = "Clădirea Centrală", Address = "Strada Mihail Kogălniceanu 1, Cluj-Napoca 400347"},
            new() {Name = "Facultatea de Drept", Address = "Str. Avram Iancu nr. 11, Cluj-Napoca 400089"},
            new() {Name = "Facultatea de Litere", Address = "Strada Horea 31, Cluj-Napoca 400394"},
            new() {Name = "Facultatea de Chimie", Address = "Strada Arany János 11, Cluj-Napoca 400028"},
            new() {Name = "Facultatea de Teatru", Address = "Strada Mihail Kogălniceanu 4, Cluj-Napoca 400084"},
            new() {Name = "Mathematica", Address = "Strada Ploiești 23-25, Cluj-Napoca 400157"},
        };

        locations.ForEach(location =>
        {
            var random = new Random();
            int no_rooms = random.Next(1, 30 + 1);

            location.Classrooms = GenerateClassrooms(location, no_rooms);
        });

        await _context.Locations.AddRangeAsync(locations);
        await _context.SaveChangesAsync();
    }

    private static List<Classroom> GenerateClassrooms(Location location, int no_rooms)
    {
        var random = new Random();
        var classroomFaker = new Faker<Classroom>("ro").UseSeed(6767);
        var no_stories = random.Next(1, 7);

        classroomFaker = classroomFaker
            .RuleFor(u => u.Name, f => $"{f.Random.Number(1, no_stories)}{f.UniqueIndex % 100:D2}")
            .RuleFor(u => u.Location, _ => location);

        var classrooms = classroomFaker.Generate(no_rooms);

        return classrooms;
    }
}
