using TrackForUBB.Domain.Enums;

namespace TrackForUBB.Repository.EFEntities;

public class User
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required UserRole Role { get; set; }

    public List<Enrollment> Enrollments { get; set; } = [];
}
