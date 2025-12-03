using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackForUBB.Domain.DTOs;

namespace TrackForUBB.Service.Interfaces;

public interface IExamRepository
{
    Task<List<ExamEntryResponseDTO>> GetExamsBySubjectId(int subjectId);
    Task<List<ExamEntryForStudentDTO>> GetStudentExamsByStudentId(int studentId);
    Task<List<ExamEntryResponseDTO>> UpdateExamEntries(List<ExamEntryPutDTO> examEntries);
}
