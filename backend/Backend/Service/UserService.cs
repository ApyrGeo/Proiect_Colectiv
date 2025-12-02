using AutoMapper;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Exceptions.Custom;
using log4net;
using TrackForUBB.Service.Interfaces;
using TrackForUBB.Service.Utils;
using System.Text.Json;
using FluentValidation;
using FluentValidation.Internal;
using IValidatorFactory = TrackForUBB.Service.Interfaces.IValidatorFactory;
using TrackForUBB.Service.EmailService.Interfaces;
using TrackForUBB.Service.EmailService.Models;
using TrackForUBB.Domain.Security;
using TrackForUBB.Controller.Interfaces;
using TrackForUBB.Service.Validators;

namespace TrackForUBB.Service;

public class UserService(IUserRepository userRepository, IValidatorFactory validator, IAdapterPasswordHasher<UserPostDTO> passwordHasher, IEmailProvider emailProvider) : IUserService
{
    private readonly ILog _logger = LogManager.GetLogger(typeof(UserService));
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IValidatorFactory _validator = validator;
    private readonly IAdapterPasswordHasher<UserPostDTO> _passwordHasher = passwordHasher;
    private readonly IEmailProvider _emailProvider = emailProvider;

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
        var userEmailModel = new CreatedUserModel { FirstName = user.FirstName!, LastName = user.LastName!, Password = user.Password! };
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

    public async Task<UserResponseDTO> UpdateUserProfileAsync(int userId, UserPostDTO dto)
    {

        var existingUser = await _userRepository.GetByIdAsync(userId);
        if (existingUser == null)
        {
            throw new NotFoundException($"User with ID {userId} not found.");
        }
        if (string.IsNullOrEmpty(dto.Password))
            dto.Password= existingUser.Password;

        if (string.IsNullOrEmpty(dto.PhoneNumber))
            dto.PhoneNumber = existingUser.PhoneNumber ;
        dto.Email = existingUser.Email;
        dto.FirstName = existingUser.FirstName;
        dto.LastName = existingUser.LastName;
        dto.Role = existingUser.Role;
        
        _logger.InfoFormat("Validating request data");
        var validator = _validator.Get<UserPostDTO>();
        var result = await validator.ValidateAsync(dto,(Action<ValidationStrategy<UserPostDTO>>)(opts => opts.IncludeRuleSets("Update")));
        if (!result.IsValid)
        {
            throw new EntityValidationException(ValidationHelper.ConvertErrorsToListOfStrings(result.Errors));
        }
        
        dto.Password = _passwordHasher.HashPassword(dto, dto.Password!);
        
        var updatedUserDTO = await _userRepository.UpdateAsync(userId,dto);
        await _userRepository.SaveChangesAsync();

        return updatedUserDTO;

    }
}
