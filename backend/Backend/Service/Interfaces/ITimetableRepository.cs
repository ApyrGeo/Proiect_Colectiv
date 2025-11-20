using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Utils;

namespace TrackForUBB.Service.Interfaces;

public interface ITimetableRepository
{
    Task<SubjectResponseDTO> AddSubjectAsync(SubjectPostDTO subject);
    Task<SubjectResponseDTO?> GetSubjectByIdAsync(int id);
    Task<SubjectResponseDTO?> GetSubjectByNameAsync(string name);
    Task<LocationResponseDTO?> GetLocationByIdAsync(int id);
    Task<LocationResponseDTO> AddLocationAsync(LocationPostDTO location);
    Task<ClassroomResponseDTO?> GetClassroomByIdAsync(int id);
    Task<ClassroomResponseDTO> AddClassroomAsync(ClassroomPostDTO classroom);
    Task<List<HourResponseDTO>> GetHoursAsync(HourFilter filter);
    Task<HourResponseDTO?> GetHourByIdAsync(int id);
    Task<HourResponseDTO> AddHourAsync(HourPostDTO hour);
    Task SaveChangesAsync();
}