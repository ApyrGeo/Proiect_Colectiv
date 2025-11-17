namespace Backend.Domain.DTOs;

public class LocationResponseDTO
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required GoogleMapsData GoogleMapsData { get; set; }
}
