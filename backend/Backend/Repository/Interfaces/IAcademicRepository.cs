using TrackForUBB.Domain;

namespace TrackForUBB.Repository.Interfaces;

public interface IAcademicRepository
{
    Task<Enrollment> AddEnrollmentAsync(Enrollment enrollment);
    Task<List<Enrollment>> GetEnrollmentsByUserId(int userId);
    Task<Teacher> AddTeacherAsync(Teacher teacher);
    Task<Teacher?> GetTeacherById(int id);
    Task<Faculty> AddFacultyAsync(Faculty faculty);
    Task<Faculty?> GetFacultyByIdAsync(int id);
    Task<Faculty?> GetFacultyByNameAsync(string name);
    Task<GroupYear> AddGroupYearAsync(GroupYear groupYear);
    Task<GroupYear?> GetGroupYearByIdAsync(int id);
    Task<Specialisation> AddSpecialisationAsync(Specialisation specialisation);
    Task<Specialisation?> GetSpecialisationByIdAsync(int id);
    Task<StudentGroup> AddGroupAsync(StudentGroup studentGroup);
    Task<StudentGroup?> GetGroupByIdAsync(int id);
    Task<StudentSubGroup> AddSubGroupAsync(StudentSubGroup studentSubGroup);
    Task<StudentSubGroup?> GetSubGroupByIdAsync(int id);
    Task SaveChangesAsync();
}