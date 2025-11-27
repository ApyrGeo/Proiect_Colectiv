using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TrackForUBB.Domain.DTOs;
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
        
        return _mapper.Map<GradeResponseDTO>(entity);
    }
    
    public async Task<List<GradeResponseDTO>> GetGradesFilteredAsync(int userId, int? yearOfStudy, int? semester)
    {
        var query = _context.Grades
            .Include(g => g.Subject)
            .Include(g => g.Semester)
                .ThenInclude(s => s.PromotionYear)
                    .ThenInclude(py => py.Promotion)
                        .ThenInclude(p => p.Specialisation)
                            .ThenInclude(s => s.Faculty)
            .AsQueryable();
        
        query = query.Where(g => g.Enrollment.UserId == userId);

        if (yearOfStudy.HasValue)
        {
            query = query.Where(g => g.Semester.PromotionYear.YearNumber == yearOfStudy.Value);
        }

        if (semester.HasValue)
        {
            query = query.Where(g => g.Semester.SemesterNumber == semester.Value);
        }

        var grades = await query.ToListAsync();

        return _mapper.Map<List<GradeResponseDTO>>(grades);
    }

    public async Task<List<GradeResponseDTO>> GetGradesForStudentInSemesterAsync(int enrollmentId, int semesterId)
    {
        var grades= await _context.Grades
            .Where(g => g.EnrollmentId == enrollmentId && g.SemesterId == semesterId)
            .ToListAsync();
        
        return _mapper.Map<List<GradeResponseDTO>>(grades);
    }

    public async Task<List<SubjectResponseDTO>> GetSubjectsForSemesterAsync(int semesterId)
    {
        var subjects= await _context.Hours
            .Where(h => h.SemesterId == semesterId)
            .Select(h => h.Subject)
            .Distinct()
            .ToListAsync();
        
        return _mapper.Map<List<SubjectResponseDTO>>(subjects);
    }
    
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}