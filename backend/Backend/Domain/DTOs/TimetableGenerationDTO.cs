namespace TrackForUBB.Domain.DTOs;

public class TimetableGenerationDTO
{
    public required int SpecialisationId { get; set; }
    public required int Year { get; set; }
    public required int Semester { get; set; }
}
