using Backend.Context;
using Backend.Domain;
using Bogus;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Backend.DataSeeder;

public class SubjectDataSeeder(AcademicAppContext context)
{
    private readonly AcademicAppContext _context = context;
    
    public async Task SeedAsync()
    {
        if (await _context.Subjects.AnyAsync())
            return;

        var groupYears = await _context.GroupYears
            .Include(gy => gy.Specialisation)
            .ThenInclude(s => s.Faculty)
            .ToListAsync();

        if (groupYears.Count == 0)
            return;

        var rand = new Random();

        //more to be added in the future
        var pools = new Dictionary<string, string[]>
        {
            ["Matematică și Informatică"] = [
                "Programare orientată pe obiecte", "Algoritmi și structuri de date",
                "Baze de date", "Analiză matematică", "Algebră liniară", "Inteligență artificială"
            ],
            ["Chimie"] = ["Chimie organică", "Chimie analitică", "Chimie fizică", "Laborator de chimie"],
            ["Fizică"] = ["Mecanică", "Fizică cuantică", "Fizică teoretică", "Laborator de fizică"],
            ["Economie"] = ["Microeconomie", "Macroeconomie", "Contabilitate", "Finanțe"],
            ["Teatru"] = ["Istoria teatrului", "Teatru practic", "Regie", "Teatrologie"],
            ["Drept"] = ["Drept civil", "Drept constituțional", "Drept penal", "Drept comercial"],
            ["generic"] = ["Introducere în domeniu", "Metodologie", "Proiect"]
        };

        var subjects = new List<Subject>();

        foreach (var gy in groupYears)
        {
            var facultyName = gy.Specialisation?.Faculty?.Name ?? "";
            var specName = gy.Specialisation?.Name ?? "";

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

            foreach (var subjName in chosenPool.Distinct())
            {
                var subj = new Subject
                {
                    Name = subjName,
                    NumberOfCredits = rand.Next(3, 9),
                    GroupYear = gy,
                    GroupYearId = gy.Id
                };
                subjects.Add(subj);
            }
        }

        await _context.Subjects.AddRangeAsync(subjects);
        await _context.SaveChangesAsync();
    }
}
