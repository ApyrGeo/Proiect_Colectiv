using TrackForUBB.Domain.DTOs;

namespace TrackForUBB.Service.Interfaces;

public interface IGradeRepository
{
    Task<GradeResponseDTO> AddGradeAsync(GradePostDTO gradePostDTO);
    Task<List<GradeResponseDTO>> GetGradesFilteredAsync(int userId, int? yearOfStudy, int? semester);
    Task<List<GradeResponseDTO>> GetGradesForStudentInSemesterAsync(int enrollmentId, int semesterId);
    Task<List<SubjectResponseDTO>> GetSubjectsForSemesterAsync(int semesterId);
    Task SaveChangesAsync();
}