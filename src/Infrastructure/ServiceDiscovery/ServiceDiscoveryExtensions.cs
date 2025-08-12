namespace Dastardly.Infrastructure.ServiceDiscovery;

using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

/// <summary>
/// Extension methods for configuring service discovery
/// </summary>
public static class ServiceDiscoveryExtensions
{
    /// <summary>
    /// Adds service discovery services to the dependency injection container
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddServiceDiscovery(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure options
        services.Configure<ServiceDiscoveryOptions>(configuration.GetSection(ServiceDiscoveryOptions.SectionName));

        // Get the provider configuration
        var serviceDiscoverySection = configuration.GetSection(ServiceDiscoveryOptions.SectionName);
        var provider = serviceDiscoverySection.GetValue<string>("Provider") ?? "Consul";

        switch (provider.ToLowerInvariant())
        {
            case "consul":
                AddConsulServiceDiscovery(services);
                break;
            case "kubernetes":
            case "k8s":
                AddKubernetesServiceDiscovery(services);
                break;
            default:
                throw new InvalidOperationException($"Unsupported service discovery provider: {provider}");
        }

        // Add service registration hosted service (only for providers that support it)
        if (provider.ToLowerInvariant() == "consul")
        {
            services.AddHostedService<ServiceRegistrationHostedService>();
        }

        return services;
    }

    private static void AddConsulServiceDiscovery(IServiceCollection services)
    {
        // Add Consul client
        services.AddSingleton<IConsulClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<ServiceDiscoveryOptions>>().Value;
            var consulConfig = new ConsulClientConfiguration
            {
                Address = new Uri(options.Consul.Address),
                Datacenter = options.Consul.Datacenter,
                Token = options.Consul.Token
            };

            return new ConsulClient(consulConfig);
        });

        // Add Consul service discovery implementation
        services.AddSingleton<IServiceDiscovery, ConsulServiceDiscovery>();
    }

    private static void AddKubernetesServiceDiscovery(IServiceCollection services)
    {
        // Add Kubernetes service discovery implementation
        services.AddSingleton<IServiceDiscovery, KubernetesServiceDiscovery>();
    }

    /// <summary>
    /// Adds HTTP client with service discovery support
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="name">HTTP client name</param>
    /// <returns>HTTP client builder for further configuration</returns>
    public static IHttpClientBuilder AddServiceDiscoveryHttpClient(this IServiceCollection services, string name)
    {
        return services.AddHttpClient(name)
            .AddHttpMessageHandler<ServiceDiscoveryHttpMessageHandler>();
    }

    /// <summary>
    /// Adds typed HTTP client with service discovery support
    /// </summary>
    /// <typeparam name="TClient">HTTP client type</typeparam>
    /// <param name="services">Service collection</param>
    /// <returns>HTTP client builder for further configuration</returns>
    public static IHttpClientBuilder AddServiceDiscoveryHttpClient<TClient>(this IServiceCollection services)
        where TClient : class
    {
        return services.AddHttpClient<TClient>()
            .AddHttpMessageHandler<ServiceDiscoveryHttpMessageHandler>();
    }

    /// <summary>
    /// Adds the service discovery HTTP message handler
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddServiceDiscoveryHttpMessageHandler(this IServiceCollection services)
    {
        services.AddTransient<ServiceDiscoveryHttpMessageHandler>();
        return services;
    }

    /// <summary>
    /// Configures service registration options
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configureOptions">Configuration action</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection ConfigureServiceRegistration(
        this IServiceCollection services,
        Action<ServiceRegistrationOptions> configureOptions)
    {
        services.Configure<ServiceDiscoveryOptions>(options => configureOptions(options.Registration));
        return services;
    }
}
