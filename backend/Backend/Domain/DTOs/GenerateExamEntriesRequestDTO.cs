namespace TrackForUBB.Domain.DTOs;

public class GenerateExamEntriesRequestDTO
{
    public required int SubjectId { get; set; }
    public required List<int> StudentGroupIds { get; set; }
}
