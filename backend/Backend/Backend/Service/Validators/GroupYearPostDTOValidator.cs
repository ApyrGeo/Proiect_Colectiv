using Backend.Domain.DTOs;
using Backend.Interfaces;
using FluentValidation;

namespace Backend.Service.Validators;

public class GroupYearPostDTOValidator : AbstractValidator<GroupYearPostDTO>
{
    private readonly ISpecialisationRepository _specialisationRepository;
    private readonly IGroupYearRepository _groupYearRepository;
    public GroupYearPostDTOValidator(ISpecialisationRepository specialisationRepository, IGroupYearRepository groupYearRepository)
    {
        _specialisationRepository = specialisationRepository;
        _groupYearRepository = groupYearRepository;

        RuleFor(g => g.Year)
            .NotNull()
            .NotEmpty()
            .WithMessage("Year is required.")
            .MustAsync(async (year, cancellation) =>
            {
                var existingGroupYear = await _groupYearRepository.GetByYearAsync(year);
                return existingGroupYear == null;
            })
            .WithMessage("GroupYear name already exists");


        RuleFor(g => g.SpecialisationId)
            .NotNull()
            .GreaterThan(0)
            .WithMessage("SpecialisationId must be a positive integer.")
            .MustAsync(async (id, cancellation) => { 
                var specialisation = await _specialisationRepository.GetByIdAsync(id);
                return specialisation != null;
            })
            .WithMessage("Specialisation with the given ID does not exist.");
    }
}
