using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackForUBB.Domain.DTOs;

public class ExamEntryPutDTO
{
    public required int Id { get; set; }
    public required DateTime Date { get; set; }
    public required int Duration { get; set; }
    public required int ClassroomId { get; set; }
    public required int SubjectId { get; set; }
    public required int StudentGroupId { get; set; }
}
