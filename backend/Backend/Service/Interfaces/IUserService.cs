using Domain.DTOs;

namespace Service.Interfaces;

public interface IUserService
{
    Task<UserResponseDTO> CreateUser(UserPostDTO user);
    Task<List<UserResponseDTO>> GetAllUser();
    Task<UserResponseDTO> GetUserById(int id);
}
