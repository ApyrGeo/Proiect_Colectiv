using log4net;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using TrackForUBB.Controller.Interfaces;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Exceptions.Custom;
using TrackForUBB.Domain.Security;
using TrackForUBB.Service.EmailService.Interfaces;
using TrackForUBB.Service.EmailService.Models;
using TrackForUBB.Service.Interfaces;
using TrackForUBB.Service.Utils;
using IValidatorFactory = TrackForUBB.Service.Interfaces.IValidatorFactory;

namespace TrackForUBB.Service;

public class UserService(IUserRepository userRepository, IValidatorFactory validator, IAdapterPasswordHasher<UserPostDTO> passwordHasher, IEmailProvider emailProvider, IConfiguration config) : IUserService
{
    private readonly ILog _logger = LogManager.GetLogger(typeof(UserService));
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IValidatorFactory _validator = validator;
    private readonly IAdapterPasswordHasher<UserPostDTO> _passwordHasher = passwordHasher;
    private readonly IEmailProvider _emailProvider = emailProvider;
    private readonly IConfiguration _config = config;

    public async Task<UserResponseDTO> CreateUser(UserPostDTO userDTO)
    {
        _logger.InfoFormat("Validating request data");
        var validator = _validator.Get<UserPostDTO>();
        var result = await validator.ValidateAsync(userDTO);
        if (!result.IsValid)
        {
            throw new EntityValidationException(ValidationHelper.ConvertErrorsToListOfStrings(result.Errors));
        }

        _logger.InfoFormat("Attempting to create user: {0}", JsonSerializer.Serialize(userDTO));

        userDTO.Password = _passwordHasher.HashPassword(userDTO, userDTO.Password!);

        _logger.InfoFormat("Saving user to repository: {0}", JsonSerializer.Serialize(userDTO));
        var addedUserDTO = await _userRepository.AddAsync(userDTO);
        await _userRepository.SaveChangesAsync();

        _logger.InfoFormat($"Sending email to user: {addedUserDTO.Email}");
        await SendWelcomeEmail(addedUserDTO);

        return addedUserDTO;
    }

    private async Task SendWelcomeEmail(UserResponseDTO user)
    {
        var userEmailModel = new CreatedUserModel { FirstName = user.FirstName!, LastName = user.LastName!, Password = _config["EntraUserDefaultPassword"]! };
        await _emailProvider.SendCreateAccountEmailAsync(user.Email!, userEmailModel);
    }

    public async Task<List<UserResponseDTO>> GetAllUser()
    {
        _logger.InfoFormat("Getting all users");
        var userDTOs = await _userRepository.GetAll();

        _logger.InfoFormat("Mapping Users to Response DTOs");

        return userDTOs;
    }

    public async Task<UserResponseDTO> GetUserById(int id)
    {
        _logger.InfoFormat("Getting user by ID: {0}", id);
        var userDTO = await _userRepository.GetByIdAsync(id) ?? throw new NotFoundException($"User with ID {id} not found.");

        _logger.InfoFormat("Mapping User to Response DTO");

        return userDTO;
    }

    public async Task<List<SpecialisationResponseDTO>> GetUserEnrolledSpecialisations(int userId)
    {
        _logger.InfoFormat("Getting enrolled specialisations for user ID: {0}", userId);
        return await _userRepository.GetUserEnrolledSpecialisations(userId);
    }

    public async Task<UserProfileResponseDTO> GetUserProfileAsync(int userId)
    {
        _logger.InfoFormat("Getting user by ID: {0}", userId);
        var userDTO = await _userRepository.GetProfileByIdAsync(userId) ?? throw new NotFoundException($"User with ID {userId} not found.");
        return userDTO;
    }

    public async Task<UserResponseDTO> UpdateUserProfileAsync(int userId, UserPutDTO dto)
    { 
        _logger.InfoFormat("Validating request data");

        var validator = _validator.Get<UserPutDTO>();
        var result = await validator.ValidateAsync(dto);

        if (!result.IsValid)
        {
            throw new EntityValidationException(ValidationHelper.ConvertErrorsToListOfStrings(result.Errors));
        }

        if (userId != dto.Id)
        {
            throw new EntityValidationException(["User ID mismatch between route and payload."]);
        }

        var updatedUserDTO = await _userRepository.UpdateAsync(userId, dto);
        await _userRepository.SaveChangesAsync();

        return updatedUserDTO;
    }
}
