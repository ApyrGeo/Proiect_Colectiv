namespace TrackForUBB.Domain.DTOs;

public class BulkPromotionResultDTO
{
    public PromotionResponseDTO? Promotion { get; set; }
    public List<BulkEnrollmentItemResultDTO> Enrollments { get; set; } = [];
    public bool IsValid => Enrollments.All(i => i.IsValid);
}
