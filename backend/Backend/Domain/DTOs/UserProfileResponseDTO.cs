namespace TrackForUBB.Domain.DTOs;

public class UserProfileResponseDTO
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Owner { get; set; }
    public required string PhoneNumber { get; set; }
    public string? SignatureUrl { get; set; }
}