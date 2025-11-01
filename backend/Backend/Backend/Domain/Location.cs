namespace Backend.Domain
{
    public class Location
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public required List<Classroom> Classrooms { get; set; } = [];
    }
}
