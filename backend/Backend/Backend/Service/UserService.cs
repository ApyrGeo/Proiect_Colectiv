using AutoMapper;
using Backend.Domain;
using Backend.Domain.DTOs;
using Backend.Exceptions.Custom;
using Backend.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using IValidatorFactory = Backend.Interfaces.IValidatorFactory;

namespace Backend.Service;

public class UserService(ILogger<UserService> logger, IUserRepository userRepository, IMapper mapper, IValidatorFactory validator, IPasswordHasher<User> passwordHasher) 
    : IUserService
{
    private readonly ILogger<UserService> _logger = logger;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IValidatorFactory _validator = validator;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

    public async Task<UserResponseDTO> CreateUser(UserPostDTO userDTO)
    {
        _logger.LogInformation("Validating request data");
        var validator = _validator.Get<UserPostDTO>();
        var result = await validator.ValidateAsync(userDTO);
        if (!result.IsValid)
        {
            _logger.LogWarning("Validation failed for user: {@Errors}", result.Errors);
            throw new EntityValidationException(result.Errors);
        }

        _logger.LogInformation("Attempting to create user: {@User}", userDTO);
        var user = _mapper.Map<User>(userDTO);

        string tmp = user.Password;
        user.Password = _passwordHasher.HashPassword(user, user.Password);
        _logger.LogInformation("Hash test result: " + _passwordHasher.VerifyHashedPassword(user, user.Password, tmp).ToString());

        _logger.LogInformation("Saving user to repository");
        var addedUser = await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        var addedUserDTO = _mapper.Map<UserResponseDTO>(addedUser);
        return addedUserDTO;
    }

    public async Task<List<UserResponseDTO>> GetAllUser()
    {
        _logger.LogInformation("Getting all users");
        var users = await _userRepository.GetAll();

        _logger.LogInformation("Mapping Users to Response DTOs");
        var userDTOs = _mapper.Map<List<UserResponseDTO>>(users);

        return userDTOs;
    }

    public async Task<UserResponseDTO> GetUserById(int id)
    {
        _logger.LogInformation("Getting user by ID: {Id}", id);
        var user = await _userRepository.GetByIdAsync(id) ?? throw new NotFoundException($"User with ID {id} not found.");

        _logger.LogInformation("Mapping User to Response DTO");
        var userDTO = _mapper.Map<UserResponseDTO>(user);
        return userDTO;
    }
}
