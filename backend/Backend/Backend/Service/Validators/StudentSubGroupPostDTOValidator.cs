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
            .MaximumLength(50).WithMessage("Student sub-group name must not exceed 50 characters.");

        RuleFor(x => x.StudentGroupId)
            .GreaterThan(0).WithMessage("StudentGroupId must be a positive integer.")
            .MustAsync(async (studentGroupId, cancellation) =>
            {
                if (!studentGroupId.HasValue)
                    return false;

                var studentGroup = await academicRepository.GetGroupByIdAsync(studentGroupId.Value);
                return studentGroup != null;
            }).WithMessage("The specified StudentGroupId does not exist.");
    }
}
