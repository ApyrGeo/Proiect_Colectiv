namespace TrackForUBB.Domain.DTOs;

public class BulkUserResultDTO
{
    public List<BulkUserItemResultDTO> Users { get; set; } = [];
    public bool IsValid => Users.All(i => i.IsValid);
}
