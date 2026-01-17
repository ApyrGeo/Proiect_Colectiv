using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackForUBB.Domain.DTOs;

namespace TrackForUBB.Controller.Interfaces;

public interface IExamService
{
    Task DeleteExamEntry(int id);
    Task<List<ExamEntryResponseDTO>> GenerateExamEntries(GenerateExamEntriesRequestDTO request);
    Task<List<ExamEntryResponseDTO>> GetExamsBySubjectId(int subjectId);
    Task<List<ExamEntryForStudentDTO>> GetStudentExamsByStudentId(int studentId);
    Task<List<ExamEntryResponseDTO>> UpdateExamEntries(List<ExamEntryPutDTO> examEntries);
}
