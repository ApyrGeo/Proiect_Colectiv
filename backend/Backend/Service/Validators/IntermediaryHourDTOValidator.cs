using FluentValidation;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Enums;
using TrackForUBB.Domain.Utils;
using TrackForUBB.Service.Interfaces;

namespace TrackForUBB.Service.Validators;

public class IntermediaryHourDTOValidator : AbstractValidator<IntermediaryHourDTO>
{
    public IntermediaryHourDTOValidator(ITimetableRepository timetableRepository)
    {
        RuleFor(h => h.HourInterval)
            .NotEmpty().WithMessage("HourInterval is required.")
            .Matches(@"^(\d{1,2}:00-\d{1,2}:00|00:00-00:00)$").WithMessage("HourInterval must be in the format 'startHour-endHour'.");

        RuleFor(h => h)
            .MustAsync(async (dto, cancellation) =>
            {
                var filter = new HourFilter { ClassroomId = dto.ClassroomId };
                var existingHours = await timetableRepository.GetHoursAsync(filter);
                return !HasSchedulingConflict(dto, existingHours);
            })
            .WithMessage(dto => $"A scheduling conflict exists: another hour is already scheduled in classroom {dto.ClassroomId} on {dto.Day} at {dto.HourInterval} with a conflicting frequency.");

        RuleFor(h => h)
            .MustAsync(async (dto, cancellation) =>
            {
                var filter = new HourFilter { TeacherId = dto.TeacherId };
                var existingHours = await timetableRepository.GetHoursAsync(filter);
                return !HasSchedulingConflict(dto, existingHours);
            })
            .WithMessage(dto => $"Teacher conflict: the teacher with ID {dto.TeacherId} is already scheduled for another class on {dto.Day} at {dto.HourInterval} with a conflicting frequency.");
    }

    private static bool HasSchedulingConflict(IntermediaryHourDTO dto, List<HourResponseDTO> existingHours)
    {
        if (dto.HourInterval == "00:00-00:00")
            return false;
        
        return existingHours.Any(h =>
        {
            if (h.Id == dto.Id)
                return false;

            if (!Enum.TryParse<HourDay>(h.Day, ignoreCase: true, out var existingDay) || existingDay != dto.Day)
                return false;

            if (h.HourInterval != dto.HourInterval)
                return false;

            if (!Enum.TryParse<HourFrequency>(h.Frequency, ignoreCase: true, out var existingFrequency))
                return false;

            return dto.Frequency == HourFrequency.Weekly
                || existingFrequency == HourFrequency.Weekly
                || dto.Frequency == existingFrequency;
        });
    }
}