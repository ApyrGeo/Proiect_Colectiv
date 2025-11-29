using TrackForUBB.Domain.DTOs;

namespace TrackForUBB.Service.Interfaces;

public interface IAcademicRepository
{
    Task<EnrollmentResponseDTO> AddEnrollmentAsync(EnrollmentPostDTO enrollment);
    Task<List<EnrollmentResponseDTO>> GetEnrollmentsByUserId(int userId);
    Task<TeacherResponseDTO> AddTeacherAsync(TeacherPostDTO teacher);
    Task<TeacherResponseDTO?> GetTeacherById(int id);
    Task<FacultyResponseDTO> AddFacultyAsync(FacultyPostDTO faculty);
    Task<FacultyResponseDTO?> GetFacultyByIdAsync(int id);
    Task<FacultyResponseDTO?> GetFacultyByNameAsync(string name);
    Task<PromotionResponseDTO> AddPromotionAsync(PromotionPostDTO promotion);
    Task<PromotionResponseDTO?> GetPromotionByIdAsync(int id);
    Task<SpecialisationResponseDTO> AddSpecialisationAsync(SpecialisationPostDTO specialisation);
    Task<SpecialisationResponseDTO?> GetSpecialisationByIdAsync(int id);
    Task<StudentGroupResponseDTO> AddGroupAsync(StudentGroupPostDTO studentGroup);
    Task<StudentGroupResponseDTO?> GetGroupByIdAsync(int id);
    Task<StudentSubGroupResponseDTO> AddSubGroupAsync(StudentSubGroupPostDTO studentSubGroup);
    Task<StudentSubGroupResponseDTO?> GetSubGroupByIdAsync(int id);
    Task SaveChangesAsync();
    Task<EnrollmentResponseDTO?> GetEnrollmentByIdAsync(int enrollmentId);
    Task<PromotionSemesterResponseDTO?> GetSemesterByIdAsync(int semesterId);
}