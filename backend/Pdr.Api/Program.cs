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

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new()
        {
            Title = "PDR API",
            Version = "v1"
        };

        document.Components ??= new();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        // ApiKey scheme allows users to enter "Bearer {token}" or just "{token}"
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Description = "Enter 'Bearer ' followed by your JWT token (e.g. 'Bearer eyJhbGci...')"
        };

        document.Security ??= [];
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer")] = []
        });

        return Task.CompletedTask;
    });
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
builder.Services.AddScoped<Pdr.Api.Services.Interfaces.IHandoverAccessService, Pdr.Api.Services.HandoverAccessService>();
builder.Services.AddScoped<Pdr.Api.Services.Interfaces.IHandoverService, Pdr.Api.Services.HandoverService>();
builder.Services.AddScoped<Pdr.Api.Services.Interfaces.ICommitmentService, Pdr.Api.Services.CommitmentService>();
builder.Services.AddScoped<IMasterChecklistRepository, MasterChecklistRepository>();
builder.Services.AddScoped<IMasterChecklistService, MasterChecklistService>();
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<IEngagementRepository, EngagementRepository>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IEngagementChecklistRepository, EngagementChecklistRepository>();
builder.Services.AddScoped<IEngagementChecklistService, EngagementChecklistService>();

// Technical Readiness — Access Tracker & Environment Tooling
builder.Services.AddScoped<Pdr.Api.Repositories.Interfaces.IAccessTrackerRepository, Pdr.Api.Repositories.AccessTrackerRepository>();
builder.Services.AddScoped<Pdr.Api.Services.Interfaces.IAccessTrackerService, Pdr.Api.Services.AccessTrackerService>();
builder.Services.AddScoped<Pdr.Api.Services.Interfaces.IRiskService, Pdr.Api.Services.RiskService>();
builder.Services.AddScoped<Pdr.Api.Repositories.Interfaces.IEnvironmentToolingRepository, Pdr.Api.Repositories.EnvironmentToolingRepository>();
builder.Services.AddScoped<Pdr.Api.Services.Interfaces.IEnvironmentToolingService, Pdr.Api.Services.EnvironmentToolingService>();


// Register FluentValidation Validators
builder.Services.AddValidatorsFromAssemblyContaining<DocumentUploadDtoValidator>();

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
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "PDR API v1");
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
