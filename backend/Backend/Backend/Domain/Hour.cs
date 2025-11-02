using Backend.Domain.Enums;

namespace Backend.Domain
{
    public class Hour
    {
        public int Id { get; set; }
        public required HourDay Day { get; set; }
        public required string HourInterval { get; set; }
        public required HourFrequency Frequency { get; set; }
        public required string Subject { get; set; }

        public int ClassroomId { get; set; }
        public required Classroom Classroom { get; set; }

        public int TeacherId { get; set; }
        public required Teacher Teacher { get; set; }

        public int? GroupYearId { get; set; } = null;
        public GroupYear? GroupYear { get; set; } = null;

        public int? StudentGroupId { get; set; } = null;
        public StudentGroup? StudentGroup { get; set; } = null;

        public int? StudentSubGroupId { get; set; } = null;
        public StudentSubGroup? StudentSubGroup { get; set; } = null;
    }
}
