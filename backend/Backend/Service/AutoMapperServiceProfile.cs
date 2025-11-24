using AutoMapper;
using TrackForUBB.Service.Contracts;
using TrackForUBB.Service.Contracts.Models;

namespace TrackForUBB.Service;

public class AutoMapperServiceProfile : Profile
{
    public AutoMapperServiceProfile()
    {
        CreateMap<ContractData, ContractViewModel>();
        CreateMap<ContractSubjectData, ContractSubjectViewModel>();
    }
}
