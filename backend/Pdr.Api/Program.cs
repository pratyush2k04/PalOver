using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models;
using Pdr.Api.Services;
using Pdr.Api.Services.Interfaces;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddScoped<IEngagementService, EngagementService>();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

// Apply migrations (and seed once) on startup. Convenient for a starter / containers;
// swap for an explicit migration step in the pipeline as the app matures.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();


    if (!db.EngagementStatuses.Any(s => s.StatusName == "Kickoff Pending"))
    {
        db.EngagementStatuses.Add(new EngagementStatus { StatusName = "Kickoff Pending" });
        db.SaveChanges();
    }

    if (!db.EngagementTypes.Any())
    {
        db.EngagementTypes.AddRange(
            new EngagementType { Name = "Greenfield" },
            new EngagementType { Name = "Brownfield" });
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Serve the built Angular app (populated into wwwroot at publish/docker build time).
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Any non-API route falls back to the Angular shell so the client-side router can handle it.
app.MapFallbackToFile("index.html");

app.Run();
