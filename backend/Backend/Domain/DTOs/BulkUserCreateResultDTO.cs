namespace TrackForUBB.Domain.DTOs;

public class BulkUserCreateResultDTO
{
    public List<BulkUserCreateItemResultDTO> Users { get; set; } = [];
    public bool IsValid => Users.All(i => i.IsValid);
}
