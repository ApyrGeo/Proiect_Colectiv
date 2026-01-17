using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackForUBB.Domain.DTOs;

public class ScholarshipStatusDTO
{
    public required double AverageScore { get; set; }
    public required int Rank { get; set; }
    public required int TotalStudents { get; set; }
    public required bool IsEligible { get; set; }
    public string? ScholarshipType { get; set; }
}
