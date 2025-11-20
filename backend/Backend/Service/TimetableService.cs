using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Exceptions.Custom;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using log4net;
using TrackForUBB.Service.Interfaces;
using TrackForUBB.Service.Utils;
using System.Globalization;
using System.Text;
using System.Text.Json;
using Calendar = Ical.Net.Calendar;
using IValidatorFactory = TrackForUBB.Service.Interfaces.IValidatorFactory;
using TrackForUBB.Domain.Utils;
using TrackForUBB.Domain.Enums;
using TrackForUBB.Controller.Interfaces;

namespace TrackForUBB.Service;

public class TimetableService(ITimetableRepository timetableRepository, IValidatorFactory validatorFactory) : ITimetableService
{
    private readonly ITimetableRepository _timetableRepository = timetableRepository;
    private readonly ILog _logger = LogManager.GetLogger(typeof(TimetableService));
    private readonly IValidatorFactory _validatorFactory = validatorFactory;

    public async Task<ClassroomResponseDTO> CreateClassroom(ClassroomPostDTO classroomPostDTO)
    {
        _logger.InfoFormat("Validating ClassroomPostDTO: {0}", JsonSerializer.Serialize(classroomPostDTO));

        var validator = _validatorFactory.Get<ClassroomPostDTO>();
        var validationResult = await validator.ValidateAsync(classroomPostDTO);

        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(ValidationHelper.ConvertErrorToListOfStrings(validationResult.Errors));
        }

        _logger.InfoFormat("Adding new classroom to repository: {0}", JsonSerializer.Serialize(classroomPostDTO));

        var classroomDto = await _timetableRepository.AddClassroomAsync(classroomPostDTO);
        await _timetableRepository.SaveChangesAsync();

