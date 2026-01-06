using TrackForUBB.Service.Contracts.Models;

namespace TrackForUBB.Service.Interfaces;

public interface IContractUnitOfWork
{
    Task<ContractData> GetData(int userId, int promotionId, int yearNumber);
}
