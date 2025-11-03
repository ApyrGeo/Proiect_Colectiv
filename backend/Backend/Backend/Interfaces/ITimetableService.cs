using Backend.Domain.DTOs;

namespace Backend.Interfaces;

public interface ITimetableService
{
    Task<SubjectResponseDTO> CreateSubject(SubjectPostDTO facultyPostDto);
    Task<SubjectResponseDTO> GetSubjectById(int subjectId);
}