using Backend.Domain;

namespace Backend.Interfaces;

public interface IAcademicRepository
{
    Task<Enrollment> AddEnrollmentAsync(Enrollment enrollment);
    Task<List<Enrollment>> GetEnrollmentsByUserId(int userId);
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