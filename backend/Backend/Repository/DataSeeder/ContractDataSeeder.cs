using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackForUBB.Domain.Enums;
using TrackForUBB.Repository.Context;
using TrackForUBB.Repository.EFEntities;

namespace TrackForUBB.Repository.DataSeeder;

public class ContractDataSeeder(AcademicAppContext context)
{
	private readonly AcademicAppContext _context = context;

	public async Task SeedAsync()
	{
		if (_context.Contracts.Any())
			return;

		var subjects = await _context.Subjects.ToListAsync();
		var enrollments = await _context.Enrollments.ToListAsync();
		var semesters = await _context.PromotionSemesters.ToListAsync();

		var selectedEnrollments = new List<Enrollment>();
		var random = new Random();
		var randomNrEnrollments = random.Next(30, 450);
		for (var i = 0; i < randomNrEnrollments; i++)
			selectedEnrollments.Add(enrollments[random.Next(enrollments.Count)]);

		var contracts = new List<Contract>();

		foreach (var enrollment in selectedEnrollments)
		{
			foreach (var semester in semesters)
			{
				var selectedSubjects = new List<Subject>();
				var random2 = new Random();
				var nrSubjects = random2.Next(1, 20);
				for (var j = 0; j < nrSubjects; j++) 
					selectedSubjects.Add(subjects[random2.Next(subjects.Count)]);


				var contract = new Contract
				{
					Semester = semester,
					Enrollment = enrollment,
					Subjects = [.. selectedSubjects.Distinct()]
                };

				contracts.Add(contract);
			}
		}

		await _context.Contracts.AddRangeAsync(contracts);
		await _context.SaveChangesAsync();
	}
}