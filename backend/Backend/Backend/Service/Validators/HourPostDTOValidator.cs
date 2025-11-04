using Backend.Domain.DTOs;
using Backend.Domain.Enums;
using Backend.Interfaces;
using Backend.Utils;
using FluentValidation;

namespace Backend.Service.Validators
{
    public class HourPostDTOValidator : AbstractValidator<HourPostDTO>
    {
        public HourPostDTOValidator(ITimetableRepository timetableRepository, IAcademicRepository academicRepository)
        {
            RuleFor(x => x.Day)
                .NotNull()
                .NotEmpty().WithMessage("Day is required.")
                .IsEnumName(typeof(HourDay)).WithMessage($"Day string cannot be converted to enum, available values: {string.Join(", ", Enum.GetNames(typeof(HourDay)))}.");

            RuleFor(x => x.HourInterval)
                .NotNull()
                .NotEmpty().WithMessage("HourInterval is required.")
                .MaximumLength(Constants.DefaultStringMaxLenght).WithMessage($"Name cannot exceed {Constants.DefaultStringMaxLenght} characters.");

            RuleFor(x => x.Frequency)
                .NotNull()
                .NotEmpty().WithMessage("Frequency is required.")
                .IsEnumName(typeof(HourFrequency)).WithMessage($"Frequency string cannot be converted to enum, available values: {string.Join(", ", Enum.GetNames(typeof(HourFrequency)))}.");

            RuleFor(x => x.ClassroomId)
                .MustAsync(async (classroomId, cancellation) =>
                {
                    if (!classroomId.HasValue) return false;

                    var classroom = await timetableRepository.GetClassroomByIdAsync(classroomId.Value);
                    return classroom != null;
                }).WithMessage("The specified Classroom does not exist.");
            
            RuleFor(x => x.SubjectId)
                .MustAsync(async (subjectId, cancellation) =>
                {
                    if (!subjectId.HasValue) return false;

                    var subject = await timetableRepository.GetSubjectByIdAsync(subjectId.Value);
                    return subject != null;
                }).WithMessage("The specified Subject does not exist.");

            RuleFor(x => x.TeacherId)
                .MustAsync(async (teacherId, cancellation) =>
                {
                    if (!teacherId.HasValue) return false;

                    var teacher = await academicRepository.GetTeacherById(teacherId.Value);
                    return teacher != null;
                }).WithMessage("The specified Teacher does not exist.");

            RuleFor(x => x)
                .Must(dto =>
                {
                    var setCount = 0;
                    if (dto.GroupYearId != null) setCount++;
                    if (dto.StudentGroupId != null) setCount++;
                    if (dto.StudentSubGroupId != null) setCount++;
                    return setCount == 1;
                }).WithMessage("Only one of GroupYearId, StudentGroupId or StudentSubGroupId can be specified.");

            RuleFor(x => x.GroupYearId)
                .MustAsync(async (groupYearId, cancellation) =>
                {
                    if (groupYearId == null) return true; 

                    var gy = await academicRepository.GetGroupYearByIdAsync(groupYearId.Value);
                    return gy != null;
                }).WithMessage("The specified GroupYear does not exist.");

            RuleFor(x => x.StudentGroupId)
                .MustAsync(async (studentGroupId, cancellation) =>
                {
                    if (studentGroupId == null) return true;

                    var group = await academicRepository.GetGroupByIdAsync(studentGroupId.Value);
                    return group != null;
                }).WithMessage("The specified StudentGroup does not exist.");

            RuleFor(x => x.StudentSubGroupId)
                .MustAsync(async (studentSubGroupId, cancellation) =>
                {
                    if (studentSubGroupId == null) return true;

                    var subGroup = await academicRepository.GetSubGroupByIdAsync(studentSubGroupId.Value);
                    return subGroup != null;
                }).WithMessage("The specified StudentSubGroup does not exist.");
        }
    }
}
