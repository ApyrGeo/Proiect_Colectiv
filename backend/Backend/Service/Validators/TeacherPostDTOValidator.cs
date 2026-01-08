using TrackForUBB.Domain.DTOs;
using FluentValidation;
using TrackForUBB.Domain.Enums;
using TrackForUBB.Service.Interfaces;

namespace TrackForUBB.Service.Validators;

public class TeacherPostDTOValidator : AbstractValidator<TeacherPostDTO>
{
    public TeacherPostDTOValidator(IAcademicRepository academicRepository, IUserRepository userRepository)
    {
        RuleFor(e => e.UserId)
            .GreaterThan(0).WithMessage("UserId must be a positive integer.")
            .MustAsync(async (userId, cancellation) =>
            {
                var user = await userRepository.GetByIdAsync(userId);
                return user?.Role == UserRole.Teacher;
            }).WithMessage("User does not have teacher role.")
            .MustAsync(async (userId, cancellation) =>
            {
                var teacher = await academicRepository.GetTeacherByUserIdAsync(userId);
                return teacher == null;
            }).WithMessage("A teacher with the specified UserId already exists.");

        RuleFor(e => e.FacultyId)
            .GreaterThan(0).WithMessage("FacultyId must be a positive integer.")
            .MustAsync(async (facultyId, cancellation) =>
            {
                var faculty = await academicRepository.GetFacultyByIdAsync(facultyId);
                return faculty != null;
            }).WithMessage("Faculty with the specified FacultyId does not exist.");
    }
}
