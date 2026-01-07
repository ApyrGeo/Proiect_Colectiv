using TrackForUBB.Domain.DTOs;
using FluentValidation;
using TrackForUBB.Domain.Utils;
using TrackForUBB.Service.Interfaces;
using TrackForUBB.Domain.Enums;

namespace TrackForUBB.Service.Validators;

public class SubjectPostDTOValidator : AbstractValidator<SubjectPostDTO>
{
    public SubjectPostDTOValidator(IAcademicRepository academicRepository, IUserRepository userRepository)
    {
        RuleFor(f => f.Name)
            .NotEmpty().WithMessage("Subject name is required.")
            .MaximumLength(Constants.DefaultStringMaxLenght).WithMessage($"Subject name must not exceed {Constants.DefaultStringMaxLenght} characters.");

        RuleFor(f => f.NumberOfCredits)
            .NotNull().WithMessage("Nr credits is required.")
            .InclusiveBetween(1, 6).WithMessage("Nr credits must be between 1 and 6.");

        RuleFor(f => f.HolderTeacherId)
            .NotEmpty()
            .WithMessage("HolderTeacherId is required.")
            .MustAsync(async (holderTeacherId, cancellation) =>
            {
                var teacher = await academicRepository.GetTeacherById(holderTeacherId);
                return teacher != null;
            }).WithMessage("The specified HolderTeacherId does not exist.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Subject code is required");

        RuleFor(f => f.SemesterId)
            .NotEmpty().WithMessage("Semester id is required")
            .MustAsync(async (id, cancellation) =>
            {
                var semester = await academicRepository.GetSemesterByIdAsync(id);
                return semester != null;
            }).WithMessage("The specified Semester does not exist.");

        RuleFor(f => f.FormationType)
            .NotEmpty().WithMessage("Formation type is required.")
            .IsEnumName(typeof(SubjectFormationType)).WithMessage($"Formation type string cannot be converted to enum, available values: {string.Join(", ", Enum.GetNames(typeof(SubjectFormationType)))}.");
    }
}
