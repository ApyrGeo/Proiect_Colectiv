namespace TrackForUBB.Controller.Interfaces;

public interface IContractService
{
    Task<byte[]> GenerateContract(int userId);
}
