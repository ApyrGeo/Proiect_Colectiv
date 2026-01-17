using TrackForUBB.Domain.Enums;

namespace TrackForUBB.Domain.DTOs;

public class UserResponseDTO
{
    public required int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }
    public required string Owner { get; set; }
    public required string TenantEmail { get; set; }
    public required UserRole Role { get; set; }
}
