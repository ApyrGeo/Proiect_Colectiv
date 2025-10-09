using Backend.Domain.DTOs;
using Backend.Interfaces;
using FluentValidation;

namespace Backend.Service.Validators;

public class UserPostDTOValidator : AbstractValidator<UserPostDTO>
{
    private readonly IUserRepository _repository;
    public UserPostDTOValidator(IUserRepository repository)
    {
        _repository = repository;

        RuleFor(user => user.FirstName)
            .NotNull()
            .NotEmpty()
            .WithMessage("First name is required.")
            .Matches("^[-'\\p{L}]+( [-'\\p{L}]+)*$")
            .WithMessage("Invalid first name format.");

        RuleFor(user => user.LastName)
            .NotNull()
            .NotEmpty()
            .WithMessage("Last name is required.")
            .Matches("^[-'\\p{L}]+( [-'\\p{L}]+)*$")
            .WithMessage("Invalid last name format.");

        RuleFor(user => user.PhoneNumber)
            .NotNull()
            .NotEmpty()
            .WithMessage("Phone number is required.")
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .WithMessage("Invalid phone number format.");

        RuleFor(user => user.Email)
            .NotNull()
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .MustAsync(async (email, cancellation) =>
            {
                var existingUser = await _repository.GetByEmailAsync(email);
                return existingUser == null;
            })
            .WithMessage("Email already exists.");

        RuleFor(user => user.Password)
            .NotNull()
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.");
            
        RuleFor(user => user.Role)
            .NotNull()
            .NotEmpty()
            .WithMessage("Role is required.")
            .Must(role => role == "Admin" || role == "Teacher" || role == "Student")
            .WithMessage("Role must be either 'Admin', 'Student' or 'Teacher'.");
    }
}
