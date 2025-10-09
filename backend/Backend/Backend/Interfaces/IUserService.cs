using Backend.Domain;
using Backend.Domain.DTOs;

namespace Backend.Interfaces;

public interface IUserService
{
    Task<UserResponseDTO> CreateUser(UserPostDTO user);
    Task<List<UserResponseDTO>> GetAllUser();
    Task<UserResponseDTO> GetUserById(int id);
}
