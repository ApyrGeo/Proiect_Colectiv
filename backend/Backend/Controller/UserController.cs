using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using System.Text.Json;
using TrackForUBB.Controller.Interfaces;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Utils;

namespace TrackForUBB.Controller;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UserController(IUserService service) : ControllerBase
{
    private readonly IUserService _service = service;
    private readonly ILog _logger = LogManager.GetLogger(typeof(UserController));

    private Guid GetLoggedUserId()
    {
        if (!Guid.TryParse(HttpContext.User.GetObjectId(), out var userId))
        {
            throw new Exception("User ID is not valid.");
        }

        return userId;
    }

    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<ActionResult<List<UserResponseDTO>>> GetAllUsers([FromQuery] string? email)
    {
        _logger.InfoFormat("Received request for all users");
        List<UserResponseDTO> users = await _service.GetAllUser(email);

        return Ok(users);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<UserResponseDTO>> GetUserById(int id)
    {
        _logger.InfoFormat("Received request for user with ID: {0}", id);
        UserResponseDTO response = await _service.GetUserById(id);

        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(422)]
    [Authorize(Roles = UserRolePermission.Admin)]
    public async Task<ActionResult> CreateUser([FromBody] UserPostDTO user)
    {
        _logger.InfoFormat("Received request to create user: {0}", user);
        UserResponseDTO createdUser = await _service.CreateUser(user);
        return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
    }

    [HttpPost("bulk")]
    [ProducesResponseType(201)]
    [ProducesResponseType(422)]
    [Authorize(Roles = UserRolePermission.Admin)]
    public async Task<ActionResult<BulkUserResultDTO>> CreateUsersFromFile(IFormFile file)
    {
        _logger.InfoFormat("Received request to create users from file: {0}", file.FileName);
        var result = await _service.CreateUsersFromFile(file);
        if (!result.IsValid)
        {
            return UnprocessableEntity(result);
        }
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpGet("{userId}/enrolled-specialisations")]
    [ProducesResponseType(200)]
    public async Task<ActionResult<List<SpecialisationResponseDTO>>> GetUserEnrolledSpecialisations([FromRoute] int userId)
    {
        _logger.InfoFormat("Received request for enrolled specialisations for user ID: {0}", userId);
        List<SpecialisationResponseDTO> specialisations = await _service.GetUserEnrolledSpecialisations(userId);
        return Ok(specialisations);
    }
    
    [HttpGet("{userId}/profile")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<UserProfileResponseDTO>> GetProfile(int userId)
    {
        _logger.InfoFormat("Received request for user with ID: {0}", userId);
        var profile = await _service.GetUserProfileAsync(userId);
        return Ok(profile);
    }

    [HttpPut("{userId}/profile")]
    [ProducesResponseType(200)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> UpdateProfile(int userId,[FromBody] UserPutDTO dto)
    {
        _logger.InfoFormat("Received request for user with ID: {0}, and body: {1}", userId, JsonSerializer.Serialize(dto));
        var result = await _service.UpdateUserProfileAsync(userId, dto);

        return Ok(result);
    }

    [HttpGet("logged-user")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<LoggedUserResponseDTO>> GetLoggedUser()
    {
        var ownerId = GetLoggedUserId();
        _logger.InfoFormat("Received request for user with jwt ID: {0}", ownerId);

        var result = await _service.GetLoggedUserAsync(ownerId);

        return Ok(result);
    }
}
