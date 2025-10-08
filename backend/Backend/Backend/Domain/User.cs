using Backend.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

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

    public List<Enrollment>? Enrollments { get; set; }
}
