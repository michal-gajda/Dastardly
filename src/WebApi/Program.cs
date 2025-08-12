using Hangfire;
using Dastardly.Application;
using Dastardly.Infrastructure;
using FastEndpoints;
using FastEndpoints.Swagger;

namespace Dastardly.WebApi;

public static class Program
{
    // C4 Level 2 - Container name (what this service does)
    private const string SERVICE_NAME = "order-api"; // More descriptive of the container's purpose
    // C4 Level 1 - System name (the overall system this belongs to)  
    private const string SERVICE_NAMESPACE = "dastardly"; // The system/product name

    private const int EXIT_SUCCESS = 0;

    public static async Task<int> Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddObservability(SERVICE_NAME, SERVICE_NAMESPACE);

        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddHttpContextAccessor();

        // Traditional ASP.NET Core Controllers (for legacy/existing APIs)
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        var app = builder.Build();

        app.UseObservability();

        // Configure FastEndpoints for External API
        app.UseFastEndpoints(c =>
        {
            c.Endpoints.RoutePrefix = "";
            c.Serializer.Options.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        });

        app.UseHangfireDashboard();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            // Add FastEndpoints Swagger support for External API documentation
            app.UseSwaggerGen();
        }

        app.UseAuthorization();

        // Keep existing controllers for backward compatibility
        app.MapControllers();

        await app.RunAsync();

        return EXIT_SUCCESS;
    }
}
