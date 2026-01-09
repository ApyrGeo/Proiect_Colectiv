using AutoMapper;
using ClosedXML.Excel;
using CsvHelper;
using CsvHelper.Configuration;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.Globalization;
using System.Text.Json;
using TrackForUBB.Controller.Interfaces;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Enums;
using TrackForUBB.Domain.Exceptions.Custom;
using TrackForUBB.Domain.Utils;
using TrackForUBB.Service.EmailService.Interfaces;
using TrackForUBB.Service.EmailService.Models;
using TrackForUBB.Service.FileHeaderMapper;
using TrackForUBB.Service.Interfaces;
using TrackForUBB.Service.Utils;
using EntraUser = Microsoft.Graph.Models.User;
using IValidatorFactory = TrackForUBB.Service.Interfaces.IValidatorFactory;

namespace TrackForUBB.Service;

public class UserService(IUserRepository userRepository, IAcademicRepository academicRepository, IMapper mapper, 
    IValidatorFactory validator, IEmailProvider emailProvider, IConfiguration config, GraphServiceClient? graph) : IUserService
{
    private readonly ILog _logger = LogManager.GetLogger(typeof(UserService));
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IAcademicRepository _academicRepository = academicRepository;
    private readonly IValidatorFactory _validator = validator;
    private readonly IEmailProvider _emailProvider = emailProvider;
    private readonly IConfiguration _config = config;
    private readonly IMapper _mapper = mapper;
    private readonly GraphServiceClient? _graph = graph;

    private async Task<(Guid ownerId, string tenantEmail)> CreateEntraUser(UserResponseDTO userDTO)
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

        var mailNickFirstName = userDTO.FirstName!;
        if (mailNickFirstName.Contains(' '))
        {
            mailNickFirstName = mailNickFirstName.Split(" ")[0];
        }

        var mailNickLastName = userDTO.LastName!;
        if (mailNickLastName.Contains(' '))
        {
            mailNickLastName = mailNickLastName.Split(" ")[0];
        }   

        var mailNick = HelperFunctions.ReplaceRomanianDiacritics($"{mailNickFirstName}.{mailNickLastName}{userDTO.Id}".ToLowerInvariant());
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
        var userRole = userDTO.Role;

        var appRoleAssigmentRequestBody = new AppRoleAssignment
        {
            PrincipalId = ownerId,
            ResourceId = Guid.Parse(resourceId),
            AppRoleId = userRole == UserRole.Student ? Guid.Parse(studentRoleId) : Guid.Parse(teacherRoleId),
        };

        await _graph.Users[$"{ownerId}"].AppRoleAssignments.PostAsync(appRoleAssigmentRequestBody);

        return (ownerId, tenantEmail: userPrincipal);
    }

    public async Task<UserResponseDTO> CreateUser(UserPostDTO dto)
    {
        var userDTO = _mapper.Map<InternalUserPostDTO>(dto);

        return await CreateUser(userDTO);
    }

    private async Task<UserResponseDTO> CreateUser(InternalUserPostDTO userDTO)
    {
        _logger.InfoFormat("Validating request data");
        var validator = _validator.Get<InternalUserPostDTO>();
        var result = await validator.ValidateAsync(userDTO);
        if (!result.IsValid)
        {
            throw new EntityValidationException(ValidationHelper.ConvertErrorsToListOfStrings(result.Errors));
        }

        _logger.InfoFormat("Attempting to create user: {0}", JsonSerializer.Serialize(userDTO));

        _logger.InfoFormat("Saving user to repository: {0}", JsonSerializer.Serialize(userDTO));
        var addedUserDTO = await _userRepository.AddAsync(userDTO);

        if (_graph is null)
            return addedUserDTO;

        (var ownerId, var tenantEmail) = await CreateEntraUser(addedUserDTO);
        var updatedUserDTO = await _userRepository.UpdateEntraDetailsAsync(addedUserDTO.Id, ownerId, tenantEmail);

        _logger.InfoFormat($"Sending email to user: {updatedUserDTO.Email}");
        await SendWelcomeEmail(updatedUserDTO, tenantEmail);

        return updatedUserDTO;
    }

    private async Task SendWelcomeEmail(UserResponseDTO user, string tenantEmail)
    {
        var userEmailModel = new CreatedUserModel 
        { FirstName = user.FirstName, LastName = user.LastName, Email = tenantEmail, Password = _config["EntraUserDefaultPassword"]! };
        await _emailProvider.SendCreateAccountEmailAsync(user.Email, userEmailModel);
    }

    public async Task<List<UserResponseDTO>> GetAllUser(string? email)
    {
        _logger.InfoFormat("Getting all users");
        var userDTOs = await _userRepository.GetAll(email);

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

        return updatedUserDTO;
    }

    public async Task<LoggedUserResponseDTO> GetLoggedUserAsync(Guid ownerId)
    {
        _logger.InfoFormat("Getting user by owner ID: {0}", ownerId);

        var userDTO = await _userRepository.GetByOwnerIdAsync(ownerId) 
            ?? throw new NotFoundException($"User with owner ID {ownerId} not found.");

        var response = new LoggedUserResponseDTO() { User = userDTO, Enrollments = [] };
        var enrollments = await _academicRepository.GetEnrollmentsByUserId(userDTO.Id);

        foreach (var enrollment in enrollments)
        {
            var loggedUserEnrollment = await _academicRepository.GetFacultyByEnrollment(enrollment.Id) 
                ?? throw new NotFoundException($"Enrollment with id {enrollment.Id} doesn't have consistent data");

            response.Enrollments.Add(loggedUserEnrollment);
        }

        return response;
    }

    public async Task<BulkUserResultDTO> CreateUsersFromFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new EntityValidationException(["File is empty or not provided."]);
        }

        if (file.Length > 100_000_000)
        {
            throw new EntityValidationException(["File size exceeds the maximum limit of 100 MB."]);
        }

        var allowedExtensions = new[] { ".csv", ".xlsx", ".xls" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(fileExtension))
        {
            throw new EntityValidationException(["Unsupported file type. Allowed: .csv, .xlsx"]);
        }

        var parseList = new List<(int Row, InternalUserPostDTO Dto)>();

        if (fileExtension == ".csv")
        {
            parseList = ParseUserCsvFile(file);
        }
        else
        {
            parseList = ParseUserExcelFile(file);
        }

        (var resultItems, var isValid) = await ValidateAddUserFile(parseList);

        if (!isValid)
        {
            return new BulkUserResultDTO { Users = resultItems };
        }

        var finalItems = new List<BulkUserItemResultDTO>();

        foreach (var (row, dto) in parseList)
        {
            var createdUser = await CreateUser(dto);
            finalItems.Add(new BulkUserItemResultDTO
            {
                Row = row,
                Email = createdUser.Email,
                IsValid = true,
                CreatedUserId = createdUser.Id,
            });
        }

        return new BulkUserResultDTO { Users = finalItems };
    }

    private async Task<(List<BulkUserItemResultDTO> resultItems, bool isValid)> 
        ValidateAddUserFile(List<(int Row, InternalUserPostDTO Dto)> list)
    {
        var validator = _validator.Get<InternalUserPostDTO>();
        var resultItems = new List<BulkUserItemResultDTO>();

        if (list.Count == 0)
        {
            return (resultItems, false);
        }

        foreach (var (row, dto) in list)
        {
            // check if InternalUserPostDTO is valid
            var item = new BulkUserItemResultDTO { Row = row, Email = dto.Email };
            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                item.IsValid = false;
                item.Errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
            }

            if (dto.Role != null && dto.Role != "" && Enum.Parse<UserRole>(dto.Role) == UserRole.Admin)
            {
                item.IsValid = false;
                item.Errors.Add("Cannot create Admin users via bulk upload.");
            }

            resultItems.Add(item);
        }

        // check for duplicate emails inside file
        var emailGroups = list
            .Select(p => new { p.Row, Email = p.Dto.Email?.Trim().ToLowerInvariant() })
            .Where(x => !string.IsNullOrEmpty(x.Email))
            .GroupBy(x => x.Email)
            .Where(g => g.Count() > 1);

        foreach (var group in emailGroups)
        {
            foreach (var entry in group)
            {
                var item = resultItems.First(i => i.Row == entry.Row);
                item.IsValid = false;
                item.Errors.Add("Duplicate email inside file.");
            }
        }

        // final validation result
        if (resultItems.Any(i => i.Errors.Count > 0))
        {
            foreach (var it in resultItems)
            {
                it.IsValid = it.Errors.Count == 0;
            }

            return (resultItems, false);
        }

        return (resultItems, true);
    }

    private static List<(int Row, InternalUserPostDTO Dto)> ParseUserCsvFile(IFormFile file)
    {
        var dtoList = new List<(int Row, InternalUserPostDTO Dto)>();

        using var reader = new StreamReader(file.OpenReadStream());
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            BadDataFound = null,
            HeaderValidated = null
        };

        using var csv = new CsvReader(reader, config);
        csv.Context.RegisterClassMap<InternalUserPostDTOMap>();

        try
        {
            if (!csv.Read())
            {
                throw new EntityValidationException(["CSV file is empty."]);
            }

            csv.ReadHeader();
            var headers = csv.HeaderRecord ?? [];
            var headerSet = new HashSet<string>(headers, StringComparer.InvariantCultureIgnoreCase);

            var map = new InternalUserPostDTOMap();
            var expectedSamples = map.MemberMaps
                .Select(m => m.Data.Names.FirstOrDefault())
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .ToList();

            var anyMappedHeaderPresent = map.MemberMaps
                .SelectMany(m => m.Data.Names)
                .Any(name => headerSet.Contains(name));

            if (!anyMappedHeaderPresent)
            {
                var sampleList = expectedSamples.Count > 0 ? string.Join(", ", expectedSamples) : "";
                throw new EntityValidationException([$"CSV headers are invalid or missing. Expected headers: {sampleList}."]);
            }

            var records = csv.GetRecords<InternalUserPostDTO>().ToList();

            for (int i = 0; i < records.Count; i++)
            {
                dtoList.Add((i + 2, records[i]));
            }

            return dtoList;
        }
        catch (CsvHelperException ex)
        {
            throw new EntityValidationException([$"Failed to parse CSV file: {ex.Message}"]);
        }
    }

    private static List<(int Row, InternalUserPostDTO dto)> ParseUserExcelFile(IFormFile file)
    {
        var dtoList = new List<(int Row, InternalUserPostDTO Dto)>();

        using var workbook = new XLWorkbook(file.OpenReadStream());
        var workSheets = workbook.Worksheets.First();
        var headerCells = workSheets.Row(1).CellsUsed().ToList();
        var headerMap = headerCells.Select((c, idx) => new { Name = c.GetString().Trim(), Index = idx + 1 })
                                   .ToDictionary(x => x.Name, x => x.Index, StringComparer.InvariantCultureIgnoreCase);

        var map = new InternalUserPostDTOMap();
        var propertyIndex = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);

        foreach (var memberMap in map.MemberMaps)
        {
            var propName = memberMap.Data.Member?.Name;
            if (string.IsNullOrWhiteSpace(propName))
            {
                continue;
            }

            foreach (var candidate in memberMap.Data.Names)
            {
                if (headerMap.TryGetValue(candidate, out var index))
                {
                    propertyIndex[propName] = index;
                    break;
                }
            }
        }

        int lastRow = workSheets.LastRowUsed()!.RowNumber();

        for (int r = 2; r <= lastRow; r++)
        {
            string? GetCellByProp(string prop)
            {
                if (!propertyIndex.TryGetValue(prop, out var idx))
                {
                    return null;
                }

                return workSheets.Row(r).Cell(idx).GetString()?.Trim();
            }

            var dto = new InternalUserPostDTO
            {
                FirstName = GetCellByProp(nameof(InternalUserPostDTO.FirstName)),
                LastName = GetCellByProp(nameof(InternalUserPostDTO.LastName)),
                Email = GetCellByProp(nameof(InternalUserPostDTO.Email)),
                PhoneNumber = GetCellByProp(nameof(InternalUserPostDTO.PhoneNumber)),
                Role = GetCellByProp(nameof(InternalUserPostDTO.Role)),
            };

            dtoList.Add((r, dto));
        }

        return dtoList;
    }
}
