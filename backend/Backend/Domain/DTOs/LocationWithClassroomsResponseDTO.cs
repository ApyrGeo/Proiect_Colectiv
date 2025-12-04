using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackForUBB.Domain.DTOs;

public class LocationWithClassroomsResponseDTO
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required List<ClassroomResponseDTO> Classrooms { get; set; }
}
