using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackForUBB.Controller.Interfaces;
using TrackForUBB.Domain.DTOs;

namespace TrackForUBB.Controller;


[ApiController]
[Route("api/[controller]")]
public class GradesController(IGradeService service) : ControllerBase
{
    private readonly ILog _logger = LogManager.GetLogger(typeof(GradesController));
    private readonly IGradeService _service = service;
    
    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<List<GradeResponseDTO>>> GetUserGrades([FromQuery] int userId, [FromQuery] int? yearOfStudy, [FromQuery] int? semester, [FromQuery] string specialisation)
    {
        _logger.InfoFormat("Fetching grades for user {0}, year {1}, semester {2}, specialisation {3}", userId, yearOfStudy, semester, specialisation);

        List<GradeResponseDTO> grades = await _service.GetGradesFiteredAsync(userId, yearOfStudy, semester, specialisation);

        return Ok(grades);
    }

    [HttpGet("status")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ScholarshipStatusDTO>> GetUserAverageScoreAndScholarshipStatus([FromQuery] int userId, [FromQuery] int yearOfStudy, [FromQuery] int semester, [FromQuery] string specialisation)
    {
        _logger.Info("Fetching all grades");
        var status = await _service.GetUserAverageScoreAndScholarshipStatusAsync(userId, yearOfStudy, semester, specialisation);
        return Ok(status);
    }

    [HttpGet("{gradeId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<GradeResponseDTO>> GetGradeById([FromRoute] int gradeId)
    {
        _logger.InfoFormat("Fetching grade with ID {0}", gradeId);

        var grade = await _service.GetGradeByIdAsync(gradeId);

        return Ok(grade);
    }
    
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(422)]
    public async Task<ActionResult<GradeResponseDTO>> CreateUserGrade([FromQuery] int teacherId, [FromBody] GradePostDTO gradePostDto)
    {
        _logger.InfoFormat("Teacher {0} creating grade: {1}", teacherId, gradePostDto);

        GradeResponseDTO createdGrade = await _service.CreateGrade(teacherId,gradePostDto);

        return CreatedAtAction(nameof(GetGradeById), new { gradeId = createdGrade.Id }, createdGrade);
    }
    
    [HttpPut("{gradeId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<GradeResponseDTO>> UpdateGrade(int gradeId, [FromQuery] int teacherId, [FromBody] GradePostDTO dto)
    {
        var updatedGrade = await _service.UpdateGradeAsync(teacherId, gradeId, dto);
        return Ok(updatedGrade);
    }

    [HttpPatch("{gradeId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<GradeResponseDTO>> PatchGrade(int gradeId, [FromQuery] int teacherId, [FromBody] GradePatchDTO dto)
    {
        var updatedGrade = await _service.PatchGradeAsync(teacherId, gradeId, dto.Value);
        return Ok(updatedGrade);
    }
}