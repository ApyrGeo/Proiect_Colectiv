namespace TrackForUBB.Service.Contracts;

public class ContractSubjectViewModel
{
    public int I { get; set; }
    public required string Code { get; set; }
    public required string Type { get; set; }
    public required string Name { get; set; }
    public int Credits { get; set; }
}
