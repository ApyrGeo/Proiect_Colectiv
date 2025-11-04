using Backend.Domain.DTOs;
using Backend.Domain.Enums;
using Backend.Interfaces;
using FluentValidation;

namespace Backend.Service.Validators
{
    public class TeacherPostDTOValidator : AbstractValidator<TeacherPostDTO>
    {
        public TeacherPostDTOValidator(IAcademicRepository academicRepository, IUserRepository userRepository)
        {
            RuleFor(e => e.UserId)
                .GreaterThan(0).WithMessage("UserId must be a positive integer.")
                .MustAsync(async (userId, cancellation) =>
                {
                    var user = await userRepository.GetByIdAsync(userId);
                    return user != null && user.Role == UserRole.Teacher;
                }).WithMessage("User with the specified UserId does not exist.");

            RuleFor(e => e.FacultyId)
                .GreaterThan(0).WithMessage("FacultyId must be a positive integer.")
                .MustAsync(async (facultyId, cancellation) =>
                {
                    var faculty = await academicRepository.GetFacultyByIdAsync(facultyId);
                    return faculty != null;
                }).WithMessage("Faculty with the specified FacultyId does not exist.");
        }
    }
}
