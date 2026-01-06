using log4net;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TrackForUBB.Controller.Interfaces;
using TrackForUBB.Domain.DTOs;

namespace TrackForUBB.Controller;

[ApiController]
[Route("api/[controller]")]
public class ExamController(IExamService examService) : ControllerBase
{
    private readonly ILog _logger = LogManager.GetLogger(typeof(ExamController));
    private readonly IExamService _examService = examService;

    [HttpGet("subject/{subjectId}")]
    [ProducesResponseType(404)]
    [ProducesResponseType(200)]
    public async Task<ActionResult<List<ExamEntryResponseDTO>>> GetExamsForTeacher([FromRoute] int subjectId)
    {
        var exams = await _examService.GetExamsBySubjectId(subjectId);
        return Ok(exams);
    }

    [HttpGet("student/{studentId}")]
    [ProducesResponseType(404)]
    [ProducesResponseType(200)]
    public async Task<ActionResult<List<ExamEntryForStudentDTO>>> GetExamsForStudent([FromRoute] int studentId)
    {
        var exams = await _examService.GetStudentExamsByStudentId(studentId);
        return Ok(exams);
    }

    [HttpPost("generate-exam-entries")]
    [ProducesResponseType(200)]
    public async Task<ActionResult<List<ExamEntryResponseDTO>>> GenerateExamEntries([FromBody] GenerateExamEntriesRequestDTO request)
    {
        _logger.Info("Received request to generate exam entries.");
        var examEntries = await _examService.GenerateExamEntries(request);
        return Ok(examEntries);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteExamEntry([FromRoute] int id)
    {
        _logger.Info($"Received request to delete exam entry with ID: {id}");
        await _examService.DeleteExamEntry(id);
        return NoContent();
    }

    [HttpPut]
    [ProducesResponseType(422)]
    [ProducesResponseType(200)]
    public async Task<ActionResult<List<ExamEntryResponseDTO>>> UpdateExamEntryList([FromBody] List<ExamEntryPutDTO> examEntries)
    {
        var updatedExams = await _examService.UpdateExamEntries(examEntries);
        return Ok(updatedExams);
    }
}
