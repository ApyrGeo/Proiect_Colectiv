using Microsoft.EntityFrameworkCore;
using TrackForUBB.Repository.Context;
using TrackForUBB.Service.Contracts.Models;
using TrackForUBB.Service.Interfaces;

namespace TrackForUBB.Repository;

public class ContractUnitOfWork(AcademicAppContext dbContext) : IContractUnitOfWork
{
    public async Task<List<ContractData>> GetData(int userId)
    {
		//TODO: get the semester number from date
		int yearNumber = 1;

        var contract = await dbContext.Contracts
            .Include(x => x.Subjects)
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.SubGroup)
                    .ThenInclude(x => x.StudentGroup)
                        .ThenInclude(x => x.Promotion)
                            .ThenInclude(x => x.Specialisation)
                                .ThenInclude(x => x.Faculty)
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.User)
            .Include(x => x.Semester)
                .ThenInclude(x => x.PromotionYear)
			.Where(x => x.Enrollment.UserId == userId && x.Semester.PromotionYear.YearNumber == yearNumber)
            .FirstOrDefaultAsync() ?? throw new Exception("Contract not found");

        var x = contract.Enrollment;

        var specialisation = x.SubGroup.StudentGroup.Promotion.Specialisation;
        var faculty = specialisation.Faculty;

        var subjects = contract.Subjects;
        var student = x.User;

        var semester1Data = subjects.Select(
            x => new ContractSubjectData()
            {
                Code = x.Id.ToString(),
                Type = "TODO",
                Name = x.Name,
                Credits = x.NumberOfCredits,
            }).ToList();
        // TODO
        var semester2Data = semester1Data.ToList();
        semester2Data.Reverse();

        var r = new ContractData()
        {
            FacultyName = faculty.Name,
            Domain = specialisation.Name,
            Specialization = specialisation.Name,
            Language = "TODO",
            StudentYear = "TODO",
            SubjectsSemester1 = semester1Data,
            SubjectsSemester2 = semester2Data,

            FullName = $"{student.FirstName} {student.LastName}",
            StudentId = student.Id.ToString(),
            IdCardSeries = "TODO",
            IdCardNumber = "TODO",
            CNP = "TODO",
            StudentPhone = student.PhoneNumber,
            StudentEmail = student.Email,
        };
        return [ r ];
    }
}
