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
                .ThenInclude(c => c.Location)
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
                .ThenInclude(c => c.Location)
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
            {                var newEntry = _mapper.Map<ExamEntry>(examEntryDto);
                await _context.ExamEntries.AddAsync(newEntry);
            }
        }

        await _context.SaveChangesAsync();

        var savedEntries = await _context.ExamEntries
            .Include(e => e.Subject)
            .Include(e => e.Classroom)
                .ThenInclude(c => c.Location)
            .Include(e => e.StudentGroup)
            .Where(e => entryIds.Contains(e.Id))
            .ToListAsync();

        return _mapper.Map<List<ExamEntryResponseDTO>>(savedEntries);
    }
}