namespace TrackForUBB.Domain.DTOs;

public class BulkEnrollmentItem ()
{
    public required string UserEmail { get; set; }
}
public class BulkSubGroupItem ()
{
    public required string Name { get; set; }
    public List<BulkEnrollmentItem> Enrollments = [];
}
public class BulkGroupItem ()
{
    public required string Name { get; set; }
    public List<BulkSubGroupItem> SubGroups { get; set; } = [];  
}

public class BulkPromotionPostDTO
{
    public int StartYear { get; set; }
    public int EndYear { get; set; }
    public required int SpecialisationId { get; set; }
    public List<BulkGroupItem> Groups { get; set; } = [];
}
