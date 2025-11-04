using System.Diagnostics.CodeAnalysis;
using Backend.Domain.DTOs;
using Backend.Interfaces;
using FluentValidation;

namespace Backend.Service.Validators;

public class EnrollmentPostDTOValidator : AbstractValidator<EnrollmentPostDTO>
{
    public EnrollmentPostDTOValidator(IUserRepository userRepository, IAcademicRepository academicRepository)
    {
        RuleFor(e => e.UserId)
            .NotNull().WithMessage("UserId cannot be null.")
            .GreaterThan(0).WithMessage("UserId must be a positive integer.")
            .MustAsync(async (userId, cancellation) =>
            {   
                if (!userId.HasValue)
                    return true;
                
                var user = await userRepository.GetByIdAsync(userId.Value);
                return user != null;
            }).WithMessage("User with the specified UserId does not exist.");

        RuleFor(e => e.SubGroupId)
            .NotNull().WithMessage("SubGroupId cannot be null.")
            .GreaterThan(0).WithMessage("SubGroupId must be a positive integer.")
            .MustAsync(async (subGroupId, cancellation) =>
            {   
                if (!subGroupId.HasValue)
                    return true;
                var subGroup = await academicRepository.GetSubGroupByIdAsync(subGroupId.Value);
                return subGroup != null;
            }).WithMessage("StudentSubGroup with the specified SubGroupId does not exist.");
    }
}
