using TrackForUBB.Domain.DTOs;
using FluentValidation;
using TrackForUBB.Domain.Utils;
using TrackForUBB.Service.Interfaces;

namespace TrackForUBB.Service.Validators;

public class GroupYearPostDTOValidator : AbstractValidator<PromotionPostDTO>
{
    public GroupYearPostDTOValidator(IAcademicRepository academicRepository)
    {
        RuleFor(g => g.StartYear)
            .NotNull()
            .NotEmpty().WithMessage("Year is required.");

        RuleFor(g => g.EndYear)
            .NotNull()
            .NotEmpty()
            .GreaterThan(g => g.StartYear).WithMessage("EndYear must be greater than StartYear.");

		RuleFor(g => g.SpecialisationId)
            .NotNull()
            .GreaterThan(0).WithMessage("SpecialisationId must be a positive integer.")
            .MustAsync(async (id, cancellation) =>
            {
                var specialisation = await academicRepository.GetSpecialisationByIdAsync(id);
                return specialisation != null;
            }).WithMessage("Specialisation with the given ID does not exist.");
    }
}
