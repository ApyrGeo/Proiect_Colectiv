using TrackForUBB.Domain.DTOs;

namespace TrackForUBB.Service.Interfaces;
public interface IUserRepository
{
    Task<UserResponseDTO?> GetByIdAsync(int id);
    Task<UserResponseDTO?> GetByEmailAsync(string email);
    Task<UserResponseDTO> AddAsync(UserPostDTO user);
    Task SaveChangesAsync();
    Task<List<UserResponseDTO>> GetAll();
    Task<List<SpecialisationResponseDTO>> GetUserEnrolledSpecialisations(int userId);
    Task<UserProfileResponseDTO?> GetProfileByIdAsync(int id);
    Task<UserResponseDTO> UpdateAsync(int id, UserPutDTO user);
    Task<TeacherResponseDTO> GetTeacherByIdAsync(int teacherId);
    Task<UserResponseDTO?> GetByOwnerIdAsync(Guid ownerId);
    Task<bool> IsTenantEmailDuplicate(string email);
}
