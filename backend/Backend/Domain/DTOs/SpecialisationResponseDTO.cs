namespace TrackForUBB.Domain.DTOs;

public class SpecialisationResponseDTO
{
    public required int Id { get; set; }
    public required string Name { get; set; }

    public List<PromotionResponseDTO> GroupYears { get; set; } = [];
}