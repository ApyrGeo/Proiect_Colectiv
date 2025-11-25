using TrackForUBB.Domain.DTOs;
using FluentValidation;
using TrackForUBB.Domain.Utils;
using TrackForUBB.Service.Interfaces;

namespace TrackForUBB.Service.Validators;

public class StudentGroupPostDTOValidator : AbstractValidator<StudentGroupPostDTO>
{
    public StudentGroupPostDTOValidator(IAcademicRepository academicRepository)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Student group name is required.")
            .MaximumLength(Constants.DefaultStringMaxLenght).WithMessage($"Student group name must not exceed {Constants.DefaultStringMaxLenght} characters.");

        RuleFor(x => x.GroupYearId)
            .GreaterThan(0).WithMessage("GroupYearId must be a positive integer.")
            .MustAsync(async (groupYearId, cancellation) =>
            {
                var groupYear = await academicRepository.GetPromotionByIdAsync(groupYearId);
                return groupYear != null;
            }).WithMessage("The specified GroupYearId does not exist.");
    }
}
