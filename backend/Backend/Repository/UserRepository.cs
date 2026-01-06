using AutoMapper;
using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Models;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Exceptions.Custom;
using TrackForUBB.Repository.Context;
using TrackForUBB.Repository.EFEntities;
using TrackForUBB.Service.Interfaces;

namespace TrackForUBB.Repository;

public class UserRepository(AcademicAppContext context, IMapper mapper) : IUserRepository
{
    private readonly AcademicAppContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly ILog _logger = LogManager.GetLogger(typeof(UserRepository));

    public async Task<UserResponseDTO?> GetByEmailAsync(string email)
    {
        _logger.InfoFormat("Fetching user by email: {0}", email);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        return _mapper.Map<UserResponseDTO>(user);
    }

    public async Task<UserResponseDTO> AddAsync(InternalUserPostDTO user)
    {
        _logger.InfoFormat("Adding new user with email: {0}", user.Email);

        var entity = _mapper.Map<User>(user);
        await _context.Users.AddAsync(entity);

        return _mapper.Map<UserResponseDTO>(entity);
    }

    public async Task<UserResponseDTO?> GetByIdAsync(int id)
    {
        _logger.InfoFormat("Fetching user by ID: {0}", id);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        return _mapper.Map<UserResponseDTO>(user);
    }
    
    public async Task<UserProfileResponseDTO?> GetProfileByIdAsync(int id)
    {
        _logger.InfoFormat("Fetching user by ID: {0}", id);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        return _mapper.Map<UserProfileResponseDTO>(user);
    }

    public async Task<UserResponseDTO> UpdateAsync(int id , UserPutDTO user)
    {
        _logger.InfoFormat("Updating user with id: {0}", user.Id);
        var entity = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (entity == null)
        {
            throw new NotFoundException($"User with ID {id} not found.");
        }

        if (!string.IsNullOrEmpty(user.SignatureBase64))
            entity.Signature = Convert.FromBase64String(user.SignatureBase64);

        return _mapper.Map<UserResponseDTO>(entity);
    }

    public async Task SaveChangesAsync()
    {
        _logger.InfoFormat("Saving changes");
        await _context.SaveChangesAsync();
    }

    public async Task<List<UserResponseDTO>> GetAll()
    {
        _logger.InfoFormat("Fetching all users");

        var users = await _context.Users.ToListAsync();

        return _mapper.Map<List<UserResponseDTO>>(users);
    }

    public async Task<List<SpecialisationResponseDTO>> GetUserEnrolledSpecialisations(int userId)
    {
        _logger.InfoFormat("Fetching enrolled specialisations for user ID: {0}", userId);

        var specialisations = await _context.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Enrollments)
            .Select(e => e.SubGroup.StudentGroup.Promotion.Specialisation)
            .Where(s => s != null)
            .Distinct()
            .ToListAsync();

        return _mapper.Map<List<SpecialisationResponseDTO>>(specialisations);
    }

    public async Task<TeacherResponseDTO> GetTeacherByIdAsync(int teacherId)
    {
        _logger.InfoFormat("Fetching teacher by ID: {0}", teacherId);
        var teacher = await _context.Teachers
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id == teacherId);

        return _mapper.Map<TeacherResponseDTO>(teacher);
    }

    public async Task<UserResponseDTO?> GetByOwnerIdAsync(Guid ownerId)
    {
        _logger.InfoFormat("Fetching user by owner ID: {0}", ownerId);
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Owner == ownerId);

        return _mapper.Map<UserResponseDTO>(user);
    }

    public async Task<UserResponseDTO> UpdateEntraDetailsAsync(int id, Guid ownerId, string tenantEmail)
    {
        _logger.InfoFormat("Updating user with id: {0}", id);
        var entity = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (entity == null)
        {
            throw new NotFoundException($"User with ID {id} not found.");
        }

        entity.Owner = ownerId;
        entity.TenantEmail = tenantEmail;

        return _mapper.Map<UserResponseDTO>(entity);
    }
}
