using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackForUBB.Domain.DTOs;

public class ExamEntryResponseDTO
{
    public int Id { get; set; }
    public required DateTime Date { get; set; }
    public required int Duration { get; set; }
    public required ClassroomResponseDTO Classroom { get; set; }
    public required SubjectResponseDTO Subject { get; set; }
    public required StudentGroupResponseDTO StudentGroup { get; set; }
}
