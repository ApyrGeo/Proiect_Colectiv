using TrackForUBB.Domain.DTOs;

namespace TrackForUBB.Service.Interfaces;
public interface IUserRepository
{
    Task<UserResponseDTO?> GetByIdAsync(int id);
    Task<UserResponseDTO?> GetByEmailAsync(string email);
    Task<UserResponseDTO> AddAsync(UserPostDTO user);
    Task SaveChangesAsync();
    Task<List<UserResponseDTO>> GetAll();
}
