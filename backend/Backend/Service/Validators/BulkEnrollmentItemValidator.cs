using FluentValidation;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Utils;
using TrackForUBB.Service.Interfaces;

namespace TrackForUBB.Service.Validators;

public class BulkEnrollmentItemValidator : AbstractValidator<BulkEnrollmentItem>
{
    public BulkEnrollmentItemValidator(IUserRepository userRepository)
    {
        RuleFor(x => x.UserEmail)
            .NotNull().WithMessage("User email is required.")
            .NotEmpty().WithMessage("User email must not be empty.")
            .MaximumLength(Constants.DefaultStringMaxLenght).WithMessage($"User email cannot exceed {Constants.DefaultStringMaxLenght} characters.")
            .MustAsync(async (email, cancellation) =>
            {
                var existingUser = await userRepository.GetByEmailAsync(email);
                return existingUser != null;
            }).WithMessage("User with the specified email does not exist.");
    }
}
