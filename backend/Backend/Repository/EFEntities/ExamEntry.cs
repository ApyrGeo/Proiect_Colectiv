using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackForUBB.Repository.EFEntities;

public class ExamEntry
{
    public int Id { get; set; }
    public DateTime? ExamDate { get; set; } 
    public int? Duration { get; set; } 
    public int? ClassroomId { get; set; } 
    public Classroom? Classroom { get; set; } 
    public int SubjectId { get; set; }
    public required Subject Subject { get; set; }
    public int StudentGroupId { get; set; }
    public required StudentGroup StudentGroup { get; set; }
}
