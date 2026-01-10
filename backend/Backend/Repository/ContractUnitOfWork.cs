using System.Text.Json;
using log4net;
using Microsoft.EntityFrameworkCore;
using TrackForUBB.Domain.Enums;
using TrackForUBB.Repository.Context;
using TrackForUBB.Repository.EFEntities;
using TrackForUBB.Service.Contracts.Models;
using TrackForUBB.Service.Interfaces;

namespace TrackForUBB.Repository;

public class ContractUnitOfWork(AcademicAppContext dbContext) : IContractUnitOfWork
{
    public async Task<ContractData> GetData(ContractRequestModel request)
    {
        var promotionId = request.PromotionId;
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

        var yearNumber = request.Year;
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
                   x.Enrollment.UserId == request.UserId
                   && (x.Semester.SemesterNumber + 1) / 2 == yearNumber
                   && x.Enrollment.SubGroup.StudentGroup.PromotionId == promotion.Id
            )
            .ToListAsync();
        if (contracts.Count is not (0 or 1 or 2))
            throw new Exception($"There are too many contract for user {request.UserId} and promotion {promotion.Id} and year {yearNumber}");

        var contractFirstSemester = await GetContract(request, promotion, contracts, request.Year * 2 - 1, request.OptionalToSubjectCodesSem1);
        var contractSecondSemester = await GetContract(request, promotion, contracts, request.Year * 2, request.OptionalToSubjectCodesSem2);
        await dbContext.SaveChangesAsync();

        var x = contractFirstSemester.Enrollment;

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

        result.SubjectsSemester1 = contractFirstSemester.Subjects.Select(MapSubject).ToList();
        result.SubjectsSemester2 = contractSecondSemester.Subjects.Select(MapSubject).ToList();

        return result;
    }

    private async ValueTask<Contract> GetContract(ContractRequestModel request, Promotion promotion, List<Contract> contracts, int semesterNumber, IDictionary<int, int> optionalToSubjectCodes)
    {
        var semester = promotion.Semesters.Single(x => x.SemesterNumber == semesterNumber);

        var contract = contracts.FirstOrDefault(x => x.SemesterId == semester.Id);

        var subjectsInSemester = await dbContext.Subjects
            .Where(x => x.Semester.PromotionId == promotion.Id
                    && x.SemesterId == semester.Id)
            .ToListAsync();

        var inexitentOptionals = optionalToSubjectCodes
            .Values
            .Where(x => !subjectsInSemester.Select(x => x.Id).Contains(x))
            .ToList();
        if (inexitentOptionals.Count != 0)
            throw new Exception($"The optional subjects {JsonSerializer.Serialize(inexitentOptionals)} do not exist in semester {semester.Id}");

        logger.InfoFormat(">>>>>>>>>>>>>>>>> semesterNumber {0}", semesterNumber);
        logger.Info(">>>>>>>>>>>>>>>>> subjectsInSemster");
        logger.Info(JsonSerializer.Serialize(subjectsInSemester.Select(x => new { x.Name, x.Id })));
        logger.Info(">>>>>>>>>>>>>>>>> optionals");
        logger.Info(JsonSerializer.Serialize(optionalToSubjectCodes));

        if (contract is null)
        {
            var enrollments = await dbContext.Enrollments
                .Include(x => x.SubGroup)
                    .ThenInclude(x => x.StudentGroup)
                        .ThenInclude(x => x.Promotion)
                            .ThenInclude(x => x.Specialisation)
                                .ThenInclude(x => x.Faculty)
                .Include(x => x.User)
                .Where(x => x.UserId == request.UserId && x.SubGroup.StudentGroup.PromotionId == promotion.Id)
                .ToListAsync();

            if (enrollments.Count is 0)
                throw new Exception($"User with id {request.UserId} is not enrolled in promotion {promotion.Id}");
            if (enrollments.Count is > 1)
                throw new Exception($"User with id {request.UserId} is enrolled multiple times in promotion {promotion.Id}");
            var enrollment = enrollments.Single();

            var newContract = new Contract()
            {
                Semester = semester,
                SemesterId = semester.Id,
                Subjects = [],
                EnrollmentId = enrollment.Id,
                Enrollment = enrollment,
            };

            dbContext.Add(newContract);
            contract = newContract;
        }
        
        var subjects = subjectsInSemester
            .Where(x => x.Type == SubjectType.Required
                   || optionalToSubjectCodes.Values.Contains(x.Id))
            .ToList();

        contract.Subjects = subjects;
        return contract;
    }

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


    private readonly ILog logger = LogManager.GetLogger(typeof(ContractUnitOfWork));
}
