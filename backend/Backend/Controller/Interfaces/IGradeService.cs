using TrackForUBB.Domain.DTOs;

namespace TrackForUBB.Controller.Interfaces;

public interface IGradeService
{
    Task<GradeResponseDTO> CreateGrade(int teacherId,GradePostDTO gradePostDto);
    Task<List<GradeResponseDTO>> GetGradesFiteredAsync(int userId, int? yearOfStudy, int? semester);
}