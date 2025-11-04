namespace Backend.Domain.DTOs
{
    public class HourPostDTO
    {
        public string? Day { get; set; }
        public string? HourInterval { get; set; }
        public string? Frequency { get; set; }
        public string? Category { get; set; }
        public int? ClassroomId { get; set; }
        public int? SubjectId { get; set; }
        public int? TeacherId { get; set; }

        public int? GroupYearId { get; set; } = null;
        public int? StudentGroupId { get; set; } = null;
        public int? StudentSubGroupId { get; set; } = null;
    }
}
