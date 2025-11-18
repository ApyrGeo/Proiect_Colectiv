using AutoMapper;
using Domain;
using Domain.DTOs;
using Domain.Exceptions.Custom;
using EmailService.Interfaces;
using EmailService.Models;
using log4net;
using Repository.Interfaces;
using Service.Interfaces;
using Service.Utils;
using System.Text.Json;
using Utils.Security;
using IValidatorFactory = Service.Interfaces.IValidatorFactory;

namespace Service;

public class UserService(IUserRepository userRepository, IMapper mapper, IValidatorFactory validator, IAdapterPasswordHasher<User> passwordHasher, IEmailProvider emailProvider) : IUserService
{
    private readonly ILog _logger = LogManager.GetLogger(typeof(UserService));
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IValidatorFactory _validator = validator;
    private readonly IAdapterPasswordHasher<User> _passwordHasher = passwordHasher;
    private readonly IEmailProvider _emailProvider = emailProvider;

    public async Task<UserResponseDTO> CreateUser(UserPostDTO userDTO)
    {
        _logger.InfoFormat("Validating request data");
        var validator = _validator.Get<UserPostDTO>();
        var result = await validator.ValidateAsync(userDTO);
        if (!result.IsValid)
        {
            throw new EntityValidationException(ConvertValidationErrorToString.Convert(result.Errors));
        }

        _logger.InfoFormat("Attempting to create user: {0}", JsonSerializer.Serialize(userDTO));
        var user = _mapper.Map<User>(userDTO);

        user.Password = _passwordHasher.HashPassword(user, user.Password);

        _logger.InfoFormat("Saving user to repository: {0}", JsonSerializer.Serialize(user));
        var addedUser = await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        _logger.InfoFormat($"Sending email to user: {user.Email}");
        await SendWelcomeEmail(userDTO);

        var addedUserDTO = _mapper.Map<UserResponseDTO>(addedUser);
        return addedUserDTO;
    }

    private async Task SendWelcomeEmail(UserPostDTO user)
    {
        var userEmailModel = new CreatedUserModel { FirstName = user.FirstName!, LastName = user.LastName!, Password = user.Password!};
        await _emailProvider.SendCreateAccountEmailAsync(user.Email!, userEmailModel);
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
