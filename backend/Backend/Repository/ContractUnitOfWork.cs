using Microsoft.EntityFrameworkCore;
using TrackForUBB.Repository.Context;
using TrackForUBB.Service.Contracts.Models;
using TrackForUBB.Service.Interfaces;

namespace TrackForUBB.Repository;

public class ContractUnitOfWork(AcademicAppContext dbContext) : IContractUnitOfWork
{
    public async Task<List<ContractData>> GetData(int userId)
    {
        var enrollment = await dbContext.Enrollments
            .Include(x => x.SubGroup)
                .ThenInclude(x => x.StudentGroup)
                    .ThenInclude(x => x.GroupYear)
                        .ThenInclude(x => x.Specialisation)
                            .ThenInclude(x => x.Faculty)
            .Include(x => x.SubGroup)
                .ThenInclude(x => x.StudentGroup)
                    .ThenInclude(x => x.GroupYear)
                        .ThenInclude(x => x.Subjects)
            .Include(x => x.User)
            .Where(x => x.UserId == userId)
            .ToListAsync();

        return enrollment
            .Select(x =>
            {
                var specialisation = x.SubGroup.StudentGroup.GroupYear.Specialisation;
                var faculty = specialisation.Faculty;

                var subjects = x.SubGroup.StudentGroup.GroupYear.Subjects;
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

                return new ContractData()
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
            })
        .ToList();
    }
}
