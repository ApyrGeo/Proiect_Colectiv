using TrackForUBB.Domain;
using TrackForUBB.Domain.Utils;

namespace TrackForUBB.Repository.Interfaces;

public interface ITimetableRepository
{
    Task<Subject> AddSubjectAsync(Subject subject);
    Task<Subject?> GetSubjectByIdAsync(int id);
    Task<Subject?> GetSubjectByNameAsync(string name);
    Task<Location?> GetLocationByIdAsync(int id);
    Task<Location> AddLocationAsync(Location location);
    Task<Classroom?> GetClassroomByIdAsync(int id);
    Task<Classroom> AddClassroomAsync(Classroom classroom);
    Task<List<Hour>> GetHoursAsync(HourFilter filter);
    Task<Hour?> GetHourByIdAsync(int id);
    Task<Hour> AddHourAsync(Hour hour);
    Task SaveChangesAsync();
}