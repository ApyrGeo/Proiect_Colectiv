using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Repository.Context;
using TrackForUBB.Repository.EFEntities;
using TrackForUBB.Service.Interfaces;

public class ExamRepository(AcademicAppContext context, IMapper mapper) : IExamRepository
{
    private readonly AcademicAppContext _context = context;
    private readonly IMapper _mapper = mapper;

    async Task<List<ExamEntryResponseDTO>> IExamRepository.GetExamsBySubjectId(int subjectId)
    {
        var examEntries = await _context.ExamEntries
            .Where(e => e.SubjectId == subjectId)
            .Select(e => new ExamEntryResponseDTO
            {
                Id = e.Id,
                Date = e.ExamDate == null ? DateTime.MinValue : e.ExamDate.Value,
                Duration = e.Duration == null ? 0 : e.Duration.Value,
                Subject = _mapper.Map<SubjectResponseDTO>(e.Subject),
                Classroom = _mapper.Map<ClassroomResponseDTO>(e.Classroom),
                StudentGroup = _mapper.Map<StudentGroupResponseDTO>(e.StudentGroup)
            })
            .ToListAsync();
        return examEntries;
    }

    async Task<List<ExamEntryResponseDTO>> IExamRepository.UpdateExamEntries(List<ExamEntryPutDTO> examEntries)
    {
        foreach (var examEntryDto in examEntries)
        {
            var existingEntry = await _context.ExamEntries.FindAsync(examEntryDto.Id);
            
            if (existingEntry != null)
            {
                // Update existing entry
                existingEntry.ExamDate = examEntryDto.Date;
                existingEntry.Duration = examEntryDto.Duration;
                existingEntry.ClassroomId = examEntryDto.ClassroomId;
                existingEntry.SubjectId = examEntryDto.SubjectId;
                existingEntry.StudentGroupId = examEntryDto.StudentGroupId;
                
                _context.ExamEntries.Update(existingEntry);
            }
            else
            {
                // Create new entry
                var newEntry = new ExamEntry
                {
                    ExamDate = examEntryDto.Date,
                    Duration = examEntryDto.Duration,
                    ClassroomId = examEntryDto.ClassroomId,
                    SubjectId = examEntryDto.SubjectId,
                    StudentGroupId = examEntryDto.StudentGroupId,
                    // Navigation properties will be loaded by EF Core
                    Classroom = null!,
                    Subject = null!,
                    StudentGroup = null!
                };
                
                await _context.ExamEntries.AddAsync(newEntry);
            }
        }

        await _context.SaveChangesAsync();

        // Retrieve all updated/created entries with their navigation properties
        var entryIds = examEntries.Select(dto => dto.Id).ToList();
        
        var savedEntries = await _context.ExamEntries
            .Include(e => e.Subject)
            .Include(e => e.Classroom)
            .Include(e => e.StudentGroup)
            .Where(e => entryIds.Contains(e.Id))
            .ToListAsync();

        return _mapper.Map<List<ExamEntryResponseDTO>>(savedEntries);
    }
}