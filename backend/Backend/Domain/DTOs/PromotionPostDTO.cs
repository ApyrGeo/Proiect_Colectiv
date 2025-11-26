namespace TrackForUBB.Domain.DTOs;

public class PromotionPostDTO
{
    public int StartYear { get; set; }
    public int EndYear { get; set; }
    public required int SpecialisationId { get; set; }
}