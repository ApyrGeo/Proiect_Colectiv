namespace TrackForUBB.Domain.DTOs;
public class GoogleMapsDataResponseDTO
{
    public required string Id { get; set; }
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
}
