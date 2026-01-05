using TrackForUBB.Domain.Enums;

namespace TrackForUBB.Repository.EFEntities;

public class Hour
{
    public int Id { get; set; }
    public required HourDay Day { get; set; }
    public required string HourInterval { get; set; }
    public required HourFrequency Frequency { get; set; }
    public required HourCategory Category { get; set; }

    public int SubjectId { get; set; }
    public required Subject Subject { get; set; }

    public int ClassroomId { get; set; }
    public required Classroom Classroom { get; set; }

    public int TeacherId { get; set; }
    public required Teacher Teacher { get; set; }

    public int? PromotionId { get; set; }
    public Promotion? Promotion { get; set; }

    public int? StudentGroupId { get; set; }
    public StudentGroup? StudentGroup { get; set; }

    public int? StudentSubGroupId { get; set; }
    public StudentSubGroup? StudentSubGroup { get; set; }
}
