namespace TrackForUBB.Domain.DTOs;
public class BulkEnrollmentItemResultDTO
{
    public int Row { get; set; }
    public string? Email { get; set; }
    public bool IsValid { get; set; }
    public bool IsCreated => CreatedEnrollmentId.HasValue;
    public int? CreatedEnrollmentId { get; set; }
    public List<string> Errors { get; set; } = [];
}
