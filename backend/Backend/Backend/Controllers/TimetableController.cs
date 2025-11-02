using Backend.Domain.DTOs;
using Backend.Interfaces;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimetableController(ITimetableService service) : ControllerBase
{
    private readonly ILog _logger = LogManager.GetLogger(typeof(TimetableController));
    private readonly ITimetableService _service = service;
    
    [HttpGet("subjects/{subjectId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<SubjectResponseDTO>> GetSubjectById([FromRoute] int subjectId)
    {
        _logger.InfoFormat("Fetching subject with id {0}", subjectId);

        var subject = await _service.GetSubjectById(subjectId);

        return Ok(subject);
    }

    [HttpPost("subjects")]
    [ProducesResponseType(201)]
    [ProducesResponseType(422)]
    public async Task<ActionResult<SubjectResponseDTO>> CreateSubject([FromBody] SubjectPostDTO subjectPostDto)
    {
        _logger.InfoFormat("Creating new subject with name {0}", subjectPostDto.Name);

        SubjectResponseDTO createdSubject = await _service.CreateSubject(subjectPostDto);

        return CreatedAtAction(nameof(GetSubjectById), new { subjectId = createdSubject.Id }, createdSubject);
    }
}