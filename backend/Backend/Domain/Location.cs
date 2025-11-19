namespace TrackForUBB.Domain;
public class GoogleMapsData
{
    public string Id { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class Location
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public List<Classroom> Classrooms { get; set; } = [];
    public GoogleMapsData? GoogleMapsData { get; set; } = null;
}
