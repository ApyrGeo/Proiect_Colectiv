using Backend.Domain;
using Backend.Domain.DTOs;

namespace Backend.Interfaces;

public interface IGroupYearRepository
{
    Task<GroupYear> AddAsync(GroupYear groupYear);
    Task<GroupYear?> GetByIdAsync(int id);
    Task<GroupYear?> GetByYearAsync(string year);
    Task SaveChangesAsync();
}
