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

        await _context.SaveChangesAsync();

        return _mapper.Map<ClassroomResponseDTO>(entity);
    }

    public async Task<HourResponseDTO> AddHourAsync(HourPostDTO hour)
    {
        _logger.InfoFormat("Adding new hour with interval: {0}", hour.HourInterval);

        var entity = _mapper.Map<Hour>(hour);
        await _context.Hours.AddAsync(entity);

        await _context.SaveChangesAsync();

        return _mapper.Map<HourResponseDTO>(entity);
    }

    public async Task<LocationResponseDTO> AddLocationAsync(LocationPostDTO location)
    {
        _logger.InfoFormat("Adding new location with name: {0}", location.Name);

        var entity = _mapper.Map<Location>(location);
        await _context.Locations.AddAsync(entity);

        await _context.SaveChangesAsync();

        return _mapper.Map<LocationResponseDTO>(entity);
    }

    public async Task<SubjectResponseDTO> AddSubjectAsync(SubjectPostDTO subject)
    {
        _logger.InfoFormat("Adding new subject with name: {0}", subject.Name);

        var entity = _mapper.Map<Subject>(subject);
        await _context.Subjects.AddAsync(entity);

        await _context.SaveChangesAsync();

        return _mapper.Map<SubjectResponseDTO>(entity);
    }

    public Task DeleteHoursBySpecializationAsync(int specializationId)
    {
        _logger.InfoFormat("Deleting hours for specialization ID: {0}", specializationId);
        var hoursToDelete = _context.Hours.Where(h =>
            _context.Promotions.Any(p => p.Id == h.PromotionId && p.SpecialisationId == specializationId)
            || _context.Groups.Any(g => g.Id == h.StudentGroupId && g.Promotion.SpecialisationId == specializationId)
            || _context.SubGroups.Any(sg => sg.Id == h.StudentSubGroupId && sg.StudentGroup.Promotion.SpecialisationId == specializationId)
        );
        _context.Hours.RemoveRange(hoursToDelete);
        return _context.SaveChangesAsync();
    }

    public async Task<List<HourResponseDTO>> GenerateTimetableAsync(TimetableGenerationDTO dto)
    {
        _logger.InfoFormat("Generating timetable for DTO: {0}", JsonSerializer.Serialize(dto));

        var semester = await _context.PromotionSemesters
            .Include(s => s.Subjects)
            .Include(s => s.Promotion)
                .ThenInclude(p => p.Specialisation)
            .FirstOrDefaultAsync(s => s.Id == dto.SemesterId);

        if (semester == null)
        {
            _logger.WarnFormat("Semester with ID {0} not found", dto.SemesterId);
            return [];
        }

        if (semester.Promotion.SpecialisationId != dto.SpecialisationId)
        {
            _logger.WarnFormat("Semester {0} does not belong to specialisation {1}", dto.SemesterId, dto.SpecialisationId);
            return [];
        }

        var currentPromotion = semester.Promotion;
        var hours = new List<Hour>();

        _logger.InfoFormat("Processing promotion ID: {0}", currentPromotion.Id);

        var groups = await _context.Groups
            .Include(g => g.StudentSubGroups)
            .Where(g => g.PromotionId == currentPromotion.Id)
            .ToListAsync();

        if (groups.Count == 0)
        {
            _logger.WarnFormat("No groups found for promotion ID: {0}", currentPromotion.Id);
            return [];
        }

        var subjects = semester.Subjects;

        foreach (var subject in subjects)
        {
            int seminarsCount = subject.FormationType switch
            {
                SubjectFormationType.Course_Seminar
                | SubjectFormationType.Course_Seminar_Laboratory
                    => groups.Count,
                _ => 0,
            };
            int labsCount = subject.FormationType switch
            {
                SubjectFormationType.Course_Laboratory
                | SubjectFormationType.Course_Seminar_Laboratory
                    => groups.Sum(x => x.StudentSubGroups.Count),
                _ => 0,
            };

            // Generate course hour
            var courseHour = new Hour
            {
                Day = HourDay.Unknown,
                HourInterval = "00:00-00:00",
                Frequency = HourFrequency.Weekly,
                Category = HourCategory.Lecture,
                SubjectId = subject.Id,
                Subject = subject,
                ClassroomId = null,
                TeacherId = null,
                PromotionId = currentPromotion.Id,
            };
            hours.Add(courseHour);

            // Generate seminar hours
            for (int i = 0; i < seminarsCount; i++)
            {
                var seminarHour = new Hour
                {
                    Day = HourDay.Unknown,
                    HourInterval = "00:00-00:00",
                    Frequency = HourFrequency.Weekly,
                    Category = HourCategory.Seminar,
                    SubjectId = subject.Id,
                    Subject = subject,
                    ClassroomId = null,
                    TeacherId = null,
                    StudentGroupId = groups[i].Id
                };
                hours.Add(seminarHour);
            }

            // Generate laboratory hours
            var subgroups = groups.SelectMany(x => x.StudentSubGroups).ToList();
            foreach (var subgroup in subgroups)
            {
                var labHour = new Hour
                {
                    Day = HourDay.Unknown,
                    HourInterval = "00:00-00:00",
                    Frequency = HourFrequency.Weekly,
                    Category = HourCategory.Laboratory,
                    SubjectId = subject.Id,
                    Subject = subject,
                    ClassroomId = null,
                    TeacherId = null,
                    StudentSubGroupId = subgroup.Id,
                };
                hours.Add(labHour);
            }
        }

        await _context.AddRangeAsync(hours);
        await _context.SaveChangesAsync();
        return _mapper.Map<List<HourResponseDTO>>(hours);
    }

    public Task<List<LocationWithClassroomsResponseDTO>> GetAllLocationsAsync()
    {
        _logger.Info("Fetching all locations");
        return _context.Locations
            .Include(x => x.Classrooms)
            .Select(location => _mapper.Map<LocationWithClassroomsResponseDTO>(location))
            .ToListAsync();
    }

    public async Task<ClassroomResponseDTO?> GetClassroomByIdAsync(int id)
    {
        _logger.InfoFormat("Fetching classroom by ID: {0}", id);

        var classroom = await _context.Classrooms
            .Include(x => x.Location)
            .FirstOrDefaultAsync(x => x.Id == id);

        return _mapper.Map<ClassroomResponseDTO>(classroom);
    }

    public async Task<List<StudentGroupResponseDTO>> GetGroupsBySubjectIdAsync(int subjectId)
    {
        _logger.InfoFormat("Fetching groups by subject ID: {0}", subjectId);

        var subject = await _context.Subjects
            .Include(s => s.Semester)
                .ThenInclude(sem => sem.Promotion)
                    .ThenInclude(p => p.StudentGroups)
                        .ThenInclude(g => g.StudentSubGroups)
            .FirstOrDefaultAsync(s => s.Id == subjectId);

        return _mapper.Map<List<StudentGroupResponseDTO>>(subject!.Semester.Promotion.StudentGroups);
    }
    public async Task<HourResponseDTO?> GetHourByIdAsync(int id)
    {
        _logger.InfoFormat("Fetching hours by ID: {0}", id);

        var hour = await _context.Hours
            .Include(x => x.Classroom)
                .ThenInclude(x => x!.Location)
            .Include(x => x.Teacher)
                .ThenInclude(x => x!.User)
            .Include(x => x.Subject)
            .Include(x => x.Promotion)
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
                    e.SubGroup.StudentGroup.PromotionId
                }).ToListAsync();

            var subGroupIds = enrollmentInfo.Select(x => x.SubGroupId).Distinct().ToList();
            var studentGroupIds = enrollmentInfo.Select(x => x.StudentGroupId).Distinct().ToList();
            var promotionIds = enrollmentInfo.Select(x => x.PromotionId).Distinct().ToList();

            query = query.Where(h =>
                (h.StudentSubGroupId != null && subGroupIds.Contains(h.StudentSubGroupId.Value))
                || (h.StudentGroupId != null && studentGroupIds.Contains(h.StudentGroupId.Value))
                || (h.PromotionId != null && promotionIds.Contains(h.PromotionId.Value))
            );
        }

        if (filter.CurrentWeekTimetable != null && filter.CurrentWeekTimetable == true)
        {
            query = query.Where(h => h.Frequency == HourFrequency.Weekly || h.Frequency == HelperFunctions.CurrentWeekType);
        }

        if (filter.FacultyId != null)
        {
            query = query.Where(h =>
                _context.Promotions.Any(gy => gy.Id == h.PromotionId && gy.Specialisation.FacultyId == filter.FacultyId)
                || _context.Groups.Any(g => g.Id == h.StudentGroupId && g.Promotion.Specialisation.FacultyId == filter.FacultyId)
                || _context.SubGroups.Any(sg => sg.Id == h.StudentSubGroupId && sg.StudentGroup.Promotion.Specialisation.FacultyId == filter.FacultyId));
        }

        if (filter.SpecialisationId != null)
        {
            query = query.Where(h =>
                _context.Promotions.Any(gy => gy.Id == h.PromotionId && gy.SpecialisationId == filter.SpecialisationId)
                || _context.Groups.Any(g => g.Id == h.StudentGroupId && g.Promotion.SpecialisationId == filter.SpecialisationId)
                || _context.SubGroups.Any(sg => sg.Id == h.StudentSubGroupId && sg.StudentGroup.Promotion.SpecialisationId == filter.SpecialisationId));
        }

        if (filter.GroupYearId != null)
        {
            query = query.Where(h =>
                _context.Promotions.Any(gy => gy.Id == h.PromotionId && gy.Id == filter.GroupYearId)
                || _context.Groups.Any(g => g.Id == h.StudentGroupId && g.PromotionId == filter.GroupYearId)
                || _context.SubGroups.Any(sg => sg.Id == h.StudentSubGroupId && sg.StudentGroup.PromotionId == filter.GroupYearId));
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

        if (filter.SemesterNumber != null)
        {
            query = query.Where(h => h.Subject.Semester.SemesterNumber % 2 == filter.SemesterNumber);
        }

        var hours = await query
            .Include(x => x.Classroom)
                .ThenInclude(x => x!.Location)
            .Include(x => x.Teacher)
                .ThenInclude(x => x!.User)
            .Include(x => x.Subject)
            .Include(x => x.Promotion)
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

    public async Task<List<SubjectResponseDTO>> GetSubjectsByHolderTeacherIdAsync(int teacherId)
    {
        _logger.InfoFormat("Fetching subjects held by teacher with ID: {0}", teacherId);
        return await _context.Subjects
            .Include(s => s.HolderTeacher)
                .ThenInclude(ht => ht.User)
            .Where(s => s.HolderTeacher.Id == teacherId)
            .Select(subject => _mapper.Map<SubjectResponseDTO>(subject))
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<HourResponseDTO> UpdateHourAsync(int hourId, IntermediaryHourDTO dto)
    {
        _logger.InfoFormat("Updating hour with ID: {0}", hourId);

        var hour = await _context.Hours.FirstOrDefaultAsync(h => h.Id == hourId)
            ?? throw new KeyNotFoundException($"Hour with ID {hourId} not found");

        if (dto.HourInterval != null)
        {
            hour.HourInterval = dto.HourInterval;
        }

        hour.Day = dto.Day;
        hour.Frequency = dto.Frequency;
        hour.Category = dto.Category;
        hour.ClassroomId = dto.ClassroomId;
        hour.SubjectId = dto.SubjectId;
        hour.TeacherId = dto.TeacherId;
        hour.PromotionId = dto.GroupYearId;
        hour.StudentGroupId = dto.StudentGroupId;
        hour.StudentSubGroupId = dto.StudentSubGroupId;

        _context.Hours.Update(hour);
        await _context.SaveChangesAsync();

        var updatedHour = await _context.Hours
            .Include(x => x.Classroom)
                .ThenInclude(x => x!.Location)
            .Include(x => x.Teacher)
                .ThenInclude(x => x!.User)
            .Include(x => x.Subject)
            .Include(x => x.Promotion)
            .Include(x => x.StudentGroup)
            .Include(x => x.StudentSubGroup)
            .FirstOrDefaultAsync(x => x.Id == hourId);

        return _mapper.Map<HourResponseDTO>(updatedHour);
    }

    public async Task<List<OptionalPackageResponseDTO>> GetOptionalSubjectsByPromotionIdAsync(int promotionId, int year)
    {
        _logger.InfoFormat("Fetching optional subjects for promotion ID: {0}", promotionId);

        var subjects = await _context.Subjects
            .Where(s =>
                s.Semester.PromotionId == promotionId && 
                (s.Semester.SemesterNumber == 2 * year - 1 || s.Semester.SemesterNumber == 2 * year) &&
                s.OptionalPackage != null &&
                s.Type == SubjectType.Optional
                )
            .Include(s => s.Semester)
            .Include(s => s.HolderTeacher)
            .ToListAsync();
        
        var grouped = subjects
            .GroupBy(s => (s.Semester.SemesterNumber, package: s.OptionalPackage!.Value))
            .Select(g => new OptionalPackageResponseDTO
            {
                PackageId = g.Key.package,
                SemesterNumber = g.Key.SemesterNumber,
                Semester1or2 = 2 - g.Key.SemesterNumber % 2,
                YearNumber = (g.Key.SemesterNumber + 1) / 2,
                Subjects = _mapper.Map<List<SubjectResponseDTO>>(g.ToList())
            })
            .OrderBy(x => x.YearNumber)
                .ThenBy(x => x.SemesterNumber)
                    .ThenBy(x => x.PackageId)
            .ToList();

        return grouped;
        
    }
}
