using System.Text.Json.Serialization;
using Pdr.Api.Extensions;
using Pdr.Api.Filters;
using Pdr.Api.Services.Audit;
using Pdr.Api.Services.Notifications;

var builder = WebApplication.CreateBuilder(args);

// Add controllers and global JSON / validation options
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Configure services via extensions
builder.Services
    .AddDatabaseServices(builder.Configuration)
    .AddSwaggerServices()
    .AddJwtAuthentication(builder.Configuration)
    .AddValidationServices()
    .AddApplicationServices()
    .AddNotificationMappers()
    .AddAuditMappers();

var app = builder.Build();

// Run database migrations and initial seed data
app.InitializeDatabase();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "PDR API v1");
    });
}

app.UseCustomMiddlewares();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
