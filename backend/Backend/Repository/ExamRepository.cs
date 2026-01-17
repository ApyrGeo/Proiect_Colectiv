using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Repository.Context;
using TrackForUBB.Repository.EFEntities;
using TrackForUBB.Service.Interfaces;

namespace TrackForUBB.Repository;

public class ExamRepository(AcademicAppContext context, IMapper mapper) : IExamRepository
{
    private readonly AcademicAppContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<ExamEntryResponseDTO> CreateExamEntryAsync(ExamEntryRequestDTO examEntry)
    {
        var newEntry = new ExamEntry
        {
            ExamDate = examEntry.Date,
            ClassroomId = examEntry.ClassroomId,
            SubjectId = examEntry.SubjectId,
            StudentGroupId = examEntry.StudentGroupId,
            Classroom = null,
            Subject = null!,
            StudentGroup = null!,
            Duration = (int?)examEntry.Duration?.TotalMinutes,
        };

        await _context.ExamEntries.AddAsync(newEntry);
        await _context.SaveChangesAsync();

        var savedEntry = await _context.ExamEntries
            .AsNoTracking()
            .Include(e => e.Subject)
            .Include(e => e.Classroom)
                .ThenInclude(c => c!.Location)
            .Include(e => e.StudentGroup)
            .FirstOrDefaultAsync(e => e.Id == newEntry.Id);

        return _mapper.Map<ExamEntryResponseDTO>(savedEntry);
    }

    public async Task<List<ExamEntryForStudentDTO>> GetStudentExamsByStudentId(int studentId)
    {
        var enrollments = await _context.Enrollments
            .Where(e => e.UserId == studentId)
            .Include(e => e.SubGroup)
                .ThenInclude(sg => sg.StudentGroup)
                    .ThenInclude(g => g.Promotion)
                        .ThenInclude(p => p.Specialisation)
            .ToListAsync();

        if (enrollments.Count == 0)
            return [];

        var studentGroupIds = enrollments
            .Select(e => e.SubGroup.StudentGroup.Id)
            .Distinct()
            .ToList();

        var exams = await _context.ExamEntries
            .Where(exam => studentGroupIds.Contains(exam.StudentGroupId))
            .Include(exam => exam.Subject)
            .Include(exam => exam.Classroom)
                .ThenInclude(c => c!.Location)
            .Include(exam => exam.StudentGroup)
                .ThenInclude(sg => sg.Promotion)
                    .ThenInclude(p => p.Specialisation)
            .ToListAsync();

        var result = new List<ExamEntryForStudentDTO>();

        var enrollmentsBySpecialisation = enrollments
            .GroupBy(e => new
            {
                SpecialisationId = e.SubGroup.StudentGroup.Promotion.Specialisation.Id,
                SpecialisationName = e.SubGroup.StudentGroup.Promotion.Specialisation.Name
            });

        foreach (var group in enrollmentsBySpecialisation)
        {
            var groupIds = group
                .Select(e => e.SubGroup.StudentGroup.Id)
                .Distinct()
                .ToList();

            var examsForSpecialisation = exams
                .Where(exam => groupIds.Contains(exam.StudentGroupId))
                .ToList();

            result.Add(new ExamEntryForStudentDTO
            {
                Specialisation = group.Key.SpecialisationName,
                ExamEntries = _mapper.Map<List<ExamEntryResponseDTO>>(examsForSpecialisation)
            });
        }

        return result;
    }


    public async Task<List<ExamEntryResponseDTO>> GetExamsBySubjectId(int subjectId)
    {
        var examEntries = await _context.ExamEntries
            .Where(e => e.SubjectId == subjectId)
            .Include(e => e.Subject)
            .Include(e => e.Classroom)
                .ThenInclude(c => c!.Location)
            .Include(e => e.StudentGroup)
            .ToListAsync();

        return _mapper.Map<List<ExamEntryResponseDTO>>(examEntries);
    }

    public async Task<List<ExamEntryResponseDTO>> UpdateExamEntries(List<ExamEntryPutDTO> examEntries)
    {
        var entryIds = examEntries.Select(dto => dto.Id).ToList();
        var existingEntries = await _context.ExamEntries
            .Where(e => entryIds.Contains(e.Id))
            .ToDictionaryAsync(e => e.Id);

        foreach (var examEntryDto in examEntries)
        {
            if (existingEntries.TryGetValue(examEntryDto.Id, out var existingEntry))
            {
                _mapper.Map(examEntryDto, existingEntry);
            }
            else
            {
                var newEntry = _mapper.Map<ExamEntry>(examEntryDto);
                await _context.ExamEntries.AddAsync(newEntry);
            }
        }

        await _context.SaveChangesAsync();

        var savedEntries = await _context.ExamEntries
            .Include(e => e.Subject)
            .Include(e => e.Classroom)
                .ThenInclude(c => c!.Location)
            .Include(e => e.StudentGroup)
            .Where(e => entryIds.Contains(e.Id))
            .ToListAsync();

        return _mapper.Map<List<ExamEntryResponseDTO>>(savedEntries);
    }

    public Task<ExamEntryResponseDTO?> GetExamEntryBySubjectAndGroupAsync(int subjectId, int studentGroupId)
    {
        return _context.ExamEntries
            .Where(e => e.SubjectId == subjectId && e.StudentGroupId == studentGroupId)
            .Include(e => e.Subject)
            .Include(e => e.Classroom)
                .ThenInclude(c => c!.Location)
            .Include(e => e.StudentGroup)
            .Select(e => _mapper.Map<ExamEntryResponseDTO>(e))
            .FirstOrDefaultAsync()!;
    }

    public async Task DeleteExamEntryAsync(int id)
    {
        var examEntry = await _context.ExamEntries.Where(e => e.Id == id).FirstOrDefaultAsync();
        if (examEntry != null)
        {
            _context.ExamEntries.Remove(examEntry);
            await _context.SaveChangesAsync();
        }
    }

    public Task<ExamEntryResponseDTO?> GetExamEntryByIdAsync(int id)
    {
        return _context.ExamEntries
            .Where(e => e.Id == id)
            .Include(e => e.Subject)
            .Include(e => e.Classroom)
                .ThenInclude(c => c!.Location)
            .Include(e => e.StudentGroup)
            .Select(e => _mapper.Map<ExamEntryResponseDTO>(e))
            .FirstOrDefaultAsync();
    }
}
