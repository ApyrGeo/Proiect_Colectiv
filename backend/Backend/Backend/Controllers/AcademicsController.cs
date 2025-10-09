using Backend.Domain.DTOs;
using Backend.Interfaces;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AcademicsController(IAcademicsService service) : ControllerBase
{
    private readonly ILog _logger = LogManager.GetLogger(typeof(AcademicsController));
    private readonly IAcademicsService _service = service;

    [HttpGet("enrollments")]
    [ProducesResponseType(200)]
    public async Task<ActionResult<EnrollmentResponseDTO>> GetUserEnrollment([FromQuery] int userId)
    {
        _logger.InfoFormat("Fetching enrollment for user with ID {0}", userId);

        EnrollmentResponseDTO enrollment = await _service.GetUserEnrollment(userId);

        return Ok(enrollment);
    }

    [HttpPost("enrollments")]
    [ProducesResponseType(201)]
    [ProducesResponseType(422)]
    public async Task<ActionResult<EnrollmentResponseDTO>> CreateUserEnrollment([FromBody] EnrollmentPostDTO enrollmentPostDto)
    {
        _logger.InfoFormat("Creating enrollment for user with ID {0}", enrollmentPostDto.UserId);

        EnrollmentResponseDTO createdEnrollment = await _service.CreateUserEnrollment(enrollmentPostDto);

        return CreatedAtAction(nameof(GetUserEnrollment), createdEnrollment);
    }

    [HttpGet("faculties/{facultyId}")]
    [ProducesResponseType(200)]
    public async Task<ActionResult<FacultyResponseDTO>> GetFacultyById([FromRoute] int facultyId)
    {
        _logger.InfoFormat("Fetching faculty with id {0}", facultyId);

        FacultyResponseDTO faculties = await _service.GetFacultyById(facultyId);

        return Ok(faculties);
    }

    [HttpPost("faculties")]
    [ProducesResponseType(201)]
    [ProducesResponseType(422)]
    public async Task<ActionResult<FacultyResponseDTO>> CreateFaculty([FromBody] FacultyPostDTO facultyPostDto)
    {
        _logger.InfoFormat("Creating new faculty with name {0}", facultyPostDto.Name);

        FacultyResponseDTO createdFaculty = await _service.CreateFaculty(facultyPostDto);

        return CreatedAtAction(nameof(GetFacultyById), new { facultyId = createdFaculty.Id }, createdFaculty);
    }

    [HttpGet("specialisations/{specialisationId}")]
    [ProducesResponseType(200)]
    public async Task<ActionResult<SpecialisationResponseDTO>> GetSpecialisationById([FromRoute] int specialisationId)
    {
        _logger.InfoFormat("Fetching specialisation with id {0}", specialisationId);

        SpecialisationResponseDTO specialisations = await _service.GetSpecialisationById(specialisationId);

        return Ok(specialisations);
    }

    [HttpPost("specialisations")]
    [ProducesResponseType(201)]
    [ProducesResponseType(422)]
    public async Task<ActionResult<SpecialisationResponseDTO>> CreateSpecialisation([FromBody] SpecialisationPostDTO specialisationPostDto)
    {
        _logger.InfoFormat("Creating new specialisation with name {0}", specialisationPostDto.Name);

        SpecialisationResponseDTO createdSpecialisation = await _service.CreateSpecialisation(specialisationPostDto);

        return CreatedAtAction(nameof(GetSpecialisationById), new { specialisationId = createdSpecialisation.Id }, createdSpecialisation);
    }

    [HttpGet("group-years/{groupYearId}")]
    [ProducesResponseType(200)]
    public async Task<ActionResult<GroupYearResponseDTO>> GetGroupYearById([FromRoute] int groupYearId)
    {
        _logger.InfoFormat("Fetching group year with id {0}", groupYearId);

        GroupYearResponseDTO groupYear = await _service.GetGroupYearById(groupYearId);

        return Ok(groupYear);
    }

    [HttpPost("group-years")]
    [ProducesResponseType(201)]
    [ProducesResponseType(422)]
    public async Task<ActionResult<GroupYearResponseDTO>> CreateGroupYear([FromBody] GroupYearPostDTO groupYearPostDto)
    {
        _logger.InfoFormat("Creating new group year for specialisation ID {0} and year {1}", groupYearPostDto.SpecialisationId, groupYearPostDto.Year);
        
        GroupYearResponseDTO createdGroupYear = await _service.CreateGroupYear(groupYearPostDto);
        
        return CreatedAtAction(nameof(GetGroupYearById), new { groupYearId = createdGroupYear.Id }, createdGroupYear);
    }

    [HttpGet("student-groups/{studentGroupId}")]
    [ProducesResponseType(200)]
    public async Task<ActionResult<StudentGroupResponseDTO>> GetStudentGroupById([FromRoute] int studentGroupId)
    {
        _logger.InfoFormat("Fetching student group with id {0}", studentGroupId);

        StudentGroupResponseDTO studentGroup = await _service.GetStudentGroupById(studentGroupId);

        return Ok(studentGroup);
    }

    [HttpPost("student-groups")]
    [ProducesResponseType(201)]
    [ProducesResponseType(422)]
    public async Task<ActionResult<StudentGroupResponseDTO>> CreateStudentGroup([FromBody] StudentGroupPostDTO studentGroupPostDto)
    {
        _logger.InfoFormat("Creating new student group with name {0} for group year ID {1}", studentGroupPostDto.Name, studentGroupPostDto.GroupYearId);
        
        StudentGroupResponseDTO createdStudentGroup = await _service.CreateStudentGroup(studentGroupPostDto);
        
        return CreatedAtAction(nameof(GetStudentGroupById), new { studentGroupId = createdStudentGroup.Id }, createdStudentGroup);
    }

    [HttpGet("student-subgroups/{studentSubGroupId}")]
    [ProducesResponseType(200)]
    public async Task<ActionResult<StudentSubGroupResponseDTO>> GetStudentSubGroupById([FromRoute] int studentSubGroupId)
    {
        _logger.InfoFormat("Fetching student sub-group with id {0}", studentSubGroupId);

        StudentSubGroupResponseDTO studentSubGroup = await _service.GetStudentSubGroupById(studentSubGroupId);

        return Ok(studentSubGroup);
    }

    [HttpPost("student-subgroups")]
    [ProducesResponseType(201)]
    [ProducesResponseType(422)]
    public async Task<ActionResult<StudentSubGroupResponseDTO>> CreateStudentSubGroup([FromBody] StudentSubGroupPostDTO studentSubGroupPostDto)
    {
        _logger.InfoFormat("Creating new student sub-group with name {0} for student group ID {1}", studentSubGroupPostDto.Name, studentSubGroupPostDto.StudentGroupId);
        
        StudentSubGroupResponseDTO createdStudentSubGroup = await _service.CreateStudentSubGroup(studentSubGroupPostDto);
        
        return CreatedAtAction(nameof(GetStudentSubGroupById), new { studentSubGroupId = createdStudentSubGroup.Id }, createdStudentSubGroup);
    }
}