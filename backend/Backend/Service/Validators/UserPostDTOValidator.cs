using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Enums;
using TrackForUBB.Repository.Interfaces;
using FluentValidation;
using TrackForUBB.Domain.Utils;

namespace TrackForUBB.Service.Validators;

public class UserPostDTOValidator : AbstractValidator<UserPostDTO>
{
    private readonly IUserRepository _repository;
    public UserPostDTOValidator(IUserRepository repository)
    {
        _repository = repository;

        RuleFor(user => user.FirstName)
            .NotNull()
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(Constants.DefaultStringMaxLenght).WithMessage($"User first name must not exceed {Constants.DefaultStringMaxLenght} characters.")
            .Matches("^[-'\\p{L}]+( [-'\\p{L}]+)*$").WithMessage("Invalid first name format.");

        RuleFor(user => user.LastName)
            .NotNull()
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(Constants.DefaultStringMaxLenght).WithMessage($"User last name must not exceed {Constants.DefaultStringMaxLenght} characters.")
            .Matches("^[-'\\p{L}]+( [-'\\p{L}]+)*$").WithMessage("Invalid last name format.");

        RuleFor(user => user.PhoneNumber)
            .NotNull()
            .NotEmpty().WithMessage("Phone number is required.")
            .MaximumLength(Constants.DefaultStringMaxLenght).WithMessage($"User phone number must not exceed {Constants.DefaultStringMaxLenght} characters.")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");

        RuleFor(user => user.Email)
            .NotNull()
            .NotEmpty().WithMessage("Email is required.")
            .MaximumLength(Constants.DefaultStringMaxLenght).WithMessage($"User email must not exceed {Constants.DefaultStringMaxLenght} characters.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MustAsync(async (email, cancellation) =>
            {
                var existingUser = await _repository.GetByEmailAsync(email);
                return existingUser == null;
            }).WithMessage("Email already exists.");

        RuleFor(user => user.Password)
            .NotNull()
            .NotEmpty().WithMessage("Password is required.")
            .MaximumLength(Constants.ExtendedStringMaxLenght).WithMessage($"User password must not exceed {Constants.ExtendedStringMaxLenght} characters.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");

        RuleFor(user => user.Role)
            .NotNull()
            .NotEmpty().WithMessage("Role is required.")
            .IsEnumName(typeof(UserRole)).WithMessage($"User role string cannot be converted to enum, available values: {string.Join(", ", Enum.GetNames(typeof(UserRole)))}.");
    }
}
