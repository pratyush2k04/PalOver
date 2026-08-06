using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using Pdr.Api.Data;
using Pdr.Api.Models;
using Pdr.Api.Middleware;
using Pdr.Api.Repositories;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services;
using Pdr.Api.Services.Interfaces;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Pdr.Api.Validators;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PDR API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });

    c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        [
            new OpenApiSecuritySchemeReference("Bearer", doc)
        ] = new List<string>()
    });
});

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));

// Register FluentValidation Validators
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestDtoValidator>();

// Dependency Injection
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IEngagementService, EngagementService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IHandoverAccessService, HandoverAccessService>();
builder.Services.AddScoped<IHandoverService, HandoverService>();
builder.Services.AddScoped<ICommitmentService, CommitmentService>();
builder.Services.AddScoped<IMasterChecklistRepository, MasterChecklistRepository>();
builder.Services.AddScoped<IMasterChecklistService, MasterChecklistService>();
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<IEngagementRepository, EngagementRepository>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IKickoffRepository, KickoffRepository>();
builder.Services.AddScoped<IKickoffService, KickoffService>();
builder.Services.AddScoped<IEngagementChecklistRepository, EngagementChecklistRepository>();
builder.Services.AddScoped<IEngagementChecklistService, EngagementChecklistService>();

// Technical Readiness — Access Tracker, Environment Tooling
builder.Services.AddScoped<IAccessTrackerRepository, AccessTrackerRepository>();
builder.Services.AddScoped<IAccessTrackerService, AccessTrackerService>();
builder.Services.AddScoped<IEnvironmentToolingRepository, EnvironmentToolingRepository>();
builder.Services.AddScoped<IEnvironmentToolingService, EnvironmentToolingService>();

// JWT Authentication
var secretKey = builder.Configuration["JwtSettings:SecretKey"]
    ?? throw new InvalidOperationException("JwtSettings:SecretKey is not configured.");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        RoleClaimType = System.Security.Claims.ClaimTypes.Role
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("isAdmin", "true"));
});

var app = builder.Build();

// Automatically apply any pending EF Core migrations on application startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Warning] EF Migration skipped or encountered existing schema: {ex.Message}");
    }

    if (!db.EngagementTypes.Any())
    {
        db.EngagementTypes.AddRange(
            Enum.GetValues<EngagementTypeEnum>()
                .Select(t => new EngagementType { Name = t.ToString() }));
        db.SaveChanges();
    }

    if (!db.EngagementStatuses.Any())
    {
        db.EngagementStatuses.AddRange(
            Enum.GetValues<EngagementStatusEnum>()
                .Select(s => new EngagementStatus { StatusName = s.ToString() }));
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "PDR API v1");
    });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseDefaultFiles();
app.UseStaticFiles();

// Prevent browser back button caching for authenticated API responses
app.Use(async (context, next) =>
{
    context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
    context.Response.Headers["Pragma"] = "no-cache";
    context.Response.Headers["Expires"] = "0";

    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();
