namespace Domain;

public class Classroom
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<Hour> Hours { get; set; } = [];
    public int LocationId { get; set; }
    public required Location Location { get; set; }
}
