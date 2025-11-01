namespace Backend.Domain
{
    public class Teacher
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required User User { get; set; }
        public required List<Hour> Hours { get; set; } = [];
    }
}
