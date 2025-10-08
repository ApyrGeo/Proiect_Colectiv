using Backend.Domain.DTOs;
using Backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/academics")]
public class AcademicsController(ILogger<AcademicsController> logger, IAcademicsService service) : ControllerBase
{
    private readonly ILogger<AcademicsController> _logger = logger;
    private readonly IAcademicsService _service = service;

    [HttpGet("enrollments")]
    [ProducesResponseType(200)]
    public async Task<ActionResult<EnrollmentResponseDTO>> GetUserEnrollment([FromQuery] int userId)
    {
        _logger.LogInformation("Fetching enrollment for user with ID {UserId}", userId);

        EnrollmentResponseDTO enrollment = await _service.GetUserEnrollment(userId);

        return enrollment;
    }

    [HttpPost("enrollments")]
    [ProducesResponseType(201)]
    [ProducesResponseType(422)]
    public async Task<ActionResult<EnrollmentResponseDTO>> CreateUserEnrollment([FromBody] EnrollmentPostDTO enrollmentPostDto)
    {
        _logger.LogInformation("Creating enrollment for user with ID {UserId}", enrollmentPostDto.UserId);

        EnrollmentResponseDTO createdEnrollment = await _service.CreateUserEnrollment(enrollmentPostDto);

        return CreatedAtAction(nameof(GetUserEnrollment), createdEnrollment);
    }

    [HttpGet("faculties/{facultyId}")]
    [ProducesResponseType(200)]
    public async Task<ActionResult<FacultyResponseDTO>> GetFacultyById([FromRoute] int facultyId)
    {
        _logger.LogInformation("Fetching faculty with id {FacultyId}", facultyId);

        FacultyResponseDTO faculties = await _service.GetFacultyById(facultyId);

        return Ok(faculties);
    }

    [HttpPost("faculties")]
    [ProducesResponseType(201)]
    [ProducesResponseType(422)]
    public async Task<ActionResult<FacultyResponseDTO>> CreateFaculty([FromBody] FacultyPostDTO facultyPostDto)
    {
        _logger.LogInformation("Creating new faculty with name {FacultyName}", facultyPostDto.Name);

        FacultyResponseDTO createdFaculty = await _service.CreateFaculty(facultyPostDto);

        return CreatedAtAction(nameof(GetFacultyById), new { facultyId = createdFaculty.Id }, createdFaculty);
    }

    [HttpGet("specialisations/{specialisationId}")]
    [ProducesResponseType(200)]
    public async Task<ActionResult<SpecialisationResponseDTO>> GetSpecialisationById([FromRoute] int specialisationId)
    {
        _logger.LogInformation("Fetching specialisation with id {SpecialisationId}", specialisationId);

        SpecialisationResponseDTO specialisations = await _service.GetSpecialisationById(specialisationId);

        return Ok(specialisations);
    }

    [HttpPost("specialisations")]
    [ProducesResponseType(201)]
    [ProducesResponseType(422)]
    public async Task<ActionResult<SpecialisationResponseDTO>> CreateSpecialisation([FromBody] SpecialisationPostDTO specialisationPostDto)
    {
        _logger.LogInformation("Creating new specialisation with name {SpecialisationName}", specialisationPostDto.Name);

        SpecialisationResponseDTO createdSpecialisation = await _service.CreateSpecialisation(specialisationPostDto);

        return CreatedAtAction(nameof(GetSpecialisationById), new { specialisationId = createdSpecialisation.Id }, createdSpecialisation);
    }

    [HttpGet("group-years/{groupYearId}")]
    [ProducesResponseType(200)]
    public async Task<ActionResult<GroupYearResponseDTO>> GetGroupYearById([FromRoute] int groupYearId)
    {
        _logger.LogInformation("Fetching group year with id {GroupYearId}", groupYearId);

        GroupYearResponseDTO groupYear = await _service.GetGroupYearById(groupYearId);

        return Ok(groupYear);
    }

    [HttpPost("group-years")]
    [ProducesResponseType(201)]
    [ProducesResponseType(422)]
    public async Task<ActionResult<GroupYearResponseDTO>> CreateGroupYear([FromBody] GroupYearPostDTO groupYearPostDto)
    {
        _logger.LogInformation("Creating new group year for specialisation ID {SpecialisationId} and year {Year}", groupYearPostDto.SpecialisationId, groupYearPostDto.Year);
        
        GroupYearResponseDTO createdGroupYear = await _service.CreateGroupYear(groupYearPostDto);
        
        return CreatedAtAction(nameof(GetGroupYearById), new { groupYearId = createdGroupYear.Id }, createdGroupYear);
    }

    [HttpGet("student-groups/{studentGroupId}")]
    [ProducesResponseType(200)]
    public async Task<ActionResult<StudentGroupResponseDTO>> GetStudentGroupById([FromRoute] int studentGroupId)
    {
        _logger.LogInformation("Fetching student group with id {StudentGroupId}", studentGroupId);

        StudentGroupResponseDTO studentGroup = await _service.GetStudentGroupById(studentGroupId);

        return Ok(studentGroup);
    }

    [HttpPost("student-groups")]
    [ProducesResponseType(201)]
    [ProducesResponseType(422)]
    public async Task<ActionResult<StudentGroupResponseDTO>> CreateStudentGroup([FromBody] StudentGroupPostDTO studentGroupPostDto)
    {
        _logger.LogInformation("Creating new student group with name {GroupName} for group year ID {GroupYearId}", studentGroupPostDto.Name, studentGroupPostDto.GroupYearId);
        
        StudentGroupResponseDTO createdStudentGroup = await _service.CreateStudentGroup(studentGroupPostDto);
        
        return CreatedAtAction(nameof(GetStudentGroupById), new { studentGroupId = createdStudentGroup.Id }, createdStudentGroup);
    }

    [HttpGet("student-subgroups/{studentSubGroupId}")]
    [ProducesResponseType(200)]
    public async Task<ActionResult<StudentSubGroupResponseDTO>> GetStudentSubGroupById([FromRoute] int studentSubGroupId)
    {
        _logger.LogInformation("Fetching student sub-group with id {StudentSubGroupId}", studentSubGroupId);

        StudentSubGroupResponseDTO studentSubGroup = await _service.GetStudentSubGroupById(studentSubGroupId);

        return Ok(studentSubGroup);
    }

    [HttpPost("student-subgroups")]
    [ProducesResponseType(201)]
    [ProducesResponseType(422)]
    public async Task<ActionResult<StudentSubGroupResponseDTO>> CreateStudentSubGroup([FromBody] StudentSubGroupPostDTO studentSubGroupPostDto)
    {
        _logger.LogInformation("Creating new student sub-group with name {SubGroupName} for student group ID {StudentGroupId}", studentSubGroupPostDto.Name, studentSubGroupPostDto.StudentGroupId);
        
        StudentSubGroupResponseDTO createdStudentSubGroup = await _service.CreateStudentSubGroup(studentSubGroupPostDto);
        
        return CreatedAtAction(nameof(GetStudentSubGroupById), new { studentSubGroupId = createdStudentSubGroup.Id }, createdStudentSubGroup);
    }
}