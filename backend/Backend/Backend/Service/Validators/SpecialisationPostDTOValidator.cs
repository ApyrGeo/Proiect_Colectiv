using Backend.Domain.DTOs;
using Backend.Interfaces;
using Backend.Utils;
using FluentValidation;

namespace Backend.Service.Validators;

public class SpecialisationPostDTOValidator : AbstractValidator<SpecialisationPostDTO>
{
    public SpecialisationPostDTOValidator(IAcademicRepository academicRepository)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Specialisation name is required.")
            .MaximumLength(Constants.DefaultStringMaxLenght).WithMessage($"Specialisation name must not exceed {Constants.DefaultStringMaxLenght} characters.");

        RuleFor(x => x.FacultyId)
            .GreaterThan(0).WithMessage("Faculty ID must be a positive integer.")
            .MustAsync(async (facultyId, cancellation) =>
            {
                if(!facultyId.HasValue) return true;

                var faculty = await academicRepository.GetFacultyByIdAsync(facultyId.Value);
                return faculty != null;
            }).WithMessage("Faculty with the given ID does not exist.");
    }
}
