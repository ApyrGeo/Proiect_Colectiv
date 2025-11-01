namespace Backend.Domain;

public class Faculty
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required List<Specialisation> Specialisations { get; set; } = [];
}
