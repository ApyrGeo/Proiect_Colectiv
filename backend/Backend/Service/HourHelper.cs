using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Enums;
using TrackForUBB.Domain.Utils;

namespace TrackForUBB.Service;

public class HourHelper
{
    public static HourFrequency GetWeekType(DateTime date)
    {
        int weeks = (DateOnly.FromDateTime(date).DayNumber - HardcodedData.CalendarStartDate.DayNumber) / 7;

        return weeks % 2 == 0 ? HourFrequency.FirstWeek : HourFrequency.SecondWeek;
    }

    public static void MarkHours(List<HourResponseDTO> hours)
    {
        if (hours == null || hours.Count == 0) return;

        var groupedHours = hours.GroupBy(h => h.Day)
            .Select(g => new
            {
                Day = HourDayConverter.ConvertToDayOfWeek(Enum.Parse<HourDay>(g.Key)),
                HourIntervalGroup = g.GroupBy(h => h.HourInterval)
            })
            .ToDictionary(
                x => x.Day,
                x => x.HourIntervalGroup.ToDictionary(
                    hg => hg.Key,
                    hg => hg.ToList()
                )
            );

        var now = DateTime.Now;
        var todayName = now.DayOfWeek;

        static bool TryParseInterval(string interval, out int startHour, out int endHour)
        {
            startHour = endHour = -1;

            if (string.IsNullOrWhiteSpace(interval)) return false;

            var parts = interval.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (parts.Length != 2) return false;

            if (!int.TryParse(parts[0], out startHour) || !int.TryParse(parts[1], out endHour)) return false;

            if (startHour < 0 || startHour > 23) return false;

            if (endHour < 1 || endHour > 24) return false;

            return true;
        }

        DateOnly DateOnlyOf(DateTime dt) => DateOnly.FromDateTime(dt);
        DateTime IntervalStart(DateOnly date, int sh) => date.ToDateTime(new TimeOnly(sh, 0));
        DateTime IntervalEnd(DateOnly date, int eh) => eh == 24 ? date.AddDays(1).ToDateTime(new TimeOnly(0, 0)) : date.ToDateTime(new TimeOnly(eh, 0));

        void MarkCurrentGroup(List<HourResponseDTO> group)
        {
            foreach (var hh in group)
            {
                if (hh.Frequency == HourFrequency.Weekly.ToString() || hh.Frequency == Constants.CurrentWeekType.ToString())
                {
                    hh.IsCurrent = true;
                }
            }
        }

        void MarkNextGroup(List<HourResponseDTO> group, DateTime futureDate)
        {
            foreach (var hh in group)
            {
                if (hh.Frequency == HourFrequency.Weekly.ToString() || hh.Frequency == GetWeekType(futureDate).ToString())
                {
                    hh.IsNext = true;
                }
            }
        }

        if (groupedHours.TryGetValue(todayName, out var todayIntervals))
        {
            foreach (var todayHours in todayIntervals)
            {
                if (!TryParseInterval(todayHours.Key, out var sh, out var eh)) continue;

                var start = IntervalStart(DateOnlyOf(now), sh);
                var end = IntervalEnd(DateOnlyOf(now), eh);

                if (start <= now && now < end)
                {
                    if (todayHours.Value.Any(hh => hh.Frequency == HourFrequency.Weekly.ToString() || hh.Frequency == Constants.CurrentWeekType.ToString()))
                    {
                        MarkCurrentGroup(todayHours.Value);
                    }

                    break;
                }
            }

            var (Start, Group) = todayIntervals
                .Select(kv =>
                {
                    if (!TryParseInterval(kv.Key, out var sh, out _)) return (Start: DateTime.MaxValue, Group: kv.Value);

                    return (Start: IntervalStart(DateOnlyOf(now), sh), Group: kv.Value);
                })
                .Where(x => x.Start > now)
                .OrderBy(x => x.Start)
                .FirstOrDefault();

            if (Group != null && Start != DateTime.MaxValue)
            {
                if (Group.Any(hh => hh.Frequency == HourFrequency.Weekly.ToString() || hh.Frequency == Constants.CurrentWeekType.ToString()))
                {
                    MarkNextGroup(Group, now);
                }

                return;
            }
        }

        for (int offset = 1; offset <= 7; offset++)
        {
            var searchDateTime = now.AddDays(offset);
            var dayName = searchDateTime.DayOfWeek;

            if (!groupedHours.TryGetValue(dayName, out var dayIntervals) || dayIntervals.Count == 0) continue;

            var (Start, Group) = dayIntervals
                .Select(kv =>
                {
                    if (!TryParseInterval(kv.Key, out var sh, out _)) return (Start: DateTime.MaxValue, Group: kv.Value);

                    return (Start: IntervalStart(DateOnlyOf(searchDateTime), sh), Group: kv.Value);
                })
                .OrderBy(x => x.Start)
                .FirstOrDefault();

            if (Group != null && Start != DateTime.MaxValue)
            {
                if (Group.Any(hh => hh.Frequency == HourFrequency.Weekly.ToString() || hh.Frequency == GetWeekType(searchDateTime).ToString()))
                {
                    MarkNextGroup(Group, searchDateTime);
                }

                return;
            }
        }
    }
}
