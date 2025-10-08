using Backend.Domain;
using Backend.Domain.DTOs;

namespace Backend.Interfaces;

public interface ISpecialisationRepository
{
    Task<Specialisation> AddAsync(Specialisation specialisation);
    Task<Specialisation?> GetByIdAsync(int id);
    Task<Specialisation?> GetByNameAsync(string name);
    Task SaveChangesAsync();
}
