namespace TrackForUBB.Service.EmailService.Models;

public class PostedSemesterGradesModel
{
    public string UserFirstName { get; set; } = string.Empty; 
    public string UserLastName { get; set; } = string.Empty;
    public int YearOfStudy { get; set; }
    public int SemesterNumber { get; set; }
    public string BaseUrl { get; set; } = string.Empty;
    
}