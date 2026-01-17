using AutoMapper;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Service.Contracts;
using TrackForUBB.Service.Contracts.Models;

namespace TrackForUBB.Service;

public class AutoMapperServiceProfile : Profile
{
    public AutoMapperServiceProfile()
    {
        CreateMap<ContractData, ContractViewModel>();
        CreateMap<ContractSubjectData, ContractSubjectViewModel>();

        CreateMap<UserPostDTO, InternalUserPostDTO>()
            .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => (string?)null))
            .ForMember(dest => dest.SignatureBase64, opt => opt.MapFrom(src => (string?)null))
            .ForMember(dest => dest.TenantEmail, opt => opt.MapFrom(src => (string?)null));
    }
}
