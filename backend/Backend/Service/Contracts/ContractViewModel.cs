namespace TrackForUBB.Service.Contracts;

public class ContractViewModel
{
    public required string FacultyName { get; set; }
    public required string UniversityYear { get; set; }
    public required string Domain { get; set; }
    public required string Specialization { get; set; }
    public required string Language { get; set; }
    public required string StudentYear { get; set; }
    public required string FullName { get; set; }
    public required string StudentId { get; set; }
    public required string IdCardSeries { get; set; }
    public required string IdCardNumber { get; set; }
    public required string CNP { get; set; }
    public required string StudentPhone { get; set; }
    public required string StudentEmail { get; set; }
    public required List<ContractSubjectViewModel> SubjectsSemester1;
    public required int TotalCreditsSemester1 { get; set; }
    public required List<ContractSubjectViewModel> SubjectsSemester2;
    public required int TotalCreditsSemester2 { get; set; }

    public required string? Signature { get; set; }

    public string Now { get; set; } = string.Empty;
}
