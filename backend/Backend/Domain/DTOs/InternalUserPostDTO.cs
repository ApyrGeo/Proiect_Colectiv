namespace TrackForUBB.Domain.DTOs;

public class InternalUserPostDTO
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public string? Owner { get; set; }
    public string? TenantEmail { get; set; }
    public string? SignatureBase64 { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is InternalUserPostDTO dTO &&
               FirstName == dTO.FirstName &&
               LastName == dTO.LastName &&
               PhoneNumber == dTO.PhoneNumber &&
               Email == dTO.Email &&
               Role == dTO.Role &&
               Owner == dTO.Owner &&
               TenantEmail == dTO.TenantEmail &&
               SignatureBase64 == dTO.SignatureBase64;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            FirstName,
            LastName,
            PhoneNumber,
            Email,
            Role,
            Owner,
            TenantEmail,
            SignatureBase64
        );
    }
}
