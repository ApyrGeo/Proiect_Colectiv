using Backend.Domain.DTOs;
using Backend.Interfaces;
using Backend.Utils;
using FluentValidation;

namespace Backend.Service.Validators;

public class FacultyPostDTOValidator : AbstractValidator<FacultyPostDTO>
{
    public FacultyPostDTOValidator(IAcademicRepository repo)
    {
        RuleFor(f => f.Name)
            .NotEmpty().WithMessage("Faculty name is required.")
            .MaximumLength(Constants.DefaultStringMaxLenght).WithMessage($"Faculty name must not exceed {Constants.DefaultStringMaxLenght} characters.")
            .MustAsync(async (name, cancellation) =>
            {
                var existingFaculty = await repo.GetFacultyByNameAsync(name); 
                return existingFaculty == null;
            }).WithMessage("A faculty with the same name already exists.");
    }
}
