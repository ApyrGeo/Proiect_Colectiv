using Backend.Domain.DTOs;
using Backend.Interfaces;
using FluentValidation;

namespace Backend.Service.Validators;

public class StudentGroupPostDTOValidator : AbstractValidator<StudentGroupPostDTO>
{
    private readonly IGroupYearRepository _groupYearRepository;
    private readonly IStudentGroupRepository _studentGroupRepository;

    public StudentGroupPostDTOValidator(IGroupYearRepository groupYearRepository, IStudentGroupRepository studentGroupRepository)
    {
        _groupYearRepository = groupYearRepository;
        _studentGroupRepository = studentGroupRepository;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Student group name is required.")
            .MaximumLength(50).WithMessage("Student group name must not exceed 50 characters.")
            .MustAsync(async (name, cancellation) =>
            {
                var existingGroup = await _studentGroupRepository.GetByNameAsync(name);
                return existingGroup == null;
            }).WithMessage("A student group with the same name already exists.");

        RuleFor(x => x.GroupYearId)
            .GreaterThan(0).WithMessage("GroupYearId must be a positive integer.")
            .MustAsync(async (groupYearId, cancellation) =>
            {
                var groupYear = await _groupYearRepository.GetByIdAsync(groupYearId);
                return groupYear != null;
            }).WithMessage("The specified GroupYearId does not exist.");
    }
}
