using Backend.Domain.DTOs;
using Backend.Interfaces;
using Backend.Utils;
using FluentValidation;

namespace Backend.Service.Validators;

public class SubjectPostDTOValidator : AbstractValidator<SubjectPostDTO>
{
    public SubjectPostDTOValidator(ITimetableRepository repo, IAcademicRepository academicRepository)
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
                if (!groupYearId.HasValue) return false;

                var groupYear = await academicRepository.GetGroupYearByIdAsync(groupYearId.Value);
                return groupYear != null;
            }).WithMessage("The specified GroupYearId does not exist.");
    }
}