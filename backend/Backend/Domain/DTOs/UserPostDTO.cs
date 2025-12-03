namespace TrackForUBB.Domain.DTOs;

public class UserPostDTO
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Role { get; set; }
    public string? SignatureBase64 { get; set; }
}
