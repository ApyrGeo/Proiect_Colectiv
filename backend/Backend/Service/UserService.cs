using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.Text.Json;
using TrackForUBB.Controller.Interfaces;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Enums;
using TrackForUBB.Domain.Exceptions.Custom;
using TrackForUBB.Domain.Security;
using TrackForUBB.Domain.Utils;
using TrackForUBB.Service.EmailService.Interfaces;
using TrackForUBB.Service.EmailService.Models;
using TrackForUBB.Service.Interfaces;
using TrackForUBB.Service.Utils;
using EntraUser = Microsoft.Graph.Models.User;
using IValidatorFactory = TrackForUBB.Service.Interfaces.IValidatorFactory;

namespace TrackForUBB.Service;

public class UserService(IUserRepository userRepository, IAcademicRepository academicRepository, IValidatorFactory validator, IAdapterPasswordHasher<UserPostDTO> passwordHasher, IEmailProvider emailProvider, IConfiguration config, GraphServiceClient graph) : IUserService
{
    private readonly ILog _logger = LogManager.GetLogger(typeof(UserService));
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IAcademicRepository _academicRepository = academicRepository;
    private readonly IValidatorFactory _validator = validator;
    private readonly IAdapterPasswordHasher<UserPostDTO> _passwordHasher = passwordHasher;
    private readonly IEmailProvider _emailProvider = emailProvider;
    private readonly IConfiguration _config = config;
    private readonly GraphServiceClient _graph = graph;

    private async Task<(Guid ownerId, string tenantEmail)> CreateEntraUser(UserPostDTO userDTO)
    {
        var resourceId = _config["AzureAd:ResourceId"];
        var studentRoleId = _config["AzureAd:AppRoles:Student"];
        var teacherRoleId = _config["AzureAd:AppRoles:Teacher"];
        var defaultPassword = _config["EntraUserDefaultPassword"];

        if (resourceId == null || studentRoleId == null || teacherRoleId == null || defaultPassword == null)
        {
            _logger.Error("Missing configuration for creating Entra user.");
            throw new NotFoundException("Missing configuration for creating Entra user.");
        }

        var mailNick = HelperFunctions.ReplaceRomanianDiacritics($"{userDTO.FirstName}.{userDTO.LastName}".ToLowerInvariant());
        var userPrincipal = $"{mailNick}@trackforubb.onmicrosoft.com";

        var entraUserRequestBody = new EntraUser
        {
            AccountEnabled = true,
            DisplayName = $"{userDTO.FirstName} {userDTO.LastName}",
            MailNickname = mailNick,
            UserPrincipalName = userPrincipal,
            PasswordProfile = new PasswordProfile
            {
                ForceChangePasswordNextSignIn = true,
                Password = defaultPassword,
            },
        };

        var result = await _graph.Users.PostAsync(entraUserRequestBody);

        if (result == null || result.Id == null)
        {
            throw new NotFoundException("Failed to create Entra user.");
        }

        var ownerId = Guid.Parse(result.Id);
        var userRole = Enum.Parse<UserRole>(userDTO.Role!);

        var appRoleAssigmentRequestBody = new AppRoleAssignment
        {
            PrincipalId = ownerId,
            ResourceId = Guid.Parse(resourceId),
            AppRoleId = userRole == UserRole.Student ? Guid.Parse(studentRoleId) : Guid.Parse(teacherRoleId),
        };

        await _graph.Users[$"{ownerId}"].AppRoleAssignments.PostAsync(appRoleAssigmentRequestBody);

        return (ownerId, tenantEmail: userPrincipal);
    }

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

        (var ownerId, var tenantEmail) = await CreateEntraUser(userDTO);
        userDTO.Owner = ownerId.ToString();

        _logger.InfoFormat("Saving user to repository: {0}", JsonSerializer.Serialize(userDTO));
        var addedUserDTO = await _userRepository.AddAsync(userDTO);
        await _userRepository.SaveChangesAsync();

        _logger.InfoFormat($"Sending email to user: {addedUserDTO.Email}");
        await SendWelcomeEmail(addedUserDTO, tenantEmail);

        return addedUserDTO;
    }

    private async Task SendWelcomeEmail(UserResponseDTO user, string tenantEmail)
    {
        var userEmailModel = new CreatedUserModel { FirstName = user.FirstName, LastName = user.LastName, Email = tenantEmail, Password = _config["EntraUserDefaultPassword"]! };
        await _emailProvider.SendCreateAccountEmailAsync(user.Email, userEmailModel);
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

    public async Task<LoggedUserResponseDTO> GetLoggedUserAsync(Guid ownerId)
    {
        _logger.InfoFormat("Getting user by owner ID: {0}", ownerId);

        var userDTO = await _userRepository.GetByOwnerIdAsync(ownerId) ?? throw new NotFoundException($"User with owner ID {ownerId} not found.");

        var response = new LoggedUserResponseDTO() { User = userDTO, Enrollments = [] };
        var enrollments = await _academicRepository.GetEnrollmentsByUserId(userDTO.Id);

        foreach (var enrollment in enrollments)
        {
            var loggedUserEnrollment = await _academicRepository.GetFacultyByEnrollment(enrollment.Id) ?? throw new NotFoundException($"Enrollment with id {enrollment.Id} doesn't have consistent data");

            response.Enrollments.Add(loggedUserEnrollment);
        }

        return response;
    }
}
