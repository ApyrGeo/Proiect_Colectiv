namespace TrackForUBB.Domain.DTOs;

public class BulkUserCreateResultDTO
{
    public List<BulkUserCreateItemResultDTO> Items { get; set; } = [];
    public bool IsValid => Items.All(i => i.Success);
}
