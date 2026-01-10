namespace TrackForUBB.Domain.DTOs;

public class ExamEntryForStudentDTO
{
    public required string Specialisation { get; set; }
    public List<ExamEntryResponseDTO> ExamEntries { get; set; } = [];
}
