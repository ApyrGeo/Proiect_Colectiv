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
        
        RuleFor(f => f.NrCredits)
            .NotNull().WithMessage("Nr credits is required.")
            .InclusiveBetween(1, 6).WithMessage("Nr credits must be between 1 and 6.");
        
        RuleFor(f => f.ForScholarship)
            .NotNull().WithMessage("You must specify if the subject counts for scholarship.");
          
    }
}