using Azure.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using TrackForUBB.Controller.Interfaces;
using TrackForUBB.Controller.Middlewares;
using TrackForUBB.Controller.Security;
using TrackForUBB.Domain.Security;
using TrackForUBB.Repository;
using TrackForUBB.Repository.AutoMapper;
using TrackForUBB.Repository.Context;
using TrackForUBB.Repository.DataSeeder;
using TrackForUBB.Service;
using TrackForUBB.Service.Contracts;
using TrackForUBB.Service.EmailService.Configuration;
using TrackForUBB.Service.EmailService.Interfaces;
using TrackForUBB.Service.EmailService.Providers;
using TrackForUBB.Service.Interfaces;
using TrackForUBB.Service.PdfGeneration;
using TrackForUBB.Service.Validators;
using IValidatorFactory = TrackForUBB.Service.Interfaces.IValidatorFactory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var oauthScheme = new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        In = ParameterLocation.Header,
        Name = "Authorization",
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri(
                    $"https://login.microsoftonline.com/{builder.Configuration["AzureAd:TenantId"]}/oauth2/v2.0/authorize"),
                TokenUrl = new Uri(
                    $"https://login.microsoftonline.com/{builder.Configuration["AzureAd:TenantId"]}/oauth2/v2.0/token"),
                Scopes = new Dictionary<string, string>
                {
                    { $"api://{builder.Configuration["AzureAd:ClientId"]}/TrackForUBB.Read", "Read access to TrackForUBB" },
                    { $"api://{builder.Configuration["AzureAd:ClientId"]}/TrackForUBB.ReadWrite", "Read/Write access to TrackForUBB" }
                }
            }
        }
    };

    c.AddSecurityDefinition("oauth2", oauthScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new[]
            {
                $"api://{builder.Configuration["AzureAd:ClientId"]}/TrackForUBB.Read",
                $"api://{builder.Configuration["AzureAd:ClientId"]}/TrackForUBB.ReadWrite"
            }
        }
    });
});

var AppAllowSpecificOrigins = "_appAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AppAllowSpecificOrigins, policy =>
    {
        policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddSingleton<GraphServiceClient>(sp =>
{
    var config = builder.Configuration.GetSection("Graph");

    var tenantId = config["TenantId"];
    var clientId = config["ClientId"];
    var clientSecret = config["ClientSecret"];

    var options = new ClientSecretCredentialOptions
    {
        AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
    };

    var clientSecretCredential = new ClientSecretCredential(
        tenantId, clientId, clientSecret, options);

    return new GraphServiceClient(clientSecretCredential, ["https://graph.microsoft.com/.default"]);
});

//database
builder.Services.AddDbContext<AcademicAppContext>((sp, options) =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
        );
});

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<EFEntitiesMappingProfile>();
    cfg.AddProfile<AutoMapperServiceProfile>();
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
builder.Services.AddScoped<IGradeRepository, GradeRepository>();
builder.Services.AddScoped<IExamRepository, ExamRepository>();

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
builder.Services.AddScoped<IGradeService, GradeService>();
builder.Services.AddScoped<IExamService, ExamService>();
builder.Services
    .AddScoped<IContractService, ContractService>()
    .AddScoped<IContractUnitOfWork, ContractUnitOfWork>()
    .AddSingleton<IDocumentTemplateFiller, DocumentTemplateFiller>()
    .AddSingleton<IPdfConverter, PdfConverter>()
    .AddSingleton<IPdfGenerator, PdfGenerator>()
    .AddSingleton<IXmlTemplateFiller, XmlTemplateFiller>()
    .Configure<PdfConverterConfiguration>(builder.Configuration.GetSection("PdfConverter"))
    .AddSingleton(resolver => resolver.GetRequiredService<IOptions<PdfConverterConfiguration>>().Value);


//data seeders
builder.Services.AddScoped<UniversityDataSeeder>();
builder.Services.AddScoped<UserDataSeeder>();
builder.Services.AddScoped<LocationDataSeeder>();
builder.Services.AddScoped<TeacherDataSeeder>();
builder.Services.AddScoped<SubjectDataSeeder>();
builder.Services.AddScoped<HourDataSeeder>();
builder.Services.AddScoped<GradesDataSeeder>();
builder.Services.AddScoped<MicrosoftEntraUserDataSeeder>();
builder.Services.AddScoped<GlobalDataSeeder>();
builder.Services.AddScoped<ExamDataSeeder>();


// https://learn.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-9.0
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy  =>
        {
            var feUrl = builder.Configuration.GetValue<string>("Email:BaseUrl");
            policy
                .WithOrigins(feUrl)
                .AllowAnyHeader().AllowAnyMethod();
                ;
        });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
        c.OAuthClientId(builder.Configuration["AzureAd:SwaggerClientId"]);
        c.OAuthUsePkce();
    });
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

// CORS trebuie să fie aici
app.UseCors(AppAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//try seed DB
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<GlobalDataSeeder>();
    await seeder.SeedAsync();
}

app.Run();
