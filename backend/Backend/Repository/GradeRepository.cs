using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Utils;
using TrackForUBB.Repository.Context;
using TrackForUBB.Repository.EFEntities;
using TrackForUBB.Service.Interfaces;

namespace TrackForUBB.Repository;

public class GradeRepository(AcademicAppContext context, IMapper mapper) : IGradeRepository
{
    private readonly AcademicAppContext _context = context;
    private readonly IMapper _mapper = mapper;


    public async Task<GradeResponseDTO> AddGradeAsync(GradePostDTO gradePostDTO)
    {
        var entity = _mapper.Map<Grade>(gradePostDTO);
        await _context.Grades.AddAsync(entity);
        await SaveChangesAsync();
        var fullEntity = await _context.Grades
            .Include(g => g.Subject)
                .ThenInclude(s => s.Semester)
                    .ThenInclude(py => py.Promotion)
                        .ThenInclude(p => p.Specialisation)
                            .ThenInclude(s => s.Faculty)
            .Include(g => g.Enrollment)
                .ThenInclude(e => e.SubGroup)
                    .ThenInclude(sg => sg.StudentGroup)
            .Include(g => g.Enrollment)
                .ThenInclude(e => e.User)
            .FirstOrDefaultAsync(g => g.Id == entity.Id);

        await _context.SaveChangesAsync();

        return _mapper.Map<GradeResponseDTO>(fullEntity);
    }

    public async Task<List<GradeResponseDTO>> GetGradesFilteredAsync(int? userId, int? yearOfStudy, int? semester, string specialisation)
    {
        var query = _context.Grades
            .Include(g => g.Subject)
                .ThenInclude(g => g.Semester)
                    .ThenInclude(py => py.Promotion)
                        .ThenInclude(p => p.Specialisation)
                            .ThenInclude(s => s.Faculty)
            .Include(g => g.Enrollment)
                .ThenInclude(e => e.User)
            .Include(g => g.Enrollment)
                 .ThenInclude(e => e.SubGroup)
                    .ThenInclude(sg => sg.StudentGroup)
            .AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(g => g.Enrollment.UserId == userId);
        }

        if (yearOfStudy.HasValue)
        {
            query = query.Where(x => (x.Subject.Semester.SemesterNumber + 1) / 2 == yearOfStudy);
        }

        if (semester.HasValue)
        {
            query = query.Where(x => x.Subject.Semester.SemesterNumber % 2 == semester % 2);
        }

        if (!string.IsNullOrWhiteSpace(specialisation))
        {
            query = query.Where(g => g.Enrollment.SubGroup.StudentGroup.Promotion.Specialisation.Name == specialisation);
        }

        var grades = await query.ToListAsync();

        return _mapper.Map<List<GradeResponseDTO>>(grades);
    }

    public async Task<List<GradeResponseDTO>> GetGradesForStudentInSemesterAsync(int enrollmentId, int semesterId)
    {
        var grades = await _context.Grades
            .Include(g => g.Subject)
            .Where(g => g.EnrollmentId == enrollmentId && g.Subject.SemesterId == semesterId)
            .ToListAsync();

        return _mapper.Map<List<GradeResponseDTO>>(grades);
    }

    public async Task<List<SubjectResponseDTO>> GetSubjectsForSemesterAsync(int semesterId)
    {
        var subjects = await _context.Hours
            .Where(h => h.Subject != null ? h.Subject.SemesterId == semesterId : false)
            .Select(h => h.Subject)
            .Distinct()
            .ToListAsync();

        return _mapper.Map<List<SubjectResponseDTO>>(subjects);
    }

    public async Task<GradeResponseDTO?> GetGradeByIdAsync(int gradeId)
    {
        var grade = await _context.Grades.Where(g => g.Id == gradeId)
            .Include(g => g.Subject)
                 .ThenInclude(s => s.Semester)
                    .ThenInclude(py => py.Promotion)
                        .ThenInclude(p => p.Specialisation)
                            .ThenInclude(s => s.Faculty)
            .Include(g => g.Enrollment)
            .FirstOrDefaultAsync();
        return _mapper.Map<GradeResponseDTO>(grade);
    }

    public async Task<bool> TeacherTeachesSubjectAsync(int teacherId, int subjectId)
    {
        return await _context.Hours
            .AnyAsync(h =>
                h.TeacherId == teacherId &&
                h.SubjectId == subjectId);
    }

    public async Task<GradeResponseDTO> GetGradeByEnrollmentAndSubjectAsync(int arg1EnrollmentId, int arg1SubjectId)
    {
        var grade = await _context.Grades.Where(g => g.EnrollmentId == arg1EnrollmentId && g.SubjectId == arg1SubjectId)
            .FirstOrDefaultAsync();
        return _mapper.Map<GradeResponseDTO>(grade);
    }

    public async Task<GradeResponseDTO> UpdateGradeAsync(int gradeId, GradePostDTO dto)
    {
        var grade = await _context.Grades.FindAsync(gradeId);
        if (grade is null)
            throw new Exception($"There is not grade for grarde id {gradeId}");

        grade.Value = dto.Value;
        grade.SubjectId = dto.SubjectId;
        grade.EnrollmentId = dto.EnrollmentId;

        await _context.SaveChangesAsync();

        return _mapper.Map<GradeResponseDTO>(grade);
    }

    public async Task<GradeResponseDTO> PatchGradeValueAsync(int gradeId, int newValue)
    {
        var grade = await _context.Grades.FindAsync(gradeId);
        if (grade is null)
            throw new Exception($"There is not grade with id {gradeId}");

        grade.Value = newValue;
        
        await _context.SaveChangesAsync(); 
        return _mapper.Map<GradeResponseDTO>(grade);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
