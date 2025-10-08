using Newtonsoft.Json;

namespace Backend.Domain.DTOs;

public class SpecialisationResponseDTO
{
    public required int Id { get; set; }
    public required string Name { get; set; }

    public List<GroupYearResponseDTO> GroupYears { get; set; } = [];
}