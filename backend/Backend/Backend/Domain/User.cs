using Backend.Domain.Enums;

namespace Backend.Domain;

public class User
{
    public required int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }   
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required UserRole Role { get; set; }

    public List<Enrollment> Enrollments { get; set; } = [];
    public List<Teacher> Teachers { get; set; } = [];
}
