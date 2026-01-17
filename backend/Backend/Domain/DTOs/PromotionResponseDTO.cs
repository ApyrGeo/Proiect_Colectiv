namespace TrackForUBB.Domain.DTOs;

public class PromotionResponseDTO
{
    public required int Id { get; set; }
    public required int StartYear { get; set; }
    public required int EndYear { get; set; }

    public required string PrettyName { get; set; }


    public List<StudentGroupResponseDTO> StudentGroups { get; set; } = [];
    
    public List<PromotionSemesterResponseDTO> Semesters { get; set; } = [];
}
