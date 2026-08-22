using Api.DTOs;
using Api.Filters;
using Api.Persistence;
using Api.Persistence.Seed;
using Api.Services;
using Api.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddValidatorsFromAssemblyContaining<CreateItineraryRequestValidator>();

builder.Services.AddScoped<PortService>();
builder.Services.AddScoped<BoatService>();
builder.Services.AddScoped<ItineraryService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddSingleton<TimeService>();
builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);
builder.Services.AddTransient<PortTimeConverter>();


var frontendOrigins =
    builder.Configuration
        .GetSection("Cors:AllowedOrigins")
        .Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(
            frontendOrigins ?? ["http://localhost:5173"])
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();
app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.StatusCode =
            StatusCodes.Status500InternalServerError;

        await context.Response.WriteAsJsonAsync(
            new ApiErrorResponse(
                [
                    new ApiError(
                        "UNEXPECTED_ERROR",
                        "Error inesperado en el servidor.")
                ]));
    });
});

using var scope = app.Services.CreateScope();

try
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await context.Database.MigrateAsync();
    await DbSeeder.SeedAsync(context);

    Console.WriteLine("Database migrated and seeded successfully.");
}
catch (Exception ex)
{
    Console.Error.WriteLine(
        $"An error occurred while preparing the database: {ex.Message}");
    throw;
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("Frontend");
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
