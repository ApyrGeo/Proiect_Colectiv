using Backend.Domain.DTOs;
using Backend.Interfaces;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService service) : ControllerBase
{
    private readonly IUserService _service = service;
    private readonly ILog _logger = LogManager.GetLogger(typeof(UserController));

    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<ActionResult<List<UserResponseDTO>>> GetAllUsers()
    {
        _logger.InfoFormat("Received request for all users");
        List<UserResponseDTO> users = await _service.GetAllUser();

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
    public async Task<ActionResult> CreateUser([FromBody] UserPostDTO user)
    {
        _logger.InfoFormat("Received request to create user: {0}", user);
        UserResponseDTO createdUser = await _service.CreateUser(user);
        return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
    }
}
