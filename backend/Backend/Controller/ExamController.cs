using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TrackForUBB.Controller.Interfaces;
using TrackForUBB.Domain.DTOs;

namespace TrackForUBB.Controller;

[ApiController]
[Route("api/[controller]")]
public class ExamController(IExamService examService) : ControllerBase
{
    private readonly IExamService _examService = examService;

    [HttpGet]
    [ProducesResponseType(404)]
    [ProducesResponseType(200)]
    public async Task<ActionResult<List<ExamEntryResponseDTO>>> GetExams([FromQuery] int subjectId)
    {
        var exams = await _examService.GetExamsBySubjectId(subjectId);
        return Ok(exams);
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
