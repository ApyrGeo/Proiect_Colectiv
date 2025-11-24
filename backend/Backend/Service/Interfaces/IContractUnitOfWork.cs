using TrackForUBB.Service.Contracts.Models;

namespace TrackForUBB.Service.Interfaces;

public interface IContractUnitOfWork
{
    Task<List<ContractData>> GetData(int userId);
}
