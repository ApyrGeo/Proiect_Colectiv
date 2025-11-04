namespace Backend.Domain.DTOs;

public class HourResponseDTO
{
    public required int Id { get; set; }
    public required string Day { get; set; }
    public required string HourInterval { get; set; }
    public required string Frequency { get; set; }
    public required string Category { get; set; }
    public required string Format { get; set; }
    public required LocationResponseDTO Location { get; set; }
    public string LocationUrl => $"{Location.Id}";
    public required ClassroomResponseDTO Classroom { get; set; }
    public string ClassroomUrl => $"{Classroom.Id}";
    public required SubjectResponseDTO Subject { get; set; }
    public string SubjectUrl => $"{Subject.Id}";
    public required TeacherResponseDTO Teacher { get; set; }
    public string TeacherUrl => $"{Teacher.User.Id}";
}
