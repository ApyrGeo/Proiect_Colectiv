using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Text.Json;
using TrackForUBB.Controller.Interfaces;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Utils;

namespace TrackForUBB.Controller;

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

    [HttpGet("promotions/{promotionId}/subjects/optional")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [Authorize(Roles = UserRolePermission.Student)]
    public async Task<ActionResult<List<OptionalPackageResponseDTO>>> GetOptionalSubjects([FromRoute] int promotionId, int year = 1)
    {
        _logger.InfoFormat("Fetching subjects with id {0}", promotionId);

        var subjects = await _service.GetOptionalSubjectsByPromotionId(promotionId, year);

        return Ok(subjects);
    }

    [HttpPost("subjects")]
    [ProducesResponseType(201)]
    [ProducesResponseType(422)]
    [Authorize(Roles = UserRolePermission.Admin)]
    public async Task<ActionResult<SubjectResponseDTO>> CreateSubject([FromBody] SubjectPostDTO subjectPostDto)
    {
        _logger.InfoFormat("Creating new subject with name {0}", subjectPostDto.Name);

        SubjectResponseDTO createdSubject = await _service.CreateSubject(subjectPostDto);

        return CreatedAtAction(nameof(GetSubjectById), new { subjectId = createdSubject.Id }, createdSubject);
    }

    [HttpGet("subjects/holder-teacher/{teacherId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<List<SubjectResponseDTO>>> GetSubjectsByHolderTeacherId([FromRoute] int teacherId)
    {
        _logger.InfoFormat("Fetching subjects held by teacher with id {0}", teacherId);
        var subject = await _service.GetSubjectsByHolderTeacherId(teacherId);
        return Ok(subject);
    }

    [HttpGet("subjects/{subjectId}/groups")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<List<StudentGroupResponseDTO>>> GetSubGroupsBySubjectId([FromRoute] int subjectId)
    {
        _logger.InfoFormat("Fetching groups for subject with id {0}", subjectId);
        var groups = await _service.GetGroupsBySubjectId(subjectId);
        return Ok(groups);
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

    [HttpGet("locations")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<List<LocationWithClassroomsResponseDTO>>> GetLocations()
    {
        _logger.Info("Fetching all locations");

        var locations = await _service.GetAllLocations();

        return Ok(locations);
    }

    [HttpPost("locations")]
    [ProducesResponseType(201)]
    [ProducesResponseType(422)]
    [Authorize(Roles = UserRolePermission.Admin)]
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
    [Authorize(Roles = UserRolePermission.Admin)]
    public async Task<ActionResult<ClassroomResponseDTO>> CreateClassroom([FromBody] ClassroomPostDTO dto)
    {
        _logger.InfoFormat("Creating new classroom with name {0}", dto.Name);

        var createdClassroom = await _service.CreateClassroom(dto);

        return CreatedAtAction(nameof(GetClassroomById), new { classroomId = createdClassroom.Id }, createdClassroom);
    }

    [HttpGet("hours")]
    [ProducesResponseType(200)]
    [ProducesResponseType(422)]
    public async Task<ActionResult<List<HourResponseDTO>>> GetHours([FromQuery] int? userId, [FromQuery] int? classroomId, [FromQuery] int? teacherId, [FromQuery] int? subjectId, [FromQuery] int? facultyId, [FromQuery] int? specialisationId, [FromQuery] int? groupYearId, [FromQuery] int? semesterNumber, [FromQuery] bool? currentWeekTimetable)
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
            SemesterNumber = semesterNumber,
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
    [Authorize(Roles = UserRolePermission.Admin)]
    public async Task<ActionResult<HourResponseDTO>> CreateHour([FromBody] HourPostDTO dto)
    {
        _logger.InfoFormat("Creating new hour {0}", JsonSerializer.Serialize(dto));

        var createdHour = await _service.CreateHour(dto);

        return CreatedAtAction(nameof(GetHourById), new { hourId = createdHour.Id }, createdHour);
    }

    [HttpGet("hours/download")]
    [ProducesResponseType(200)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> DownloadIcs([FromQuery] HourFilter filter)
    {
        _logger.InfoFormat("Downloading ICS with filter {0}", JsonSerializer.Serialize(filter));

        var icsBytes = await _service.GenerateIcs(filter);

        return File(icsBytes, "text/calendar; charset=utf-8", $"timetable_{DateTime.UtcNow:yyyyMMdd}.ics");
    }

    [HttpPost("hours/generate-timetable")]
    [ProducesResponseType(200)]
    [ProducesResponseType(422)]
    public async Task<ActionResult> GenerateTimetable([FromBody] TimetableGenerationDTO dto)
    {
        _logger.InfoFormat("Generating timetable with parameters {0}", JsonSerializer.Serialize(dto));
        List<HourResponseDTO> hours = await _service.GenerateTimetable(dto);
        return Ok(hours);
    }

    [HttpPut("hours/{hourId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<HourResponseDTO>> UpdateHour([FromRoute] int hourId, [FromBody] HourPutDTO dto)
    {
        _logger.InfoFormat("Updating hour with id {0}", hourId);
        var updatedHour = await _service.UpdateHour(hourId, dto);
        return Ok(updatedHour);
    }
}
