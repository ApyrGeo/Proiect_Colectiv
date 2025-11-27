using TrackForUBB.Domain.DTOs;
using FluentValidation;
using TrackForUBB.Domain.Enums;
using TrackForUBB.Service.Interfaces;

namespace TrackForUBB.Service.Validators;

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
                return user?.Role == nameof(UserRole.Student);
            }).WithMessage("User with the specified UserId does not exist.");

        RuleFor(e => e.SubGroupId)
            .NotNull().WithMessage("SubGroupId cannot be null.")
            .GreaterThan(0).WithMessage("SubGroupId must be a positive integer.")
            .MustAsync(async (subGroupId, cancellation) =>
            {
                var subGroup = await academicRepository.GetSubGroupByIdAsync(subGroupId);
                return subGroup != null;
            }).WithMessage("StudentSubGroup with the specified SubGroupId does not exist.");

		RuleFor(e => e)
	        .MustAsync(async (dto, cancellation) =>
	        {
		        var enrollments = await academicRepository.GetEnrollmentsByUserId(dto.UserId);
		        return !enrollments.Any(enrollment => enrollment.SubGroupId == dto.SubGroupId);
	        }).WithMessage("The user is already enrolled in the specified StudentSubGroup.");
	}
}
