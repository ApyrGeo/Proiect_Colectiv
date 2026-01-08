using Microsoft.AspNetCore.Http;
using TrackForUBB.Domain.DTOs;

namespace TrackForUBB.Controller.Interfaces;

public interface IAcademicsService
{
    Task<FacultyResponseDTO> CreateFaculty(FacultyPostDTO facultyPostDto);
    Task<PromotionResponseDTO> CreatePromotion(PromotionPostDTO promotionPostDto);
    Task<SpecialisationResponseDTO> CreateSpecialisation(SpecialisationPostDTO specialisationPostDto);
    Task<StudentGroupResponseDTO> CreateStudentGroup(StudentGroupPostDTO studentGroupPostDto);
    Task<StudentSubGroupResponseDTO> CreateStudentSubGroup(StudentSubGroupPostDTO studentSubGroupPostDto);
    Task<EnrollmentResponseDTO> CreateUserEnrollment(EnrollmentPostDTO enrollmentPostDto);
    Task<TeacherResponseDTO> CreateTeacher(TeacherPostDTO teacherPostDTO);
    Task<FacultyResponseDTO> GetFacultyById(int facultyId);
    Task<PromotionResponseDTO> GetPromotionById(int promotionId);
    Task<List<SpecialisationResponseDTO>> GetAllSpecialisations();
    Task<SpecialisationResponseDTO> GetSpecialisationById(int specialisationId);
    Task<StudentGroupResponseDTO> GetStudentGroupById(int studentGroupId);
    Task<StudentSubGroupResponseDTO> GetStudentSubGroupById(int studentSubGroupId);
    Task<List<EnrollmentResponseDTO>> GetUserEnrollments(int userId);
    Task<TeacherResponseDTO> GetTeacherById(int id);
    Task<EnrollmentResponseDTO?> GetEnrollmentById(int enrollmentId);
    Task<TeacherResponseDTO?> GetTeacherByUserId(int userId);
    Task<List<EnrollmentResponseDTO>> GetStudentsByStudentGroup(int studentGroupId);
    Task<List<FacultyResponseDTO>> GetAllFaculties();
    Task<List<TeacherResponseDTO>> GetAllTeachersByFacultyId(int facultyId);
    Task<BulkPromotionResultDTO> CreatePromotionBulk(PromotionPostDTO promotionDto, IFormFile file);
}
