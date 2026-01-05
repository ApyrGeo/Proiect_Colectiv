using log4net;
using Microsoft.EntityFrameworkCore;
using TrackForUBB.Domain.Enums;
using TrackForUBB.Domain.Utils;
using TrackForUBB.Repository.Context;
using TrackForUBB.Repository.EFEntities;
using TrackForUBB.Service.Contracts.Models;
using TrackForUBB.Service.Interfaces;

namespace TrackForUBB.Repository;

public class ContractUnitOfWork(AcademicAppContext dbContext) : IContractUnitOfWork
{
    public async Task<ContractData> GetData(int userId, int promotionId, int yearNumber)
    {
        var promotion = await dbContext.Promotions
            .Include(x => x.Semesters)
            .Where(x => x.Id == promotionId)
            .SingleOrDefaultAsync();

        if (promotion is null)
            throw new Exception($"The promotion with id {promotionId} does not exist");

        if (promotion.Semesters.Count == 0)
            throw new Exception($"The promotion with id {promotionId} does not contain any semesters");
        var lastSemester = promotion.Semesters.Select(x => x.SemesterNumber).Max();
        var lastYear = lastSemester / 2;

        if (yearNumber < 1)
            throw new Exception($"The year number {yearNumber} is lower than 1");
        if (yearNumber > lastYear)
            throw new Exception($"The year number {yearNumber} is past the last year of the promotion with id {promotionId}");

        var contracts = await dbContext.Contracts
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
            .Where(x =>
                   x.Enrollment.UserId == userId
                   && (x.Semester.SemesterNumber + 1) / 2 == yearNumber
                   && x.Enrollment.SubGroup.StudentGroup.PromotionId == promotion.Id
            )
            .ToListAsync();
        if (contracts.Count == 0)
            throw new Exception($"There are no contracts for user {userId} and promotion {promotion.Id} and year {yearNumber}");
        logger.Info($"Pulled {contracts.Count} contracts for user {userId} promotion {promotion.Id} and year {yearNumber}");
        if (contracts.Count is not (1 or 2))
            throw new Exception($"There are too many contract for user {userId} and promotion {promotion.Id} and year {yearNumber}");
        var contract = contracts[0];

        var x = contract.Enrollment;

        var specialisation = x.SubGroup.StudentGroup.Promotion.Specialisation;
        var faculty = specialisation.Faculty;

        var student = x.User;

        var uniYear = promotion.StartYear + yearNumber - 1;

        var result = new ContractData()
        {
            FacultyName = faculty.Name,
            Domain = specialisation.Name,
            Specialization = specialisation.Name,
            Language = "TODO",
            StudentYear = yearNumber.ToString(),
            SubjectsSemester1 = [],
            SubjectsSemester2 = [],

            UniversityYear = $"{uniYear}-{uniYear + 1}",

            FullName = $"{student.FirstName} {student.LastName}",
            StudentId = student.Id.ToString(),
            IdCardSeries = "TODO",
            IdCardNumber = "TODO",
            CNP = "TODO",
            StudentPhone = student.PhoneNumber,
            StudentEmail = student.Email,
        };

        contracts
            .Select(x => x.Subjects.Select(MapSubject).ToList())
            .PopOrDefault(v => result.SubjectsSemester1 = v, [])
            .PopOrDefault(v => result.SubjectsSemester2 = v, [])
            ;

        return result;
    }

    private readonly ILog logger = LogManager.GetLogger(typeof(ContractUnitOfWork));

    private static ContractSubjectData MapSubject(Subject x) => new()
    {
        Code = x.SubjectCode,
        Type = FormatSubjectType(x.Type),
        Name = x.Name,
        Credits = x.NumberOfCredits,
    };

    private static string FormatSubjectType(SubjectType type) => type switch
    {
        SubjectType.Required => "obligatoriu",
        SubjectType.Optional => "opțional",
        SubjectType.Facultative => "facultativ",
        _ => throw new ArgumentOutOfRangeException(nameof(type)),
    };

}

file static class MyEnumExtensions
{
    public static IEnumerable<T> PopOrDefault<T>(this IEnumerable<T> enumerable, Action<T> popper, T defaultValue)
    {
        if (enumerable.Any())
        {
            popper(enumerable.First());
            return enumerable.Skip(1);
        }
        return enumerable;
    }
}
