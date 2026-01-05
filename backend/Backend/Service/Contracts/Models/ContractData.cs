namespace TrackForUBB.Service.Contracts.Models;

public class ContractData
{
    public required string FacultyName { get; set; }
    public required string Domain { get; set; }
    public required string Specialization { get; set; }
    public required string Language { get; set; }
    public required string StudentYear { get; set; }
    public required List<ContractSubjectData> SubjectsSemester1;
    public required List<ContractSubjectData> SubjectsSemester2;

    public required string FullName { get; set; }
    public required string StudentId { get; set; }
    public required string IdCardSeries { get; set; }
    public required string IdCardNumber { get; set; }
    public required string CNP { get; set; }
    public required string StudentPhone { get; set; }
    public required string StudentEmail { get; set; }
    public required string UniversityYear { get; set; }
}
