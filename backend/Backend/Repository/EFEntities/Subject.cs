namespace TrackForUBB.Repository.EFEntities;

public class Subject
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required int NumberOfCredits { get; set; }

    public List<Hour> Hours { get; set; } = [];
    public List<Grade> Grades { get; set; } = [];
    
    public List<Contract> Contracts { get; set; } = [];

    public int? HolderTeacherId { get; set; }
    public Teacher? HolderTeacher { get; set; }
    public List<ExamEntry> RegisteredExams { get; set; } = [];
}