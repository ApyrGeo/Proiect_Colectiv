using TrackForUBB.Domain;
using TrackForUBB.Domain.Enums;
using TrackForUBB.Repository.Interfaces;
using log4net;
using Microsoft.EntityFrameworkCore;
using TrackForUBB.Repository.Context;
using System.Text.Json;
using TrackForUBB.Domain.Utils;

namespace TrackForUBB.Repository;

public class TimetableRepository(AcademicAppContext context) : ITimetableRepository
{
    private readonly AcademicAppContext _context = context;
    private readonly ILog _logger = LogManager.GetLogger(typeof(UserRepository));

    public async Task<Classroom> AddClassroomAsync(Classroom classroom)
    {
        _logger.InfoFormat("Adding new classroom with name: {0}", classroom.Name);

        await _context.Classrooms.AddAsync(classroom);
        return classroom;
    }

    public async Task<Hour> AddHourAsync(Hour hour)
    {
        _logger.InfoFormat("Adding new hour with interval: {0}", hour.HourInterval);

        await _context.Hours.AddAsync(hour);
        return hour;
    }

    public async Task<Location> AddLocationAsync(Location location)
    {
        _logger.InfoFormat("Adding new location with name: {0}", location.Name);

        await _context.Locations.AddAsync(location);
        return location;
    }

    public async Task<Subject> AddSubjectAsync(Subject subject)
    {
        _logger.InfoFormat("Adding new subject with name: {0}", subject.Name);

        await _context.Subjects.AddAsync(subject);
        return subject;
    }

    public async Task<Classroom?> GetClassroomByIdAsync(int id)
    {
        _logger.InfoFormat("Fetching classroom by ID: {0}", id);

        return await _context.Classrooms
            .Include(x => x.Location)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Hour?> GetHourByIdAsync(int id)
    {
        _logger.InfoFormat("Fetching hours by ID: {0}", id);

        return await _context.Hours
            .Include(x => x.Classroom)
                .ThenInclude(x => x.Location)
            .Include(x => x.Teacher)
                .ThenInclude(x => x.User)
            .Include(x => x.Subject)
            .Include(x => x.GroupYear)
            .Include(x => x.StudentGroup)
            .Include(x => x.StudentSubGroup)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Hour>> GetHoursAsync(HourFilter filter)
    {
        _logger.InfoFormat("Fetching hours by hour filter: {0}", JsonSerializer.Serialize(filter));

        var query = _context.Hours.AsQueryable();

        if (filter.UserId != null)
        {
            var enrollmentInfo = await _context.Enrollments
                .Where(e => e.UserId == filter.UserId)
                .Select(e => new
                {
                    e.SubGroupId,
                    e.SubGroup.StudentGroupId,
                    e.SubGroup.StudentGroup.GroupYearId
                }).ToListAsync();

            var subGroupIds = enrollmentInfo.Select(x => x.SubGroupId).Distinct().ToList();
            var studentGroupIds = enrollmentInfo.Select(x => x.StudentGroupId).Distinct().ToList();
            var groupYearIds = enrollmentInfo.Select(x => x.GroupYearId).Distinct().ToList();

            query = query.Where(h =>
                (h.StudentSubGroupId != null && subGroupIds.Contains(h.StudentSubGroupId.Value))
                || (h.StudentGroupId != null && studentGroupIds.Contains(h.StudentGroupId.Value))
                || (h.GroupYearId != null && groupYearIds.Contains(h.GroupYearId.Value))
            );
        }

        if (filter.CurrentWeekTimetable != null && filter.CurrentWeekTimetable == true)
        {
            query = query.Where(h => h.Frequency == HourFrequency.Weekly || h.Frequency == Constants.CurrentWeekType);
        }

        if (filter.FacultyId != null)
        {
            query = query.Where(h =>
                _context.GroupYears.Any(gy => gy.Id == h.GroupYearId && gy.Specialisation.FacultyId == filter.FacultyId)
                || _context.Groups.Any(g => g.Id == h.StudentGroupId && g.GroupYear.Specialisation.FacultyId == filter.FacultyId)
                || _context.SubGroups.Any(sg => sg.Id == h.StudentSubGroupId && sg.StudentGroup.GroupYear.Specialisation.FacultyId == filter.FacultyId));
        }

        if (filter.SpecialisationId != null)
        {
            query = query.Where(h =>
                _context.GroupYears.Any(gy => gy.Id == h.GroupYearId && gy.SpecialisationId == filter.SpecialisationId)
                || _context.Groups.Any(g => g.Id == h.StudentGroupId && g.GroupYear.SpecialisationId == filter.SpecialisationId)
                || _context.SubGroups.Any(sg => sg.Id == h.StudentSubGroupId && sg.StudentGroup.GroupYear.SpecialisationId == filter.SpecialisationId));
        }

        if (filter.GroupYearId != null)
        {
            query = query.Where(h =>
                _context.GroupYears.Any(gy => gy.Id == h.GroupYearId && gy.Id == filter.GroupYearId)
                || _context.Groups.Any(g => g.Id == h.StudentGroupId && g.GroupYearId == filter.GroupYearId)
                || _context.SubGroups.Any(sg => sg.Id == h.StudentSubGroupId && sg.StudentGroup.GroupYearId == filter.GroupYearId));
        }

        if (filter.TeacherId != null)
        {
            query = query.Where(x => x.TeacherId == filter.TeacherId);
        }

        if (filter.SubjectId != null)
        {
            query = query.Where(x => x.SubjectId == filter.SubjectId);
        }

        if (filter.ClassroomId != null)
        {
            query = query.Where(x => x.ClassroomId == filter.ClassroomId);
        }

        return await query
            .Include(x => x.Classroom)
                .ThenInclude(x => x.Location)
            .Include(x => x.Teacher)
                .ThenInclude(x => x.User)
            .Include(x => x.Subject)
            .Include(x => x.GroupYear)
            .Include(x => x.StudentGroup)
            .Include(x => x.StudentSubGroup)
            .OrderBy(x => x.Day)
                .ThenBy(x => x.HourInterval)
            .ToListAsync();
    }

    public async Task<Location?> GetLocationByIdAsync(int id)
    {
        _logger.InfoFormat("Fetching location by ID: {0}", id);

        return await _context.Locations
            .Include(x => x.Classrooms)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Subject?> GetSubjectByIdAsync(int id)
    {
        return await _context.Subjects.FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<Subject?> GetSubjectByNameAsync(string name)
    {
        return await _context.Subjects.FirstOrDefaultAsync(f => f.Name == name);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}