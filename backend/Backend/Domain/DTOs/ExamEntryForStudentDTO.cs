using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackForUBB.Domain.DTOs;

public class ExamEntryForStudentDTO
{
    public required string Specialisation { get; set; }
    public List<ExamEntryResponseDTO> ExamEntries { get; set; } = [];
}
