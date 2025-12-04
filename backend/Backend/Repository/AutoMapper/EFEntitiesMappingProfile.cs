using AutoMapper;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Enums;
using TrackForUBB.Domain.Utils;
using TrackForUBB.Repository.EFEntities;

namespace TrackForUBB.Repository.AutoMapper;
public class EFEntitiesMappingProfile : Profile
{
    public EFEntitiesMappingProfile()
    {
        CreateMap<User, UserResponseDTO>().ReverseMap();
        CreateMap<UserPostDTO, User>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

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
        CreateMap<Location, LocationWithClassroomsResponseDTO>();
        CreateMap<LocationPostDTO, Location>();
        
        CreateMap<Grade, GradeResponseDTO>()
            .ForMember(x => x.Semester, o => o.MapFrom(s => s.Semester));;
        CreateMap<GradePostDTO, Grade>();
        
        CreateMap<PromotionSemester, PromotionSemesterResponseDTO>().ReverseMap();
        
        CreateMap<PromotionYear, PromotionYearResponseDTO>().ReverseMap();

        CreateMap<User, UserProfileResponseDTO>()
            .ForMember(dest => dest.SignatureUrl,
                opt => opt.MapFrom(src => src.Signature != null ? Convert.ToBase64String(src.Signature) : null));
        
        CreateMap<Hour, HourResponseDTO>()
            .ForMember(x => x.Day, o => o.MapFrom(s => s.Day.ToString()))
            .ForMember(x => x.Frequency, o => o.MapFrom(s => s.Frequency.ToString()))
            .ForMember(x => x.Category, o => o.MapFrom(s => s.Category.ToString()))
            .ForMember(x => x.Location, o => o.MapFrom(s => s.Classroom.Location))
            .ForMember(x => x.Format, o => o.MapFrom(s =>
                s.StudentSubGroup != null ? s.StudentSubGroup.Name
                : s.StudentGroup != null ? s.StudentGroup.Name
                : s.Promotion != null ? s.Promotion.Specialisation.Name + " " + HelperFunctions.GetCurrentStudentYear(s.Promotion.StartYear)
				: "Unknown"
            ));
        CreateMap<HourPostDTO, Hour>()
            .ForMember(x => x.Day, o => o.MapFrom(s => Enum.Parse<HourDay>(s.Day!)))
            .ForMember(x => x.Frequency, o => o.MapFrom(s => Enum.Parse<HourFrequency>(s.Frequency!)))
            .ForMember(x => x.Category, o => o.MapFrom(s => Enum.Parse<HourCategory>(s.Category!)));


        CreateMap<ExamEntry, ExamEntryResponseDTO>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.ExamDate))
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
            .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Subject))
            .ForMember(dest => dest.Classroom, opt => opt.MapFrom(src => src.Classroom))
            .ForMember(dest => dest.StudentGroup, opt => opt.MapFrom(src => src.StudentGroup));

        CreateMap<ExamEntryPutDTO, ExamEntry>()
           .ForMember(dest => dest.ExamDate, opt => opt.MapFrom(src => src.Date))
           .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
           .ForMember(dest => dest.ClassroomId, opt => opt.MapFrom(src => src.ClassroomId))
           .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.SubjectId))
           .ForMember(dest => dest.StudentGroupId, opt => opt.MapFrom(src => src.StudentGroupId));
    }
}