        return classroomDto;
    }

    public async Task<HourResponseDTO> CreateHour(HourPostDTO hourPostDTO)
    {
        _logger.InfoFormat("Validating HourPostDTO: {0}", JsonSerializer.Serialize(hourPostDTO));

        var validator = _validatorFactory.Get<HourPostDTO>();
        var validationResult = await validator.ValidateAsync(hourPostDTO);

        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(ValidationHelper.ConvertErrorToListOfStrings(validationResult.Errors));
        }

        _logger.InfoFormat("Adding new hour to repository: {0}", JsonSerializer.Serialize(hourPostDTO));

        var hourDto = await _timetableRepository.AddHourAsync(hourPostDTO);
        await _timetableRepository.SaveChangesAsync();

        return hourDto;
    }

    public async Task<LocationResponseDTO> CreateLocation(LocationPostDTO locationPostDTO)
    {
        _logger.InfoFormat("Validating LocationPostDTO: {0}", JsonSerializer.Serialize(locationPostDTO));

        var validator = _validatorFactory.Get<LocationPostDTO>();
        var validationResult = await validator.ValidateAsync(locationPostDTO);

        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(ValidationHelper.ConvertErrorToListOfStrings(validationResult.Errors));
        }

        _logger.InfoFormat("Adding new location to repository: {0}", JsonSerializer.Serialize(locationPostDTO));

        var locationDto = await _timetableRepository.AddLocationAsync(locationPostDTO);
        await _timetableRepository.SaveChangesAsync();

        return locationDto;
    }

    public async Task<SubjectResponseDTO> CreateSubject(SubjectPostDTO subjectPostDto)
    {
        _logger.InfoFormat("Validating SubjectPostDTO: {0}", JsonSerializer.Serialize(subjectPostDto));

        var validator = _validatorFactory.Get<SubjectPostDTO>();
        var validationResult = await validator.ValidateAsync(subjectPostDto);

        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(ValidationHelper.ConvertErrorToListOfStrings(validationResult.Errors));
        }

        _logger.InfoFormat("Adding new subject to repository: {0}", JsonSerializer.Serialize(subjectPostDto));

        var subjectDto = await _timetableRepository.AddSubjectAsync(subjectPostDto);
        await _timetableRepository.SaveChangesAsync();

        return subjectDto;
    }

    public async Task<ClassroomResponseDTO> GetClassroomById(int classroomId)
    {
        _logger.InfoFormat("Trying to retrieve classroom with id {0}", classroomId);

        var classroomDto = await _timetableRepository.GetClassroomByIdAsync(classroomId)
            ?? throw new NotFoundException($"Classroom with ID {classroomId} not found.");

        _logger.InfoFormat("Mapping classroom entity to DTO for ID {0}", classroomId);

        return classroomDto;
    }

    public async Task<TimetableResponseDTO> GetHourByFilter(HourFilter filter)
    {
        _logger.InfoFormat("Trying to retrive hours with filter {0}", JsonSerializer.Serialize(filter));

        var validator = _validatorFactory.Get<HourFilter>();
        var validationResult = await validator.ValidateAsync(filter);

        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(ValidationHelper.ConvertErrorToListOfStrings(validationResult.Errors));
        }

        var hoursDto = await _timetableRepository.GetHoursAsync(filter);

        _logger.InfoFormat("Mapping hour entities to DTOs for filter {0}", JsonSerializer.Serialize(filter));

        HourHelper.MarkHours(hoursDto);

        return new TimetableResponseDTO { Hours = hoursDto, CalendarStartISODate = HardcodedData.CalendarStartDate.ToString("o", CultureInfo.InvariantCulture) };
    }

    public async Task<HourResponseDTO> GetHourById(int hourId)
    {
        _logger.InfoFormat("Trying to retrieve hour with id {0}", hourId);

        var hourDto = await _timetableRepository.GetHourByIdAsync(hourId)
            ?? throw new NotFoundException($"Hour with ID {hourId} not found.");

        _logger.InfoFormat("Mapping hour entity to DTO for ID {0}", hourId);

        return hourDto;
    }

    public async Task<LocationResponseDTO> GetLocationById(int locationId)
    {
        _logger.InfoFormat("Trying to retrieve location with id {0}", locationId);

        var locationDto = await _timetableRepository.GetLocationByIdAsync(locationId)
            ?? throw new NotFoundException($"Location with ID {locationId} not found.");

        _logger.InfoFormat("Mapping location entity to DTO for ID {0}", locationId);

        return locationDto;
    }

    public async Task<SubjectResponseDTO> GetSubjectById(int subjectId)
    {
        _logger.InfoFormat("Trying to retrieve subject with id {0}", JsonSerializer.Serialize(subjectId));

        var subjectDto = await _timetableRepository.GetSubjectByIdAsync(subjectId)
            ?? throw new NotFoundException($"Subject with ID {subjectId} not found.");

        _logger.InfoFormat("Mapping subject entity to DTO for ID {0}", JsonSerializer.Serialize(subjectId));

        return subjectDto;
    }

    public async Task<byte[]> GenerateIcs(HourFilter filter)
    {
        _logger.InfoFormat("Trying to retrive hours with filter {0}", JsonSerializer.Serialize(filter));

        var validator = _validatorFactory.Get<HourFilter>();
        var validationResult = await validator.ValidateAsync(filter);

        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(ValidationHelper.ConvertErrorToListOfStrings(validationResult.Errors));
        }

        var hours = await _timetableRepository.GetHoursAsync(filter);

        var calendar = new Calendar
        {
            ProductId = "-//AcademicApp//Timetable//EN",
            Method = "PUBLISH"
        };

        calendar.AddTimeZone(new VTimeZone(Constants.CalendarTzId));

        static CalDateTime MakeCalDateTime(DateOnly d, TimeSpan time, string tzId)
        {
            var days = (int)Math.Floor(time.TotalDays);
            var timeOfDayTicks = time.Ticks % TimeSpan.FromDays(1).Ticks;
            var timeOfDay = TimeSpan.FromTicks(timeOfDayTicks);

            var date = d.AddDays(days);
            var to = TimeOnly.FromTimeSpan(timeOfDay);
            var dt = date.ToDateTime(to);
            return new CalDateTime(dt, tzId);
        }

        static DateOnly AddWeeks(DateOnly d, int weeks) => d.AddDays(weeks * 7);

        foreach (var hour in hours)
        {
            if (!Enum.TryParse<DayOfWeek>(hour.Day.Trim(), ignoreCase: true, out var hourDayOfWeek))
            {
                continue;
            }

            var intervalParts = hour.HourInterval.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToArray();

            if (intervalParts.Length != 2)
            {
                continue;
            }

            if (!int.TryParse(intervalParts[0].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var startHour))
            {
                continue;
            }

            if (!int.TryParse(intervalParts[1].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var endHour))
            {
                continue;
            }

            if (startHour < 0 || startHour > 23)
            {
                continue;
            }

            if (endHour < 0 || endHour > 24)
            {
                continue;
            }

            var startTime = TimeSpan.FromHours(startHour);
            var endTime = TimeSpan.FromHours(endHour);

            var daysUntil = ((int)hourDayOfWeek - (int)HardcodedData.CalendarStartDate.DayOfWeek + 7) % 7;
            var firstDate = HardcodedData.CalendarStartDate.AddDays(daysUntil);

            var summary = $"{hour.Subject.Name} - {hour.Teacher.User.FirstName} {hour.Teacher.User.LastName}";
            var locationText = $"Location: {hour.Location.Name} - Classroom: {hour.Classroom.Name}";
            var format = hour.Format;
            var description = $"Category: {hour.Category} - Format: {format}";

            void AddRecurringCalendarEvent(DateOnly dtStartDate, int weeklyInterval, int count)
            {
                var ev = new CalendarEvent
                {
                    Summary = summary,
                    Location = locationText,
                    Description = description,
                    DtStamp = new CalDateTime(DateTime.UtcNow, "UTC"),
                    Created = new CalDateTime(DateTime.UtcNow, "UTC"),
                    Uid = $"hour-{hour.Id}-{Guid.NewGuid()}",
                    DtStart = MakeCalDateTime(dtStartDate, startTime, Constants.CalendarTzId),
                    DtEnd = MakeCalDateTime(dtStartDate, endTime, Constants.CalendarTzId)
                };

                var rrule = new RecurrencePattern(FrequencyType.Weekly)
                {
                    Interval = weeklyInterval,
                    Count = count,
                    ByDay = [new WeekDay(hourDayOfWeek)]
                };

                ev.RecurrenceRules = [rrule];
                ev.Categories = [hour.Category.ToString()];

                calendar.Events.Add(ev);
            }

            switch (hour.Frequency)
            {
                case nameof(HourFrequency.Weekly):
                    AddRecurringCalendarEvent(firstDate, weeklyInterval: 1, count: 12);
                    var secondBlockStart = AddWeeks(firstDate, 14);
                    AddRecurringCalendarEvent(secondBlockStart, weeklyInterval: 1, count: 2);
                    break;

                case nameof(HourFrequency.FirstWeek):
                    AddRecurringCalendarEvent(firstDate, weeklyInterval: 2, count: 6);
                    var firstWeekSecondStart = AddWeeks(firstDate, 14);
                    AddRecurringCalendarEvent(firstWeekSecondStart, weeklyInterval: 2, count: 1);
                    break;

                case nameof(HourFrequency.SecondWeek):
                    var shifted = firstDate.AddDays(7);
                    AddRecurringCalendarEvent(shifted, weeklyInterval: 2, count: 6);
                    var secondWeekSecondStart = AddWeeks(shifted, 14);
                    AddRecurringCalendarEvent(secondWeekSecondStart, weeklyInterval: 2, count: 1);
                    break;

                default:
                    AddRecurringCalendarEvent(firstDate, weeklyInterval: 1, count: 12);
                    var fallbackSecond = AddWeeks(firstDate, 14);
                    AddRecurringCalendarEvent(fallbackSecond, weeklyInterval: 1, count: 2);
                    break;
            }
        }

        var serializer = new CalendarSerializer();
        var serialized = serializer.SerializeToString(calendar) ?? string.Empty;

        return Encoding.UTF8.GetBytes(serialized);
    }
}