namespace Backend.Domain;

public class Specialisation
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required List<GroupYear> GroupYears { get; set; }
    public required int FacultyId { get; set; }
    public required Faculty Faculty { get; set; }
}