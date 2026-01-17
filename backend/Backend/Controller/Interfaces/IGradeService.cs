using TrackForUBB.Domain.DTOs;

namespace TrackForUBB.Controller.Interfaces;

public interface IGradeService
{
    Task<GradeResponseDTO> CreateGrade(int teacherId, GradePostDTO gradePostDto);
    Task<List<GradeResponseDTO>> GetGradesFiteredAsync(int userId, int? yearOfStudy, int? semester, int? promotionId);
    Task<GradeResponseDTO> GetGradeByIdAsync(int gradeId);
    Task<ScholarshipStatusDTO?> GetUserAverageScoreAndScholarshipStatusAsync(int userId, int year, int semester, int promotionId);
    Task<GradeResponseDTO> UpdateGradeAsync(int teacherId, int gradeId, GradePostDTO dto);
    Task<GradeResponseDTO> PatchGradeAsync(int teacherId, int gradeId, int newValue);
    Task<SubjectGroupGradesDTO> GetSubjectGroupsAsync(int subjectId, int groupId);
}
