using System.Diagnostics.CodeAnalysis;
using Backend.Domain.DTOs;
using Backend.Interfaces;
using FluentValidation;
using Backend.Domain.Enums;

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
                var user = await userRepository.GetByIdAsync(userId);
                return user?.Role == UserRole.Student;
            }).WithMessage("User with the specified UserId does not exist.");

        RuleFor(e => e.SubGroupId)
            .NotNull().WithMessage("SubGroupId cannot be null.")
            .GreaterThan(0).WithMessage("SubGroupId must be a positive integer.")
            .MustAsync(async (subGroupId, cancellation) =>
            {   
                var subGroup = await academicRepository.GetSubGroupByIdAsync(subGroupId);
                return subGroup != null;
            }).WithMessage("StudentSubGroup with the specified SubGroupId does not exist.");
    }
}
