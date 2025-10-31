using Backend.Domain.DTOs;
using Backend.Interfaces;
using FluentValidation;

namespace Backend.Service.Validators;

public class SubjectPostDTOValidator : AbstractValidator<SubjectPostDTO>
{
    public SubjectPostDTOValidator(ITimetableRepository repo)
    {
        RuleFor(f => f.Name)
            .NotEmpty().WithMessage("Subject name is required.")
            .MaximumLength(100).WithMessage("Subject name must not exceed 100 characters.")
            .MustAsync(async (name, cancellation) =>
            {
                var existingSubject = await repo.GetSubjectByNameAsync(name); 
                return existingSubject == null;
            }).WithMessage("A subject with the same name already exists.");
    }
}