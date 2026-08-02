using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Pdr.Api.Data;
using Pdr.Api.Models;
using Pdr.Api.Repositories;
using Pdr.Api.Repositories.Interfaces;

using Pdr.Api.Services;
using Pdr.Api.Services.Interfaces;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using FluentValidation;
using Pdr.Api.Validators;

var builder = WebApplication.CreateBuilder(args);
 
// Add services to the container.
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));

// Dependency Injection
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IEngagementService, EngagementService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();

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
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IsAdmin", policy =>
        policy.RequireClaim("isAdmin", "true"));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
