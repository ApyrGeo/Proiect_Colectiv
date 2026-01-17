using FluentValidation;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Enums;
using TrackForUBB.Domain.Exceptions.Custom;
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

                if (existingUser != null && existingUser.Role != UserRole.Student)
                {
                    throw new EntityValidationException("The specified email does not belong to a student user.");
                }

                return existingUser != null;
            }).WithMessage("User with the specified email does not exist.");
    }
}
