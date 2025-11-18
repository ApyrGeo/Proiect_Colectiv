using Domain.DTOs;
using Repository.Interfaces;
using Utils;
using FluentValidation;

namespace Service.Validators;

public class GroupYearPostDTOValidator : AbstractValidator<GroupYearPostDTO>
{
    public GroupYearPostDTOValidator(IAcademicRepository academicRepository)
    {
        RuleFor(g => g.Year)
            .NotNull()
            .NotEmpty().WithMessage("Year is required.")
            .MaximumLength(Constants.DefaultStringMaxLenght).WithMessage($"Year must not exceed {Constants.DefaultStringMaxLenght} characters.");

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
