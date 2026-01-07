using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackForUBB.Domain.DTOs;

public class SubjectGroupGradesDTO
{
    public required SubjectResponseDTO Subject { get; set; }
    public required StudentGroupResponseDTO StudentGroup { get; set; }
    public required List<UserGradeDTO> Grades { get; set; }
}
