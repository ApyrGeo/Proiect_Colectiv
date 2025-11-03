namespace Backend.Domain.DTOs
{
    public class HourPostDTO
    {
        public required string Day { get; set; }
        public required string HourInterval { get; set; }
        public required string Frequency { get; set; }
        public required int ClassroomId { get; set; }
        public required int SubjectId { get; set; }
        public required int TeacherId { get; set; }

        public int? GroupYearId { get; set; } = null;
        public int? StudentGroupId { get; set; } = null;
        public int? StudentSubGroupId { get; set; } = null;
    }
}
