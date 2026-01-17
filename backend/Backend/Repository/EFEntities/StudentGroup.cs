namespace TrackForUBB.Repository.EFEntities;

public class StudentGroup
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<StudentSubGroup> StudentSubGroups { get; set; } = [];
    public int PromotionId { get; set; }
    public required Promotion Promotion { get; set; }

    public List<ExamEntry> RegisteredExams { get; set; } = [];
}