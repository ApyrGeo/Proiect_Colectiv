using TrackForUBB.Domain.DTOs;

namespace TrackForUBB.Controller.Interfaces;

public interface IUserService
{
    Task<UserResponseDTO> CreateUser(UserPostDTO user);
    Task<List<UserResponseDTO>> GetAllUser();
    Task<UserResponseDTO> GetUserById(int id);
    Task<List<SpecialisationResponseDTO>> GetUserEnrolledSpecialisations(int userId);
    Task<UserProfileResponseDTO> GetUserProfileAsync(int userId);
    Task<UserResponseDTO> UpdateUserProfileAsync(int userId, UserPostDTO dto);
}
