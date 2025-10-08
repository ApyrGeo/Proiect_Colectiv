using Backend.Domain.DTOs;
using Backend.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Backend.Controllers;

[ApiController]
[Route("api/users")]
public class UserController(IUserService service, ILogger<UserController> logger) : ControllerBase
{
    private readonly IUserService _service = service;
    private readonly ILogger<UserController> _logger = logger;

    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<ActionResult<List<UserResponseDTO>>> GetAllUsers()
    {
        _logger.LogInformation("Received request for all users");
        List<UserResponseDTO> users = await _service.GetAllUser();

        return Ok(users);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<UserResponseDTO>> GetUserById(int id)
    {
        _logger.LogInformation("Received request for user with ID: {Id}", id);
        UserResponseDTO response = await _service.GetUserById(id);

        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(422)]
    public async Task<ActionResult> Createuser([FromBody] UserPostDTO user)
    {
        _logger.LogInformation("Received request to create user: {@User}", user);
        UserResponseDTO createdUser = await _service.CreateUser(user);
        return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
    }
}
