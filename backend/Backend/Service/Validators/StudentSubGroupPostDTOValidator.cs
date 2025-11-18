using Domain.DTOs;
using Repository.Interfaces;
using Utils;
using FluentValidation;

namespace Service.Validators;

public class StudentSubGroupPostDTOValidator : AbstractValidator<StudentSubGroupPostDTO>
{
    public StudentSubGroupPostDTOValidator(IAcademicRepository academicRepository)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Student sub-group name is required.")
            .MaximumLength(Constants.DefaultStringMaxLenght).WithMessage($"Student sub-group name must not exceed {Constants.DefaultStringMaxLenght} characters.");

        RuleFor(x => x.StudentGroupId)
            .GreaterThan(0).WithMessage("StudentGroupId must be a positive integer.")
            .MustAsync(async (studentGroupId, cancellation) =>
            {
                var studentGroup = await academicRepository.GetGroupByIdAsync(studentGroupId);
                return studentGroup != null;
            }).WithMessage("The specified StudentGroupId does not exist.");
    }
}
