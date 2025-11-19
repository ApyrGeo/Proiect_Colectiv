namespace TrackForUBB.Domain.DTOs;

public class LocationResponseDTO
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required GoogleMapsDataResponseDTO GoogleMapsData { get; set; }
}
