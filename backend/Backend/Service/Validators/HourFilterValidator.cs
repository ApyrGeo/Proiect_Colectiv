using FluentValidation;
using TrackForUBB.Domain.Utils;
using TrackForUBB.Service.Interfaces;

namespace TrackForUBB.Service.Validators;

public class HourFilterValidator : AbstractValidator<HourFilter>
{
    private readonly IAcademicRepository _academicRepository;
    private readonly ITimetableRepository _timetableRepository;

    public HourFilterValidator(IAcademicRepository academicRepository, ITimetableRepository timetableRepository)
    {
        _academicRepository = academicRepository;
        _timetableRepository = timetableRepository;

        RuleFor(x => x.UserId)
            .MustAsync(async (userId, cancellation) =>
            {
                if (userId == null) return true;

                var enrollments = await _academicRepository.GetEnrollmentsByUserId(userId.Value);
                return enrollments != null && enrollments.Count > 0;
            }).WithMessage(filter => $"Enrollments with ID {filter.UserId} not found.");

        RuleFor(x => x.ClassroomId)
            .MustAsync(async (classroomId, cancellation) =>
            {
                if (classroomId == null) return true;

                var classroom = await _timetableRepository.GetClassroomByIdAsync(classroomId.Value);
                return classroom != null;
            }).WithMessage(filter => $"Classroom with ID {filter.ClassroomId} not found.");

        RuleFor(x => x.TeacherId)
            .MustAsync(async (teacherId, cancellation) =>
            {
                if (teacherId == null) return true;

                var teacher = await _academicRepository.GetTeacherById(teacherId.Value);
                return teacher != null;
            }).WithMessage(filter => $"Teacher with ID {filter.TeacherId} not found.");

        RuleFor(x => x.SubjectId)
            .MustAsync(async (subjectId, cancellation) =>
            {
                if (subjectId == null) return true;

                var subject = await _timetableRepository.GetSubjectByIdAsync(subjectId.Value);
                return subject != null;
            }).WithMessage(filter => $"Subject with ID {filter.SubjectId} not found.");

        RuleFor(x => x.FacultyId)
            .MustAsync(async (facultyId, cancellation) =>
            {
                if (facultyId == null) return true;

                var faculty = await _academicRepository.GetFacultyByIdAsync(facultyId.Value);
                return faculty != null;
            }).WithMessage(filter => $"Faculty with ID {filter.FacultyId} not found.");

        RuleFor(x => x.SpecialisationId)
            .MustAsync(async (specialisationId, cancellation) =>
            {
                if (specialisationId == null) return true;

                var specialisation = await _academicRepository.GetSpecialisationByIdAsync(specialisationId.Value);
                return specialisation != null;
            }).WithMessage(filter => $"Specialisation with ID {filter.SpecialisationId} not found.");

        RuleFor(x => x.GroupYearId)
            .MustAsync(async (groupYearId, cancellation) =>
            {
                if (groupYearId == null) return true;

                var groupYear = await _academicRepository.GetPromotionByIdAsync(groupYearId.Value);
                return groupYear != null;
            }).WithMessage(filter => $"Group year with ID {filter.GroupYearId} not found.");

        RuleFor(x => x.SemesterNumber)
            .InclusiveBetween(1, 10)
            .WithMessage(filter => $"Semester must be between 1 and 10");
    }
}