namespace Dastardly.Infrastructure.ServiceDiscovery;

using Consul;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// Consul-based service discovery implementation
/// </summary>
public sealed class ConsulServiceDiscovery : IServiceDiscovery, IDisposable
{
    private readonly IConsulClient _consulClient;
    private readonly ServiceDiscoveryOptions _options;
    private readonly ILogger<ConsulServiceDiscovery> _logger;
    private readonly Random _random = new();

    public ConsulServiceDiscovery(
        IConsulClient consulClient,
        IOptions<ServiceDiscoveryOptions> options,
        ILogger<ConsulServiceDiscovery> logger)
    {
        _consulClient = consulClient;
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ServiceInstance>> DiscoverServicesAsync(string serviceName, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Discovering services for {ServiceName}", serviceName);

            var queryResult = await _consulClient.Health.Service(serviceName, string.Empty, true, cancellationToken);

            var instances = queryResult.Response.Select(ConvertToServiceInstance).ToList();

            _logger.LogDebug("Discovered {Count} healthy instances for service {ServiceName}", instances.Count, serviceName);

            return instances.AsReadOnly();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to discover services for {ServiceName}", serviceName);
            return Array.Empty<ServiceInstance>();
        }
    }

    /// <inheritdoc />
    public async Task<ServiceInstance?> DiscoverServiceAsync(string serviceName, CancellationToken cancellationToken = default)
    {
        var instances = await DiscoverServicesAsync(serviceName, cancellationToken);

        if (!instances.Any())
        {
            _logger.LogWarning("No healthy instances found for service {ServiceName}", serviceName);
            return null;
        }

        // Simple random load balancing
        var selectedInstance = instances[_random.Next(instances.Count)];

        _logger.LogDebug("Selected instance {ServiceId} for service {ServiceName}",
            selectedInstance.ServiceId, serviceName);

        return selectedInstance;
    }

    /// <inheritdoc />
    public async Task RegisterServiceAsync(ServiceInstance serviceInstance, CancellationToken cancellationToken = default)
    {
        try
        {
            var registration = new AgentServiceRegistration
            {
                ID = serviceInstance.ServiceId,
                Name = serviceInstance.ServiceName,
                Address = serviceInstance.Address,
                Port = serviceInstance.Port,
                Tags = serviceInstance.Tags.ToArray(),
                Meta = serviceInstance.Metadata.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                Check = new AgentServiceCheck
                {
                    HTTP = $"{serviceInstance.GetServiceUrl()}{_options.Registration.HealthCheckEndpoint}",
                    Interval = _options.Registration.HealthCheckInterval,
                    Timeout = _options.Registration.HealthCheckTimeout,
                    DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1)
                }
            };

            await _consulClient.Agent.ServiceRegister(registration, cancellationToken);

            _logger.LogInformation("Successfully registered service {ServiceName} with ID {ServiceId} at {Address}:{Port}",
                serviceInstance.ServiceName, serviceInstance.ServiceId, serviceInstance.Address, serviceInstance.Port);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register service {ServiceName} with ID {ServiceId}",
                serviceInstance.ServiceName, serviceInstance.ServiceId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task DeregisterServiceAsync(string serviceId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _consulClient.Agent.ServiceDeregister(serviceId, cancellationToken);

            _logger.LogInformation("Successfully deregistered service with ID {ServiceId}", serviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deregister service with ID {ServiceId}", serviceId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> IsServiceHealthyAsync(string serviceId, CancellationToken cancellationToken = default)
    {
        try
        {
            var healthResult = await _consulClient.Agent.ServiceDeregister(serviceId, cancellationToken);
            return true; // If no exception, service exists and is registered
        }
        catch
        {
            return false;
        }
    }

    private static ServiceInstance ConvertToServiceInstance(ServiceEntry consulService)
    {
        return new ServiceInstance
        {
            ServiceName = consulService.Service.Service,
            ServiceId = consulService.Service.ID,
            Address = consulService.Service.Address,
            Port = consulService.Service.Port,
            Tags = consulService.Service.Tags?.ToList().AsReadOnly() ?? (IReadOnlyList<string>)Array.Empty<string>(),
            Metadata = consulService.Service.Meta?.AsReadOnly() ?? (IReadOnlyDictionary<string, string>)new Dictionary<string, string>(),
            IsHealthy = consulService.Checks?.All(check => check.Status == HealthStatus.Passing) ?? true
        };
    }

    public void Dispose()
    {
        _consulClient?.Dispose();
    }
}
