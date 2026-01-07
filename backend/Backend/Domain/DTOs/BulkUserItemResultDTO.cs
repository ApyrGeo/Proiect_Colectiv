namespace TrackForUBB.Domain.DTOs;

public class BulkUserItemResultDTO
{
    public int Row { get; set; }
    public string? Email { get; set; }
    public bool IsValid { get; set; }
    public bool IsCreated => CreatedUserId.HasValue;
    public int? CreatedUserId { get; set; }
    public List<string> Errors { get; set; } = [];
}
