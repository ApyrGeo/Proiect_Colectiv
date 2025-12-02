namespace TrackForUBB.Domain.DTOs;

public class PromotionSemesterResponseDTO
{   
    public required int Id { get; set; }
    public required int SemesterNumber { get; set; }
    public required PromotionYearResponseDTO PromotionYear { get; set; }
}