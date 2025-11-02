using Backend.Domain.DTOs;
using Backend.Interfaces;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimetableController(ITimetableService service) : ControllerBase
{
    private readonly ILog _logger = LogManager.GetLogger(typeof(AcademicsController));
    private readonly ITimetableService _service = service;
    
    [HttpGet("subjects/{subjectId}")]
    [ProducesResponseType(200)]
    public async Task<ActionResult<SubjectResponseDTO>> GetSubjectById([FromRoute] int subjectId)
    {
        _logger.InfoFormat("Fetching subject with id {0}", subjectId);

        SubjectResponseDTO subjects = await _service.GetSubjectById(subjectId);

        return Ok(subjects);
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