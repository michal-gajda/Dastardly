namespace Dastardly.Infrastructure.ServiceDiscovery;

/// <summary>
/// Configuration options for service discovery
/// </summary>
public sealed class ServiceDiscoveryOptions
{
    public const string SectionName = "ServiceDiscovery";

    /// <summary>
    /// Type of service discovery to use (Consul, Kubernetes, etc.)
    /// </summary>
    public string Provider { get; set; } = "Consul";

    /// <summary>
    /// Consul-specific configuration
    /// </summary>
    public ConsulOptions Consul { get; set; } = new();

    /// <summary>
    /// Service registration configuration
    /// </summary>
    public ServiceRegistrationOptions Registration { get; set; } = new();
}

/// <summary>
/// Consul service discovery configuration
/// </summary>
public sealed class ConsulOptions
{
    /// <summary>
    /// Consul server address
    /// </summary>
    public string Address { get; set; } = "http://localhost:8500";

    /// <summary>
    /// Consul datacenter
    /// </summary>
    public string Datacenter { get; set; } = "dc1";

    /// <summary>
    /// Consul token for authentication
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Timeout for Consul operations
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
}

/// <summary>
/// Service registration configuration
/// </summary>
public sealed class ServiceRegistrationOptions
{
    /// <summary>
    /// Service name to register
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Service ID (unique instance identifier)
    /// </summary>
    public string ServiceId { get; set; } = string.Empty;

    /// <summary>
    /// Service address
    /// </summary>
    public string Address { get; set; } = "localhost";

    /// <summary>
    /// Service port
    /// </summary>
    public int Port { get; set; } = 5000;

    /// <summary>
    /// Service tags for metadata
    /// </summary>
    public string[] Tags { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Health check endpoint
    /// </summary>
    public string HealthCheckEndpoint { get; set; } = "/health";

    /// <summary>
    /// Health check interval
    /// </summary>
    public TimeSpan HealthCheckInterval { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Health check timeout
    /// </summary>
    public TimeSpan HealthCheckTimeout { get; set; } = TimeSpan.FromSeconds(3);

    /// <summary>
    /// Whether to register the service automatically
    /// </summary>
    public bool AutoRegister { get; set; } = true;

    /// <summary>
    /// Whether to deregister on shutdown
    /// </summary>
    public bool DeregisterOnShutdown { get; set; } = true;
}
