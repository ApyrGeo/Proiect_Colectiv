using TrackForUBB.Domain.DTOs;

namespace TrackForUBB.Service.Interfaces;

public interface IGradeRepository
{
    Task<GradeResponseDTO> AddGradeAsync(GradePostDTO gradePostDTO);
    Task<List<GradeResponseDTO>> GetGradesFilteredAsync(int? userId, int? yearOfStudy, int? semester, string specialisation);
    Task<List<GradeResponseDTO>> GetGradesForStudentInSemesterAsync(int enrollmentId, int semesterId);
    Task<List<SubjectResponseDTO>> GetSubjectsForSemesterAsync(int semesterId);
    Task<GradeResponseDTO?> GetGradeByIdAsync(int gradeId);
    Task<bool> TeacherTeachesSubjectAsync(int teacherId, int subjectId);
    Task<GradeResponseDTO> GetGradeByEnrollmentAndSubjectAsync(int arg1EnrollmentId, int arg1SubjectId);
    Task<GradeResponseDTO> UpdateGradeAsync(int gradeId, GradePostDTO dto);
    Task<GradeResponseDTO> PatchGradeValueAsync(int gradeId, int newValue);
    Task SaveChangesAsync();
}