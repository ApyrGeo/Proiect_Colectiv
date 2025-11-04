using Backend.Domain.DTOs;
using Backend.Interfaces;
using FluentValidation;

namespace Backend.Service.Validators;

public class StudentSubGroupPostDTOValidator : AbstractValidator<StudentSubGroupPostDTO>
{
    public StudentSubGroupPostDTOValidator(IAcademicRepository academicRepository)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Student sub-group name is required.")
            .MaximumLength(50).WithMessage("Student sub-group name must not exceed 50 characters.")
            .MustAsync(async (dto, name, cancellation) =>
            {
                var existingSubGroup = await academicRepository.GetSubGroupByNameAsync(name);
                return existingSubGroup == null;
            }).WithMessage("A student sub-group with the same name already exists in the specified student group.");

        RuleFor(x => x.StudentGroupId)
            .GreaterThan(0).WithMessage("StudentGroupId must be a positive integer.")
            .MustAsync(async (studentGroupId, cancellation) =>
            {
                if (!studentGroupId.HasValue)
                    return true;
                var studentGroup = await academicRepository.GetGroupByIdAsync(studentGroupId.Value);
                return studentGroup != null;
            }).WithMessage("The specified StudentGroupId does not exist.");
    }
}
