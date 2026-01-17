using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Utils;

namespace TrackForUBB.Controller.Interfaces;

public interface ITimetableService
{
    Task<SubjectResponseDTO> CreateSubject(SubjectPostDTO facultyPostDto);
    Task<LocationResponseDTO> CreateLocation(LocationPostDTO locationPostDTO);
    Task<ClassroomResponseDTO> CreateClassroom(ClassroomPostDTO classroomPostDTO);
    Task<HourResponseDTO> CreateHour(HourPostDTO hourPostDTO);
    Task<SubjectResponseDTO> GetSubjectById(int subjectId);
    Task<LocationResponseDTO> GetLocationById(int locationId);
    Task<ClassroomResponseDTO> GetClassroomById(int classroomId);
    Task<HourResponseDTO> GetHourById(int hourId);
    Task<TimetableResponseDTO> GetHourByFilter(HourFilter filter);
    Task<byte[]> GenerateIcs(HourFilter filter);
    Task<List<LocationWithClassroomsResponseDTO>> GetAllLocations();
    Task<List<SubjectResponseDTO>> GetSubjectsByHolderTeacherId(int teacherId);
    Task<List<StudentGroupResponseDTO>> GetGroupsBySubjectId(int subjectId);
    Task<List<HourResponseDTO>> GenerateTimetable(TimetableGenerationDTO dto);
    Task DeleteHoursBySemesterId(int semseterId);
    Task<HourResponseDTO> UpdateHour(int hourId, HourPutDTO dto);
    Task<List<OptionalPackageResponseDTO>>GetOptionalSubjectsByPromotionId(int promotionId, int year);
}
