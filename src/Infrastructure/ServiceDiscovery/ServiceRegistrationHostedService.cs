namespace Dastardly.Infrastructure.ServiceDiscovery;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// Background service that handles automatic service registration and deregistration
/// </summary>
public sealed class ServiceRegistrationHostedService : IHostedService
{
    private readonly IServiceDiscovery _serviceDiscovery;
    private readonly ServiceDiscoveryOptions _options;
    private readonly ILogger<ServiceRegistrationHostedService> _logger;
    private ServiceInstance? _registeredService;

    public ServiceRegistrationHostedService(
        IServiceDiscovery serviceDiscovery,
        IOptions<ServiceDiscoveryOptions> options,
        ILogger<ServiceRegistrationHostedService> logger)
    {
        _serviceDiscovery = serviceDiscovery;
        _options = options.Value;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_options.Registration.AutoRegister)
        {
            _logger.LogInformation("Auto-registration is disabled, skipping service registration");
            return;
        }

        try
        {
            _registeredService = CreateServiceInstance();
            await _serviceDiscovery.RegisterServiceAsync(_registeredService, cancellationToken);

            _logger.LogInformation("Service registration completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register service during startup");
            // Don't throw - let the application start even if service discovery fails
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_registeredService == null || !_options.Registration.DeregisterOnShutdown)
        {
            return;
        }

        try
        {
            await _serviceDiscovery.DeregisterServiceAsync(_registeredService.ServiceId, cancellationToken);

            _logger.LogInformation("Service deregistration completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deregister service during shutdown");
            // Don't throw during shutdown
        }
    }

    private ServiceInstance CreateServiceInstance()
    {
        var registration = _options.Registration;

        // Generate unique service ID if not provided
        var serviceId = string.IsNullOrEmpty(registration.ServiceId)
            ? $"{registration.ServiceName}-{Environment.MachineName}-{Environment.ProcessId}"
            : registration.ServiceId;

        return new ServiceInstance
        {
            ServiceName = registration.ServiceName,
            ServiceId = serviceId,
            Address = registration.Address,
            Port = registration.Port,
            Tags = registration.Tags,
            Metadata = new Dictionary<string, string>
            {
                ["version"] = GetAssemblyVersion(),
                ["environment"] = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                ["machine"] = Environment.MachineName,
                ["processId"] = Environment.ProcessId.ToString(),
                ["startTime"] = DateTimeOffset.UtcNow.ToString("O")
            }
        };
    }

    private static string GetAssemblyVersion()
    {
        var assembly = System.Reflection.Assembly.GetEntryAssembly();
        return assembly?.GetName().Version?.ToString() ?? "1.0.0";
    }
}
