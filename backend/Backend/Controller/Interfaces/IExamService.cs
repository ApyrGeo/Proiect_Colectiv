using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackForUBB.Domain.DTOs;

namespace TrackForUBB.Controller.Interfaces;

public interface IExamService
{
    Task<List<ExamEntryResponseDTO>> GetExamsBySubjectId(int subjectId);
    Task<List<ExamEntryResponseDTO>> UpdateExamEntries(List<ExamEntryPutDTO> examEntries);
}
