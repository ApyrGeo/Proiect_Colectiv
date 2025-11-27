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
    private readonly ILog _logger = LogManager.GetLogger(typeof(AcademicsController));
    private readonly IGradeService _service = service;
    
    [HttpGet("grades")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<List<GradeResponseDTO>>> GetUserGrades([FromQuery] int userId, [FromQuery] int? yearOfStudy, [FromQuery] int? semester)
    {
        _logger.InfoFormat("Fetching grades for user {0}, year {1}, semester {2}", userId, yearOfStudy, semester);

        List<GradeResponseDTO> grades = await _service.GetGradesFiteredAsync(userId, yearOfStudy, semester);

        return Ok(grades);
    }
    
    [HttpPost("grades")]
    [ProducesResponseType(201)]
    [ProducesResponseType(422)]
    public async Task<ActionResult<GradeResponseDTO>> CreateUserGrade([FromQuery] int teacherId,[FromBody] GradePostDTO gradePostDto)
    {
        _logger.InfoFormat("Teacher {0} creating grade: {1}", teacherId, gradePostDto);

        GradeResponseDTO createdGrade = await _service.CreateGrade(teacherId,gradePostDto);

        return CreatedAtAction(nameof(GetUserGrades), new { userId = createdGrade.Enrollment.UserId }, createdGrade);
    }
}