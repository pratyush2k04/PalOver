using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Apply migrations (and seed once) on startup. Convenient for a starter / containers;
// swap for an explicit migration step in the pipeline as the app matures.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.Notes.Any())
    {
        db.Notes.Add(new Note { Title = "Welcome to PDR", Content = "This note was seeded on first run." });
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

app.MapControllers();

// Any non-API route falls back to the Angular shell so the client-side router can handle it.
app.MapFallbackToFile("index.html");

app.Run();
