using AutoMapper;
using Backend.Domain;
using Backend.Domain.DTOs;
using Backend.Exceptions.Custom;
using Backend.Interfaces;
using EmailService.Models;
using EmailService.Providers;
using FluentValidation;
using FluentValidation.Results;
using log4net;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using IValidatorFactory = Backend.Interfaces.IValidatorFactory;

namespace Backend.Service;

public class UserService(IUserRepository userRepository, IMapper mapper, IValidatorFactory validator, IPasswordHasher<User> passwordHasher, EmailProvider emailProvider) : IUserService
{
    private readonly ILog _logger = LogManager.GetLogger(typeof(UserService));
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IValidatorFactory _validator = validator;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
    private readonly EmailProvider _emailProvider = emailProvider;

    public async Task<UserResponseDTO> CreateUser(UserPostDTO userDTO)
    {
        _logger.InfoFormat("Validating request data");
        var validator = _validator.Get<UserPostDTO>();
        var result = await validator.ValidateAsync(userDTO);
        if (!result.IsValid)
        {
            throw new EntityValidationException(result.Errors);
        }

        _logger.InfoFormat("Attempting to create user: {0}", JsonSerializer.Serialize(userDTO));
        var user = _mapper.Map<User>(userDTO);

        var tmp = user.Password;
        user.Password = _passwordHasher.HashPassword(user, user.Password);
        _logger.InfoFormat("Hash test result: " + _passwordHasher.VerifyHashedPassword(user, user.Password, tmp).ToString());

        _logger.InfoFormat("Saving user to repository: {0}", JsonSerializer.Serialize(user));
        var addedUser = await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        await SendWelcomeEmail(userDTO);

        var addedUserDTO = _mapper.Map<UserResponseDTO>(addedUser);
        return addedUserDTO;
    }

    private async Task SendWelcomeEmail(UserPostDTO user)
    {
        var userEmailModel = new CreatedUserModel { FirstName = user.FirstName, LastName = user.LastName, Password = user.Password};
        await _emailProvider.SendAsync(EmailService.Enums.EmailType.CreatedAccount, user.Email, userEmailModel);
    }

    public async Task<List<UserResponseDTO>> GetAllUser()
    {
        _logger.InfoFormat("Getting all users");
        var users = await _userRepository.GetAll();

        _logger.InfoFormat("Mapping Users to Response DTOs");
        var userDTOs = _mapper.Map<List<UserResponseDTO>>(users);

        return userDTOs;
    }

    public async Task<UserResponseDTO> GetUserById(int id)
    {
        _logger.InfoFormat("Getting user by ID: {0}", id);
        var user = await _userRepository.GetByIdAsync(id) ?? throw new NotFoundException($"User with ID {id} not found.");

        _logger.InfoFormat("Mapping User to Response DTO");
        var userDTO = _mapper.Map<UserResponseDTO>(user);
        return userDTO;
    }
}
