using Backend.Domain.DTOs;
using Backend.Interfaces;
using Backend.Utils;
using FluentValidation;

namespace Backend.Service.Validators;

public class StudentGroupPostDTOValidator : AbstractValidator<StudentGroupPostDTO>
{
    public StudentGroupPostDTOValidator(IAcademicRepository academicRepository) 
    { 
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Student group name is required.")
            .MaximumLength(Constants.DefaultStringMaxLenght).WithMessage($"Student group name must not exceed {Constants.DefaultStringMaxLenght} characters.")
            .MustAsync(async (name, cancellation) =>
            {
                var existingGroup = await academicRepository.GetGroupByNameAsync(name);
                return existingGroup == null;
            }).WithMessage("A student group with the same name already exists.");

        RuleFor(x => x.GroupYearId)
            .GreaterThan(0).WithMessage("GroupYearId must be a positive integer.")
            .MustAsync(async (groupYearId, cancellation) =>
            {   
                if(!groupYearId.HasValue)
                    return true;
                var groupYear = await academicRepository.GetGroupYearByIdAsync(groupYearId.Value);
                return groupYear != null;
            }).WithMessage("The specified GroupYearId does not exist.");
    }
}
