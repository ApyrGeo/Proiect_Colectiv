using Backend.Domain.Enums;

namespace Backend.Domain
{
    public class Hour
    {
        public int Id { get; set; }
        public required HourDay Day { get; set; }
        public required string HourInterval { get; set; }
        public required HourFrequency Frequency { get; set; }
        public required string Format { get; set; }

        public required string Subject { get; set; }

        public int ClassroomId { get; set; }
        public required Classroom Classroom { get; set; }

        public int TeacherId { get; set; }
        public required Teacher Teacher { get; set; }

        public int SpecialisationId { get; set; }
        public required Specialisation Specialisation { get; set; }
    }
}
