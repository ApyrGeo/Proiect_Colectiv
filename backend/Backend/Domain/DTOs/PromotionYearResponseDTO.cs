namespace TrackForUBB.Domain.DTOs;

public class PromotionYearResponseDTO
{
    public required int Id { get; set; }
    public required int YearNumber { get; set; }
    public required PromotionResponseDTO Promotion { get; set; }
}