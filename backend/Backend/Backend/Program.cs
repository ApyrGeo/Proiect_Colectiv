using TrackForUBB.Repository.Context;
using TrackForUBB.Service.Interfaces;
using TrackForUBB.Repository;
using TrackForUBB.Service.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using IValidatorFactory = TrackForUBB.Service.Interfaces.IValidatorFactory;
using TrackForUBB.Service;
using TrackForUBB.Service.EmailService.Interfaces;
using TrackForUBB.Service.EmailService.Configuration;
using TrackForUBB.Service.EmailService.Providers;
using TrackForUBB.Domain.Security;
using TrackForUBB.Repository.DataSeeder;
using TrackForUBB.Controller.Middlewares;
using TrackForUBB.Controller.Security;
using TrackForUBB.Repository.AutoMapper;
using TrackForUBB.Controller.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var AppAllowSpecificOrigins = "_appAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AppAllowSpecificOrigins, policy =>
    {
        policy.AllowAnyHeader().AllowAnyOrigin();
    });
});


//database
builder.Services.AddDbContext<AcademicAppContext>((sp, options) =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
        );
});

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<EFEntitiesMappingProfile>());

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
builder.Services.AddSingleton(typeof(IAdapterPasswordHasher<>), typeof(AspNetCorePasswordHasher<>));

//email
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
builder.Services.AddSingleton(resolver =>
    resolver.GetRequiredService<IOptions<EmailSettings>>().Value);
builder.Services.AddScoped<IEmailProvider, EmailProvider>();

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

app.UseCors(AppAllowSpecificOrigins);

//try seed DB
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<GlobalDataSeeder>();
    await seeder.SeedAsync();
}

app.Run();