using TrackForUBB.Domain.Enums;

namespace TrackForUBB.Repository.EFEntities;

public class Subject
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required int NumberOfCredits { get; set; }

    public List<Hour> Hours { get; set; } = [];
    public List<Grade> Grades { get; set; } = [];
    
    public List<Contract> Contracts { get; set; } = [];

    public int HolderTeacherId { get; set; }
    public required Teacher HolderTeacher { get; set; }
    public List<ExamEntry> RegisteredExams { get; set; } = [];

    public int SemesterId { get; set; }
    public required PromotionSemester Semester { get; set; }

    public SubjectType Type { get; set; }

    public int? OptionalPackage { get; set; }

    public required string SubjectCode { get; set; }
}
