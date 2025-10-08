namespace Backend.Domain.DTOs;

public class GroupYearPostDTO
{
    public required string Year { get; set; } 
    public required int SpecialisationId { get; set; }
}