using TrackForUBB.Domain.DTOs;
using TrackForUBB.Repository.Interfaces;
using FluentValidation;
using TrackForUBB.Domain.Utils;

namespace TrackForUBB.Service.Validators;

public class SubjectPostDTOValidator : AbstractValidator<SubjectPostDTO>
{
    public SubjectPostDTOValidator(IAcademicRepository academicRepository)
    {
        RuleFor(f => f.Name)
            .NotEmpty().WithMessage("Subject name is required.")
            .MaximumLength(Constants.DefaultStringMaxLenght).WithMessage($"Subject name must not exceed {Constants.DefaultStringMaxLenght} characters.");

        RuleFor(f => f.NumberOfCredits)
            .NotNull().WithMessage("Nr credits is required.")
            .InclusiveBetween(1, 6).WithMessage("Nr credits must be between 1 and 6.");

        RuleFor(f => f.GroupYearId)
            .GreaterThan(0).WithMessage("GroupYearId must be a positive integer.")
            .MustAsync(async (groupYearId, cancellation) =>
            {
                var groupYear = await academicRepository.GetGroupYearByIdAsync(groupYearId);
                return groupYear != null;
            }).WithMessage("The specified GroupYearId does not exist.");
    }
}