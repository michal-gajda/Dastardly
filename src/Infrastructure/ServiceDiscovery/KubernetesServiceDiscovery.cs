namespace Dastardly.Infrastructure.ServiceDiscovery;

using Microsoft.Extensions.Logging;
using System.Net;

/// <summary>
/// Kubernetes-based service discovery implementation
/// Uses Kubernetes DNS for service resolution
/// </summary>
public sealed class KubernetesServiceDiscovery : IServiceDiscovery
{
    private readonly ILogger<KubernetesServiceDiscovery> _logger;
    private readonly string _namespace;

    public KubernetesServiceDiscovery(ILogger<KubernetesServiceDiscovery> logger)
    {
        _logger = logger;
        _namespace = Environment.GetEnvironmentVariable("KUBERNETES_NAMESPACE") ?? "default";
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ServiceInstance>> DiscoverServicesAsync(string serviceName, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Discovering Kubernetes service {ServiceName} in namespace {Namespace}", serviceName, _namespace);

            var hostname = $"{serviceName}.{_namespace}.svc.cluster.local";
            var addresses = await Dns.GetHostAddressesAsync(hostname, cancellationToken);

            var instances = new List<ServiceInstance>();

            foreach (var address in addresses)
            {
                instances.Add(new ServiceInstance
                {
                    ServiceName = serviceName,
                    ServiceId = $"{serviceName}-{address}",
                    Address = address.ToString(),
                    Port = 80, // Default HTTP port - could be configurable
                    Tags = new[] { "kubernetes", _namespace },
                    Metadata = new Dictionary<string, string>
                    {
                        ["namespace"] = _namespace,
                        ["provider"] = "kubernetes"
                    }
                });
            }

            _logger.LogDebug("Discovered {Count} instances for Kubernetes service {ServiceName}", instances.Count, serviceName);
            return instances.AsReadOnly();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to discover Kubernetes service {ServiceName}", serviceName);
            return Array.Empty<ServiceInstance>();
        }
    }

    /// <inheritdoc />
    public async Task<ServiceInstance?> DiscoverServiceAsync(string serviceName, CancellationToken cancellationToken = default)
    {
        var instances = await DiscoverServicesAsync(serviceName, cancellationToken);
        return instances.FirstOrDefault();
    }

    /// <inheritdoc />
    public Task RegisterServiceAsync(ServiceInstance serviceInstance, CancellationToken cancellationToken = default)
    {
        // In Kubernetes, service registration is handled by Kubernetes itself
        // Services are registered via Service and Endpoint resources
        _logger.LogInformation("Service registration is handled by Kubernetes for service {ServiceName}", serviceInstance.ServiceName);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeregisterServiceAsync(string serviceId, CancellationToken cancellationToken = default)
    {
        // In Kubernetes, service deregistration is handled by Kubernetes itself
        _logger.LogInformation("Service deregistration is handled by Kubernetes for service {ServiceId}", serviceId);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<bool> IsServiceHealthyAsync(string serviceId, CancellationToken cancellationToken = default)
    {
        // In Kubernetes, health is managed by readiness/liveness probes
        // For simplicity, we'll assume the service is healthy if it can be resolved
        try
        {
            var serviceName = ExtractServiceNameFromId(serviceId);
            var instances = await DiscoverServicesAsync(serviceName, cancellationToken);
            return instances.Any();
        }
        catch
        {
            return false;
        }
    }

    private static string ExtractServiceNameFromId(string serviceId)
    {
        // Extract service name from service ID (assumes format: servicename-address)
        var parts = serviceId.Split('-');
        return parts.Length > 0 ? parts[0] : serviceId;
    }
}
