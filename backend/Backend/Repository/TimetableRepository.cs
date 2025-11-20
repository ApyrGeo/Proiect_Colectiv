using AutoMapper;
using log4net;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Enums;
using TrackForUBB.Domain.Utils;
using TrackForUBB.Repository.Context;
using TrackForUBB.Repository.EFEntities;
using TrackForUBB.Service.Interfaces;

namespace TrackForUBB.Repository;

public class TimetableRepository(AcademicAppContext context, IMapper mapper) : ITimetableRepository
{
    private readonly AcademicAppContext _context = context;
	private readonly IMapper _mapper = mapper;
	private readonly ILog _logger = LogManager.GetLogger(typeof(UserRepository));

    public async Task<ClassroomResponseDTO> AddClassroomAsync(ClassroomPostDTO classroom)
    {
        _logger.InfoFormat("Adding new classroom with name: {0}", classroom.Name);

		var entity = _mapper.Map<Classroom>(classroom);
		await _context.Classrooms.AddAsync(entity);

		return _mapper.Map<ClassroomResponseDTO>(entity);
	}

    public async Task<HourResponseDTO> AddHourAsync(HourPostDTO hour)
    {
        _logger.InfoFormat("Adding new hour with interval: {0}", hour.HourInterval);

		var entity = _mapper.Map<Hour>(hour);
		await _context.Hours.AddAsync(entity);

		return _mapper.Map<HourResponseDTO>(entity);
	}

    public async Task<LocationResponseDTO> AddLocationAsync(LocationPostDTO location)
    {
        _logger.InfoFormat("Adding new location with name: {0}", location.Name);

		var entity = _mapper.Map<Location>(location);
		await _context.Locations.AddAsync(entity);

		return _mapper.Map<LocationResponseDTO>(entity);
	}

    public async Task<SubjectResponseDTO> AddSubjectAsync(SubjectPostDTO subject)
    {
        _logger.InfoFormat("Adding new subject with name: {0}", subject.Name);

		var entity = _mapper.Map<Subject>(subject);
		await _context.Subjects.AddAsync(entity);

		return _mapper.Map<SubjectResponseDTO>(entity);
	}

    public async Task<ClassroomResponseDTO?> GetClassroomByIdAsync(int id)
    {
        _logger.InfoFormat("Fetching classroom by ID: {0}", id);

        var classroom = await _context.Classrooms
            .Include(x => x.Location)
            .FirstOrDefaultAsync(x => x.Id == id);

		return _mapper.Map<ClassroomResponseDTO>(classroom);
	}

    public async Task<HourResponseDTO?> GetHourByIdAsync(int id)
    {
        _logger.InfoFormat("Fetching hours by ID: {0}", id);

        var hour = await _context.Hours
            .Include(x => x.Classroom)
                .ThenInclude(x => x.Location)
            .Include(x => x.Teacher)
                .ThenInclude(x => x.User)
            .Include(x => x.Subject)
            .Include(x => x.GroupYear)
            .Include(x => x.StudentGroup)
            .Include(x => x.StudentSubGroup)
            .FirstOrDefaultAsync(x => x.Id == id);

		return _mapper.Map<HourResponseDTO>(hour);
	}

    public async Task<List<HourResponseDTO>> GetHoursAsync(HourFilter filter)
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
            query = query.Where(h => h.Frequency == HourFrequency.Weekly || h.Frequency == HelperFunctions.CurrentWeekType);
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

        var hours = await query
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

		return _mapper.Map<List<HourResponseDTO>>(hours);
	}

    public async Task<LocationResponseDTO?> GetLocationByIdAsync(int id)
    {
        _logger.InfoFormat("Fetching location by ID: {0}", id);

        var location = await _context.Locations
            .Include(x => x.Classrooms)
            .FirstOrDefaultAsync(x => x.Id == id);

		return _mapper.Map<LocationResponseDTO>(location);
	}

    public async Task<SubjectResponseDTO?> GetSubjectByIdAsync(int id)
    {
        var subject = await _context.Subjects.FirstOrDefaultAsync(f => f.Id == id);

		return _mapper.Map<SubjectResponseDTO>(subject);
	}

	public async Task<SubjectResponseDTO?> GetSubjectByNameAsync(string name)
    {
        var subject = await _context.Subjects.FirstOrDefaultAsync(f => f.Name == name);

		return _mapper.Map<SubjectResponseDTO>(subject);
	}

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}