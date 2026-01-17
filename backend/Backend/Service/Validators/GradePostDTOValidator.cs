using FluentValidation;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Service.Interfaces;

namespace TrackForUBB.Service.Validators;

public class GradePostDTOValidator : AbstractValidator<GradePostDTO>
{
    public GradePostDTOValidator(IGradeRepository gradeRepository,IAcademicRepository academicRepository,ITimetableRepository timetableRepository)
    {
        RuleFor(g => g.Value)
            .NotNull().WithMessage("Grade value cannot be null.")
            .InclusiveBetween(1, 10).WithMessage("Grade must be between 1 and 10.");

        RuleFor(g => g.SubjectId)
            .GreaterThan(0).WithMessage("SubjectId must be a positive integer.")
            .MustAsync(async (subjectId, _) =>
            {
                var subject = await timetableRepository.GetSubjectByIdAsync(subjectId);
                return subject != null;
            })
            .WithMessage("Subject with the given ID does not exist.");
        
        RuleFor(g => g.EnrollmentId)
            .GreaterThan(0).WithMessage("EnrollmentId must be a positive integer.")
            .MustAsync(async (enrollmentId, _) =>
            {
                var enrollment = await academicRepository.GetEnrollmentByIdAsync(enrollmentId);
                return enrollment != null;
            })
            .WithMessage("Enrollment with the given ID does not exist.");
        
        RuleFor(g => g)
            .MustAsync(async (dto, _) =>
            {
                var existing = await gradeRepository.GetGradeByEnrollmentAndSubjectAsync(
                    dto.EnrollmentId, 
                    dto.SubjectId);
                    return existing == null;
                })
            .WithMessage("A grade for this subject already exists for this student.");
    }
}
