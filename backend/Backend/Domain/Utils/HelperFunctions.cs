using System.Globalization;
using System.Text;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Enums;

namespace TrackForUBB.Domain.Utils;
public class HelperFunctions
{
    public static HourFrequency GetWeekType(DateTime date)
    {
        int weeks = (DateOnly.FromDateTime(date).DayNumber - HardcodedData.CalendarStartDate.DayNumber) / 7;

        return weeks % 2 == 0 ? HourFrequency.FirstWeek : HourFrequency.SecondWeek;
    }

    public static HourFrequency CurrentWeekType => GetWeekType(DateTime.Now);

    public static int GetCurrentStudentYear(int promotionStartYear)
    {
        int currentYear = DateTime.Now.Year;
        int yearDifference = currentYear - promotionStartYear;
        if (DateTime.Now.Month >= 7)
            yearDifference += 1;

        return yearDifference;
    }

    public static string ReplaceRomanianDiacritics(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input ?? string.Empty;
        }

        var map = new Dictionary<char, char>
        {
            ['Ă'] = 'A',
            ['ă'] = 'a',
            ['Â'] = 'A',
            ['â'] = 'a',
            ['Î'] = 'I',
            ['î'] = 'i',
            ['Ș'] = 'S',
            ['ș'] = 's',
            ['Ş'] = 'S',
            ['ş'] = 's',
            ['Ț'] = 'T',
            ['ț'] = 't',
            ['Ţ'] = 'T',
            ['ţ'] = 't'
        };

        var sb = new StringBuilder(input.Length);
        foreach (var ch in input)
        {
            if (map.TryGetValue(ch, out var replacement))
            {
                sb.Append(replacement);
                continue;
            }
            
            sb.Append(ch);
        }

        var normalized = sb.ToString().Normalize(NormalizationForm.FormD);
        var clean = new StringBuilder(normalized.Length);

        foreach (var ch in normalized)
        {
            var uc = CharUnicodeInfo.GetUnicodeCategory(ch);

            if (uc != UnicodeCategory.NonSpacingMark)
            {
                clean.Append(ch);
            }
        }

        return clean.ToString().Normalize(NormalizationForm.FormC);
    }

    /// semester == [1, 2, 3, 4, 5, 6]
    /// year == [1, 2, 3]
    public static bool SemesterBelongsToYear(int semester, int year)
    {
        return YearOfSemester(semester) == year;
    }

    public static int YearOfSemester(int semester) => (semester + 1) / 2;

    public static int FirstSemesterOfYear(int year) => year * 2 - 1;
    public static int SecondSemesterOfYear(int year) => year * 2;
}