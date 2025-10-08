using Backend.Domain.DTOs;
using Backend.Interfaces;
using FluentValidation;

namespace Backend.Service.Validators;

public class SpecialisationPostDTOValidator : AbstractValidator<SpecialisationPostDTO>
{
    private readonly IFacultyRepository _facultyRepository;
    private readonly ISpecialisationRepository _specialisationRepository;

    public SpecialisationPostDTOValidator(IFacultyRepository facultyRepository, ISpecialisationRepository specialisationRepository)
    {
        _facultyRepository = facultyRepository;
        _specialisationRepository = specialisationRepository;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Specialisation name is required.")
            .MaximumLength(100).WithMessage("Specialisation name must not exceed 100 characters.")
            .MustAsync(async (name, cancellation) =>
            {
                var existingSpecialisation = await _specialisationRepository.GetByNameAsync(name);
                return existingSpecialisation == null;
            }).WithMessage("A specialisation with the same name already exists.");

        RuleFor(x => x.FacultyId)
            .GreaterThan(0).WithMessage("Faculty ID must be a positive integer.")
            .MustAsync(async (facultyId, cancellation) =>
            {
                var faculty = await _facultyRepository.GetByIdAsync(facultyId);
                return faculty != null;
            }).WithMessage("Faculty with the given ID does not exist.");
    }
}
