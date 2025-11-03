using Backend.Domain.DTOs;

namespace Backend.Interfaces;

public interface IAcademicsService
{
    Task<FacultyResponseDTO> CreateFaculty(FacultyPostDTO facultyPostDto);
    Task<GroupYearResponseDTO> CreateGroupYear(GroupYearPostDTO groupYearPostDto);
    Task<SpecialisationResponseDTO> CreateSpecialisation(SpecialisationPostDTO specialisationPostDto);
    Task<StudentGroupResponseDTO> CreateStudentGroup(StudentGroupPostDTO studentGroupPostDto);
    Task<StudentSubGroupResponseDTO> CreateStudentSubGroup(StudentSubGroupPostDTO studentSubGroupPostDto);
    Task<EnrollmentResponseDTO> CreateUserEnrollment(EnrollmentPostDTO enrollmentPostDto);
    Task<TeacherResponseDTO> CreateTeacher(TeacherPostDTO teacherPostDTO);
    Task<FacultyResponseDTO> GetFacultyById(int facultyId);
    Task<GroupYearResponseDTO> GetGroupYearById(int groupYearId);
    Task<SpecialisationResponseDTO> GetSpecialisationById(int specialisationId);
    Task<StudentGroupResponseDTO> GetStudentGroupById(int studentGroupId);
    Task<StudentSubGroupResponseDTO> GetStudentSubGroupById(int studentSubGroupId);
    Task<List<EnrollmentResponseDTO>> GetUserEnrollments(int userId);
    Task<List<TeacherResponseDTO>> GetTeachers(int userId);
}
