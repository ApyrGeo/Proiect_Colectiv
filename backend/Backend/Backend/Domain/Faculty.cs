namespace Backend.Domain;

public class Faculty
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<Specialisation> Specialisations { get; set; } = [];
}
