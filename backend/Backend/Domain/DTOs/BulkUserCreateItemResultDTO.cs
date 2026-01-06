namespace TrackForUBB.Domain.DTOs;

public class BulkUserCreateItemResultDTO
{
    public int Row { get; set; }
    public string? Email { get; set; }
    public bool Success { get; set; }
    public int? CreatedUserId { get; set; }
    public List<string> Errors { get; set; } = [];
}
