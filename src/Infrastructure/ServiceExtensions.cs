namespace Dastardly.Infrastructure;

using System.Reflection;
using Dastardly.Application.Interfaces;
using Dastardly.Infrastructure.Clients;
using Dastardly.Infrastructure.Messaging;
using Dastardly.Infrastructure.ServiceDiscovery;
using FastEndpoints;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // Register Hangfire for background processing
        services.AddHangfire(config => config.UseInMemoryStorage());
        services.AddHangfireServer();

        // Register background command dispatcher
        services.AddScoped<IBackgroundCommandDispatcher, BackgroundCommandDispatcher>();

        // Register FastEndpoints for External API
        services.AddFastEndpoints();

        // Register Service Discovery
        services.AddServiceDiscovery(configuration);
        services.AddServiceDiscoveryHttpMessageHandler();

        // Register HTTP clients with service discovery
        services.AddServiceDiscoveryHttpClient<ExampleServiceClient>();
    }
}
