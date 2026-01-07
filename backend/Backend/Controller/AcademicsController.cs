using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackForUBB.Controller.Interfaces;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Utils;

namespace TrackForUBB.Controller;

[ApiController]
[Route("api/[controller]")]
public class AcademicsController(IAcademicsService service) : ControllerBase
{
    private readonly ILog _logger = LogManager.GetLogger(typeof(AcademicsController));
    private readonly IAcademicsService _service = service;

    [HttpGet("enrollments")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [Authorize]
    public async Task<ActionResult<List<EnrollmentResponseDTO>>> GetUserEnrollments([FromQuery] int userId)
    {
        _logger.InfoFormat("Fetching enrollment for user with ID {0}", userId);

        List<EnrollmentResponseDTO> enrollments = await _service.GetUserEnrollments(userId);

        return Ok(enrollments);
    }

    [HttpGet("enrollments/{enrollmentId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [Authorize]
    public async Task<ActionResult<EnrollmentResponseDTO>> GetEnrollmentById([FromRoute] int enrollmentId)
    {
        _logger.InfoFormat("Fetching enrollment with ID {0}", enrollmentId);

        var enrollment = await _service.GetEnrollmentById(enrollmentId);

        return Ok(enrollment);
    }

    [HttpPost("enrollments")]
    [ProducesResponseType(201)]
    [ProducesResponseType(422)]
    [Authorize(Roles = UserRolePermission.Admin)]
    public async Task<ActionResult<EnrollmentResponseDTO>> CreateUserEnrollment([FromBody] EnrollmentPostDTO enrollmentPostDto)
    {
        _logger.InfoFormat("Creating enrollment for user with ID {0}", enrollmentPostDto.UserId);

        EnrollmentResponseDTO createdEnrollment = await _service.CreateUserEnrollment(enrollmentPostDto);

        return CreatedAtAction(nameof(GetEnrollmentById), new { enrollmentId = createdEnrollment.Id }, createdEnrollment);
    }

    [HttpGet("teachers/{teacherId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [Authorize]
    public async Task<ActionResult<TeacherResponseDTO>> GetTeacherById([FromRoute] int teacherId)
    {
        _logger.InfoFormat("Fetching teacher with ID {0}", teacherId);

        var teacher = await _service.GetTeacherById(teacherId);

        return Ok(teacher);
    }

    [HttpGet("teacher/user/{userId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    [Authorize]
    public async Task<ActionResult<TeacherResponseDTO>> GetTeacherByUserId([FromRoute] int userId)
    {
        _logger.InfoFormat("Fetching teacher for user with ID {0}", userId);
        var teacher = await _service.GetTeacherByUserId(userId);
        return Ok(teacher);
    }

    [HttpGet("teachers/faculty/{facultyId}")]
    [ProducesResponseType(200)]
    public async Task<ActionResult<List<TeacherResponseDTO>>> GetAllTeachersByFacultyId([FromRoute] int facultyId)
    {
        _logger.Info("Fetching all teachers");
        List<TeacherResponseDTO> teachers = await _service.GetAllTeachersByFacultyId(facultyId);
        return Ok(teachers);
    }

    [HttpPost("teachers")]
    [ProducesResponseType(201)]
    [ProducesResponseType(422)]
    [Authorize(Roles = UserRolePermission.Admin)]
    public async Task<ActionResult<EnrollmentResponseDTO>> CreateTeacher([FromBody] TeacherPostDTO dto)
    {
        _logger.InfoFormat("Creating teacher for user with ID {0}", dto.UserId);

        var createdTeacher = await _service.CreateTeacher(dto);

        return CreatedAtAction(nameof(GetTeacherById), new { teacherId = createdTeacher.Id }, createdTeacher);
    }

    [HttpGet("faculties/{facultyId}")]
    [ProducesResponseType(200)]
    public async Task<ActionResult<FacultyResponseDTO>> GetFacultyById([FromRoute] int facultyId)
    {
        _logger.InfoFormat("Fetching faculty with id {0}", facultyId);

        FacultyResponseDTO faculties = await _service.GetFacultyById(facultyId);

        return Ok(faculties);
    }

    [HttpGet("faculties")]
    [ProducesResponseType(200)]
    public async Task<ActionResult<List<FacultyResponseDTO>>> GetAllFaculties()
    {
        _logger.Info("Fetching all faculties");
        List<FacultyResponseDTO> faculties = await _service.GetAllFaculties();
        return Ok(faculties);
    }

    [HttpPost("faculties")]
    [ProducesResponseType(201)]
    [ProducesResponseType(422)]
    [Authorize(Roles = UserRolePermission.Admin)]
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
    [Authorize(Roles = UserRolePermission.Admin)]
    public async Task<ActionResult<SpecialisationResponseDTO>> CreateSpecialisation([FromBody] SpecialisationPostDTO specialisationPostDto)
    {
        _logger.InfoFormat("Creating new specialisation with name {0}", specialisationPostDto.Name);

        SpecialisationResponseDTO createdSpecialisation = await _service.CreateSpecialisation(specialisationPostDto);

        return CreatedAtAction(nameof(GetSpecialisationById), new { specialisationId = createdSpecialisation.Id }, createdSpecialisation);
    }

    [HttpGet("promotions/{promotionId}")]
    [ProducesResponseType(200)]
    public async Task<ActionResult<PromotionResponseDTO>> GetPromotionById([FromRoute] int promotionId)
    {
        _logger.InfoFormat("Fetching promotion with id {0}", promotionId);

        PromotionResponseDTO promotion = await _service.GetPromotionById(promotionId);

        return Ok(promotion);
    }

    [HttpPost("promotions")]
    [ProducesResponseType(201)]
    [ProducesResponseType(422)]
    [Authorize(Roles = UserRolePermission.Admin)]
    public async Task<ActionResult<PromotionResponseDTO>> CreatePromotion([FromBody] PromotionPostDTO promotionPostDto)
    {
        _logger.InfoFormat("Creating new group year for specialisation ID {0} and years {1} {2}", promotionPostDto.SpecialisationId, promotionPostDto.StartYear, promotionPostDto.EndYear);

        PromotionResponseDTO createdPromotion = await _service.CreatePromotion(promotionPostDto);

        return CreatedAtAction(nameof(GetPromotionById), new { promotionId = createdPromotion.Id }, createdPromotion);
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
    [Authorize(Roles = UserRolePermission.Admin)]
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
    [Authorize(Roles = UserRolePermission.Admin)]
    public async Task<ActionResult<StudentSubGroupResponseDTO>> CreateStudentSubGroup([FromBody] StudentSubGroupPostDTO studentSubGroupPostDto)
    {
        _logger.InfoFormat("Creating new student sub-group with name {0} for student group ID {1}", studentSubGroupPostDto.Name, studentSubGroupPostDto.StudentGroupId);

        StudentSubGroupResponseDTO createdStudentSubGroup = await _service.CreateStudentSubGroup(studentSubGroupPostDto);

        return CreatedAtAction(nameof(GetStudentSubGroupById), new { studentSubGroupId = createdStudentSubGroup.Id }, createdStudentSubGroup);
    }
    
    [HttpGet("student-groups/{studentGroupId}/students")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<List<EnrollmentResponseDTO>>> GetStudentsByStudentGroup([FromRoute] int studentGroupId)
    {
        _logger.InfoFormat("Fetching students for student group ID {0}", studentGroupId);

        var students = await _service.GetStudentsByStudentGroup(studentGroupId);

        return Ok(students);
    }
}