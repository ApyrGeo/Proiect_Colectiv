using Backend.Domain.DTOs;
using Backend.Interfaces;
using Backend.Utils;
using log4net;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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

    [HttpGet("locations/{locationId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<LocationResponseDTO>> GetLocationById([FromRoute] int locationId)
    {
        _logger.InfoFormat("Fetching location with id {0}", locationId);

        var location = await _service.GetLocationById(locationId);

        return Ok(location);
    }

    [HttpPost("locations")]
    [ProducesResponseType(201)]
    [ProducesResponseType(422)]
    public async Task<ActionResult<LocationResponseDTO>> CreateLocation([FromBody] LocationPostDTO dto)
    {
        _logger.InfoFormat("Creating new location with name {0}", dto.Name);

        var createdLocation = await _service.CreateLocation(dto);

        return CreatedAtAction(nameof(GetLocationById), new { locationId = createdLocation.Id }, createdLocation);
    }

    [HttpGet("classrooms/{classroomId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ClassroomResponseDTO>> GetClassroomById([FromRoute] int classroomId)
    {
        _logger.InfoFormat("Fetching classroom with id {0}", classroomId);

        var classroom = await _service.GetClassroomById(classroomId);

        return Ok(classroom);
    }

    [HttpPost("classrooms")]
    [ProducesResponseType(201)]
    [ProducesResponseType(422)]
    public async Task<ActionResult<ClassroomResponseDTO>> CreateClassroom([FromBody] ClassroomPostDTO dto)
    {
        _logger.InfoFormat("Creating new classroom with name {0}", dto.Name);

        var createdClassroom = await _service.CreateClassroom(dto);

        return CreatedAtAction(nameof(GetClassroomById), new { classroomId = createdClassroom.Id }, createdClassroom);
    }

    [HttpGet("hours")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<List<HourResponseDTO>>> GetHours([FromQuery] int? userId, [FromQuery] int? classroomId, [FromQuery] int? teacherId, [FromQuery] int? subjectId, [FromQuery] int? facultyId, [FromQuery] int? specialisationId, [FromQuery] int? groupYearId, [FromQuery] bool? currentWeekTimetable)
    {
        var filter = new HourFilter
        {
            UserId = userId,
            ClassroomId = classroomId,
            TeacherId = teacherId,
            SubjectId = subjectId,
            FacultyId = facultyId,
            SpecialisationId = specialisationId,
            GroupYearId = groupYearId,
            CurrentWeekTimetable = currentWeekTimetable
        };

        _logger.InfoFormat("Fetching hours with filter {0}", JsonSerializer.Serialize(filter));

        var hours = await _service.GetHourByFilter(filter);

        return Ok(hours);
    }

    [HttpGet("hours/{hourId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<HourResponseDTO>> GetHourById([FromRoute] int hourId)
    {
        _logger.InfoFormat("Fetching hour with id {0}", hourId);

        var hour = await _service.GetHourById(hourId);

        return Ok(hour);
    }

    [HttpPost("hours")]
    [ProducesResponseType(201)]
    [ProducesResponseType(422)]
    public async Task<ActionResult<HourResponseDTO>> CreateHour([FromBody] HourPostDTO dto)
    {
        _logger.InfoFormat("Creating new hour {0}", JsonSerializer.Serialize(dto));

        var createdHour = await _service.CreateHour(dto);

        return CreatedAtAction(nameof(GetHourById), new { hourId = createdHour.Id }, createdHour);
    }
}