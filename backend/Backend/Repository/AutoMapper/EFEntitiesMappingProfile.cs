using AutoMapper;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Enums;
using TrackForUBB.Repository.EFEntities;

namespace TrackForUBB.Repository.AutoMapper;
public class EFEntitiesMappingProfile : Profile
{
    public EFEntitiesMappingProfile()
    {
        CreateMap<User, UserResponseDTO>().ReverseMap();
        CreateMap<UserPostDTO, User>();

        CreateMap<Enrollment, EnrollmentResponseDTO>().ReverseMap();
        CreateMap<EnrollmentPostDTO, Enrollment>();

        CreateMap<Faculty, FacultyResponseDTO>().ReverseMap();
        CreateMap<FacultyPostDTO, Faculty>();

        CreateMap<Specialisation, SpecialisationResponseDTO>().ReverseMap();
        CreateMap<SpecialisationPostDTO, Specialisation>();

        CreateMap<Promotion, PromotionResponseDTO>().ReverseMap();
        CreateMap<PromotionPostDTO, Promotion>();

        CreateMap<StudentGroup, StudentGroupResponseDTO>().ReverseMap();
        CreateMap<StudentGroupPostDTO, StudentGroup>();

        CreateMap<StudentSubGroup, StudentSubGroupResponseDTO>().ReverseMap();
        CreateMap<StudentSubGroupPostDTO, StudentSubGroup>();

        CreateMap<Subject, SubjectResponseDTO>().ReverseMap();
        CreateMap<SubjectPostDTO, Subject>();

        CreateMap<Teacher, TeacherResponseDTO>().ReverseMap();
        CreateMap<TeacherPostDTO, Teacher>();

        CreateMap<Classroom, ClassroomResponseDTO>().ReverseMap();
        CreateMap<ClassroomPostDTO, Classroom>();

        CreateMap<GoogleMapsData, GoogleMapsDataResponseDTO>().ReverseMap();

        CreateMap<Location, LocationResponseDTO>().ReverseMap();
        CreateMap<LocationPostDTO, Location>();

        CreateMap<Hour, HourResponseDTO>()
            .ForMember(x => x.Day, o => o.MapFrom(s => s.Day.ToString()))
            .ForMember(x => x.Frequency, o => o.MapFrom(s => s.Frequency.ToString()))
            .ForMember(x => x.Category, o => o.MapFrom(s => s.Category.ToString()))
            .ForMember(x => x.Location, o => o.MapFrom(s => s.Classroom.Location))
            .ForMember(x => x.Format, o => o.MapFrom(s =>
                s.StudentSubGroup != null ? s.StudentSubGroup.Name
                : s.StudentGroup != null ? s.StudentGroup.Name
                : s.Promotion != null ? s.Promotion.Specialisation.Name + " " + (DateTime.Now.Year - s.Promotion.StartYear + (DateTime.Now.Month < 7 ? 0 : 1))
				: "Unknown"
            ));
        CreateMap<HourPostDTO, Hour>()
            .ForMember(x => x.Day, o => o.MapFrom(s => Enum.Parse<HourDay>(s.Day!)))
            .ForMember(x => x.Frequency, o => o.MapFrom(s => Enum.Parse<HourFrequency>(s.Frequency!)))
            .ForMember(x => x.Category, o => o.MapFrom(s => Enum.Parse<HourCategory>(s.Category!)));
    }
}
