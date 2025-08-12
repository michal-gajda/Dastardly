namespace Dastardly.Infrastructure.ServiceDiscovery;

/// <summary>
/// Represents a service instance discovered through service discovery
/// </summary>
public sealed record ServiceInstance
{
    /// <summary>
    /// Service name
    /// </summary>
    public required string ServiceName { get; init; }

    /// <summary>
    /// Unique service instance ID
    /// </summary>
    public required string ServiceId { get; init; }

    /// <summary>
    /// Service address/hostname
    /// </summary>
    public required string Address { get; init; }

    /// <summary>
    /// Service port
    /// </summary>
    public required int Port { get; init; }

    /// <summary>
    /// Service tags for metadata
    /// </summary>
    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Service metadata key-value pairs
    /// </summary>
    public IReadOnlyDictionary<string, string> Metadata { get; init; } = new Dictionary<string, string>();

    /// <summary>
    /// Whether the service instance is healthy
    /// </summary>
    public bool IsHealthy { get; init; } = true;

    /// <summary>
    /// Gets the full service URL
    /// </summary>
    public string GetServiceUrl(string scheme = "http") => $"{scheme}://{Address}:{Port}";

    /// <summary>
    /// Checks if the service has a specific tag
    /// </summary>
    public bool HasTag(string tag) => Tags.Contains(tag, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets metadata value by key
    /// </summary>
    public string? GetMetadata(string key) => Metadata.TryGetValue(key, out var value) ? value : null;
}
