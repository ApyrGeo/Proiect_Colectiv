namespace TrackForUBB.Service.Contracts.Models;

public class ContractRequestModel
{
    public required IDictionary<int, int> OptionalToSubjectCodesSem1 { get; set; }
    public required IDictionary<int, int> OptionalToSubjectCodesSem2 { get; set; }
    public string? SignatureBase64 { get; set; }

    public int PromotionId { get; set; }
    public int Year { get; set; }
    public int UserId { get; set; }
}
