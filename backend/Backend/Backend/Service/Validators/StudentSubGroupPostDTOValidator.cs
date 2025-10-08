using Backend.Domain.DTOs;
using Backend.Interfaces;
using FluentValidation;

namespace Backend.Service.Validators;

public class StudentSubGroupPostDTOValidator : AbstractValidator<StudentSubGroupPostDTO>
{
    private readonly IStudentGroupRepository _studentGroupRepository;
    private readonly IStudentSubGroupRepository _studentSubGroupRepository;

    public StudentSubGroupPostDTOValidator(IStudentGroupRepository studentGroupRepository, IStudentSubGroupRepository studentSubGroupRepository)
    {
        _studentGroupRepository = studentGroupRepository;
        _studentSubGroupRepository = studentSubGroupRepository;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Student sub-group name is required.")
            .MaximumLength(50).WithMessage("Student sub-group name must not exceed 50 characters.")
            .MustAsync(async (dto, name, cancellation) =>
            {
                var existingSubGroup = await _studentSubGroupRepository.GetByNameAsync(name);
                return existingSubGroup == null;
            }).WithMessage("A student sub-group with the same name already exists in the specified student group.");

        RuleFor(x => x.StudentGroupId)
            .GreaterThan(0).WithMessage("StudentGroupId must be a positive integer.")
            .MustAsync(async (studentGroupId, cancellation) =>
            {
                var studentGroup = await _studentGroupRepository.GetByIdAsync(studentGroupId);
                return studentGroup != null;
            }).WithMessage("The specified StudentGroupId does not exist.");
    }
}
