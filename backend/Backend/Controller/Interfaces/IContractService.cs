using TrackForUBB.Domain.DTOs.Contracts;

namespace TrackForUBB.Controller.Interfaces;

public interface IContractService
{
    Task<byte[]> GenerateContract(int userId, ContractPostRequest request);
}
