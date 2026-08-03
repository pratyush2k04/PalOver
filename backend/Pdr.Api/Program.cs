using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
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
//builder.Services.AddSwaggerGen();

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

        document.Components.SecuritySchemes ??=
            new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes["Bearer"] =
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Enter your JWT token"
            };

        document.Security ??= [];

        document.Security.Add(
            new OpenApiSecurityRequirement
            {
                [
                    new OpenApiSecuritySchemeReference(
                        "Bearer",
                        document)
                ] = []
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
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IChecklistRepository, ChecklistRepository>();
builder.Services.AddScoped<IChecklistService, ChecklistService>();
builder.Services.AddScoped<Pdr.Api.Services.Interfaces.IHandoverAccessService, Pdr.Api.Services.HandoverAccessService>();
builder.Services.AddScoped<Pdr.Api.Services.Interfaces.IHandoverService, Pdr.Api.Services.HandoverService>();
builder.Services.AddScoped<Pdr.Api.Services.Interfaces.ICommitmentService, Pdr.Api.Services.CommitmentService>();

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
        ClockSkew = TimeSpan.Zero
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
    db.Database.Migrate();

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
