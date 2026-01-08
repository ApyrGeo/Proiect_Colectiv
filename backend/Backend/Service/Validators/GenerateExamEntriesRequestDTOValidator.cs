using FluentValidation;
using TrackForUBB.Domain.DTOs;

namespace TrackForUBB.Service.Validators;

public class GenerateExamEntriesRequestDTOValidator : AbstractValidator<GenerateExamEntriesRequestDTO>
{
    public GenerateExamEntriesRequestDTOValidator()
    {
        RuleFor(x => x.SubjectId)
            .NotNull().WithMessage("SubjectId cannot be null.")
            .GreaterThan(0).WithMessage("SubjectId must be a positive integer.");
        RuleFor(x => x.StudentGroupIds)
            .NotNull().WithMessage("StudentGroupIds cannot be null.")
            .Must(list => list != null && list.Count > 0).WithMessage("StudentGroupIds cannot be empty.")
            .ForEach(idRule => idRule.GreaterThan(0).WithMessage("Each StudentGroupId must be a positive integer."));
    }
}
