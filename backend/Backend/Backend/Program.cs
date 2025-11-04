using Backend.Context;
using Backend.DataSeeder;
using Backend.Domain;
using Backend.Domain.DTOs;
using Backend.Domain.Enums;
using Backend.Interfaces;
using Backend.Middlewares;
using Backend.Repository;
using Backend.Service;
using Backend.Service.Validators;
using EmailService.Configuration;
using EmailService.Providers;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using IValidatorFactory = Backend.Interfaces.IValidatorFactory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//database
builder.Services.AddDbContext<AcademicAppContext>((sp, options) =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
        );
});

builder.Services.AddAutoMapper(cfg =>
{
    // Register your mappings here
    // Example: cfg.CreateMap<Source, Destination>();

    cfg.CreateMap<User, UserResponseDTO>().ReverseMap();
    cfg.CreateMap<UserPostDTO, User>();

    cfg.CreateMap<Enrollment, EnrollmentResponseDTO>().ReverseMap();
    cfg.CreateMap<EnrollmentPostDTO, Enrollment>();

    cfg.CreateMap<Faculty, FacultyResponseDTO>().ReverseMap();
    cfg.CreateMap<FacultyPostDTO, Faculty>();

    cfg.CreateMap<Specialisation, SpecialisationResponseDTO>().ReverseMap();
    cfg.CreateMap<SpecialisationPostDTO, Specialisation>();

    cfg.CreateMap<GroupYear, GroupYearResponseDTO>().ReverseMap();
    cfg.CreateMap<GroupYearPostDTO, GroupYear>();

    cfg.CreateMap<StudentGroup, StudentGroupResponseDTO>().ReverseMap();
    cfg.CreateMap<StudentGroupPostDTO, StudentGroup>();

    cfg.CreateMap<StudentSubGroup, StudentSubGroupResponseDTO>().ReverseMap();
    cfg.CreateMap<StudentSubGroupPostDTO, StudentSubGroup>();

    cfg.CreateMap<Subject, SubjectResponseDTO>().ReverseMap();
    cfg.CreateMap<SubjectPostDTO, Subject>();

    cfg.CreateMap<Teacher, TeacherResponseDTO>().ReverseMap();
    cfg.CreateMap<TeacherPostDTO, Teacher>();

    cfg.CreateMap<Classroom, ClassroomResponseDTO>().ReverseMap();
    cfg.CreateMap<ClassroomPostDTO, Classroom>();

    cfg.CreateMap<Location, LocationResponseDTO>().ReverseMap();
    cfg.CreateMap<LocationPostDTO, Location>();

    cfg.CreateMap<Hour, HourResponseDTO>()
        .ForMember(x => x.Day, o => o.MapFrom(s => s.Day.ToString()))
        .ForMember(x => x.Frequency, o => o.MapFrom(s => s.Frequency.ToString()))
        .ForMember(x => x.Category, o => o.MapFrom(s => s.Category.ToString()))
        .ForMember(x => x.Location, o => o.MapFrom(s => s.Classroom.Location))
        .ForMember(x => x.Format, o => o.MapFrom(s =>
            s.StudentSubGroup != null ? s.StudentSubGroup.Name
            : s.StudentGroup != null ? s.StudentGroup.Name
            : s.GroupYear != null ? s.GroupYear.Year
            : "Unknown"
        ));
    cfg.CreateMap<HourPostDTO, Hour>()
        .ForMember(x => x.Day, o => o.MapFrom(s => Enum.Parse<HourDay>(s.Day!)))
        .ForMember(x => x.Frequency, o => o.MapFrom(s => Enum.Parse<HourFrequency>(s.Frequency!)))
        .ForMember(x => x.Category, o => o.MapFrom(s => Enum.Parse<HourCategory>(s.Category!)));
});

//logging
builder.Logging.ClearProviders();
builder.Logging.AddLog4Net("log4net.config");

//validators
builder.Services.AddScoped<IValidatorFactory, ValidatorFactory>();
builder.Services.AddValidatorsFromAssemblyContaining<UserPostDTOValidator>();

//repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAcademicRepository, AcademicRepository>();
builder.Services.AddScoped<ITimetableRepository, TimetableRepository>();

//helpers
builder.Services.Configure<PasswordHasherOptions>(
    options => options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3
    );
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

//email
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
builder.Services.AddSingleton(resolver =>
    resolver.GetRequiredService<IOptions<EmailSettings>>().Value);
builder.Services.AddScoped<EmailProvider>();

//services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAcademicsService, AcademicsService>();
builder.Services.AddScoped<ITimetableService, TimetableService>();

//data seeders
builder.Services.AddScoped<UniversityDataSeeder>();
builder.Services.AddScoped<UserDataSeeder>();
builder.Services.AddScoped<LocationDataSeeder>();
builder.Services.AddScoped<TeacherDataSeeder>();
builder.Services.AddScoped<SubjectDataSeeder>();
builder.Services.AddScoped<HourDataSeeder>();
builder.Services.AddScoped<GlobalDataSeeder>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//try seed DB
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<GlobalDataSeeder>();
    await seeder.SeedAsync();
}

app.Run();