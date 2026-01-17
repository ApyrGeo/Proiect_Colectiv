using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Service.Interfaces;

namespace TrackForUBB.Service.Validators;

public class ExamEntryDTOValidator : AbstractValidator<ExamEntryPutDTO>
{
    public ExamEntryDTOValidator(ITimetableRepository timetableRepository, IAcademicRepository academicRepository, IExamRepository examRepository)
    {
        RuleFor(x => x.Date)
            .NotNull()
            .NotEmpty().WithMessage("ExamDate is required.")
            .Must(date => date > DateTime.Now).WithMessage("ExamDate must be in the future.")
            .MustAsync(async (dto, date, cancellation) =>
            {
                var existingExams = await examRepository.GetExamsBySubjectId(dto.SubjectId);
                return !existingExams.Any(e => e.Date == date && e.StudentGroup.Id == dto.StudentGroupId);
            }).WithMessage("An exam for the specified Subject and Student Group already exists on the given date.");

        RuleFor(x => x.Duration)
            .GreaterThan(0).WithMessage("Duration must be greater than 0.");

        RuleFor(x => x)
            .MustAsync(async (dto, cancellation) =>
            {
                var classroom = await timetableRepository.GetClassroomByIdAsync(dto.ClassroomId);
                if (classroom == null) return true; 

                var allExams = await examRepository.GetExamsBySubjectId(dto.SubjectId);
                var classroomExams = allExams.Where(e => e.Classroom?.Id == dto.ClassroomId).ToList();

                var newExamStart = dto.Date;
                var newExamEnd = dto.Date.AddMinutes(dto.Duration);

                foreach (var exam in classroomExams)
                {
                    if (exam.Date == null || exam.Duration == null) continue;
                    if (exam.Id == dto.Id) continue;
                    var existingExamStart = exam.Date;
                    var existingExamEnd = exam.Date?.AddMinutes((double) exam.Duration);

                    if (newExamStart < existingExamEnd && newExamEnd > existingExamStart)
                    {
                        return false;
                    }
                }

                return true;
            }).WithMessage("The classroom is already occupied during the specified time period.");

        RuleFor(x => x.SubjectId)
            .MustAsync(async (subjectId, cancellation) =>
            {
                var subject = await timetableRepository.GetSubjectByIdAsync(subjectId);
                return subject != null;
            }).WithMessage("The specified Subject does not exist.");

        RuleFor(x => x.StudentGroupId)
            .MustAsync(async (studentGroupId, cancellation) =>
            {
                var studentGroup = await academicRepository.GetGroupByIdAsync(studentGroupId);
                return studentGroup != null;
            }).WithMessage("The specified Student Group does not exist.");

        RuleFor(x => x.ClassroomId)
            .MustAsync(async (classroomId, cancellation) =>
            {
                var classroom = await timetableRepository.GetClassroomByIdAsync(classroomId);
                return classroom != null;
            }).WithMessage("The specified Classroom does not exist.");

    }
}
