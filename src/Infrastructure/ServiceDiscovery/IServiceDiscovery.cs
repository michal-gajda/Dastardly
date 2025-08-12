namespace Dastardly.Infrastructure.ServiceDiscovery;

/// <summary>
/// Service discovery interface for finding and managing service instances
/// </summary>
public interface IServiceDiscovery
{
    /// <summary>
    /// Discovers all healthy instances of a service
    /// </summary>
    /// <param name="serviceName">Name of the service to discover</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of healthy service instances</returns>
    Task<IReadOnlyList<ServiceInstance>> DiscoverServicesAsync(string serviceName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Discovers a single healthy instance of a service (load balanced)
    /// </summary>
    /// <param name="serviceName">Name of the service to discover</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A single service instance or null if none available</returns>
    Task<ServiceInstance?> DiscoverServiceAsync(string serviceName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a service instance
    /// </summary>
    /// <param name="serviceInstance">Service instance to register</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RegisterServiceAsync(ServiceInstance serviceInstance, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deregisters a service instance
    /// </summary>
    /// <param name="serviceId">Service instance ID to deregister</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeregisterServiceAsync(string serviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks the health of a service instance
    /// </summary>
    /// <param name="serviceId">Service instance ID to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if healthy, false otherwise</returns>
    Task<bool> IsServiceHealthyAsync(string serviceId, CancellationToken cancellationToken = default);
}
