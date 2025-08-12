namespace Dastardly.Infrastructure.ServiceDiscovery;

using Microsoft.Extensions.Logging;

/// <summary>
/// HTTP message handler that integrates with service discovery for outbound HTTP calls
/// </summary>
public sealed class ServiceDiscoveryHttpMessageHandler : DelegatingHandler
{
    private readonly IServiceDiscovery _serviceDiscovery;
    private readonly ILogger<ServiceDiscoveryHttpMessageHandler> _logger;

    public ServiceDiscoveryHttpMessageHandler(
        IServiceDiscovery serviceDiscovery,
        ILogger<ServiceDiscoveryHttpMessageHandler> logger)
    {
        _serviceDiscovery = serviceDiscovery;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Check if the request URI uses a service name instead of a concrete URL
        if (request.RequestUri != null && IsServiceDiscoveryUri(request.RequestUri))
        {
            var serviceName = ExtractServiceName(request.RequestUri);
            var serviceInstance = await _serviceDiscovery.DiscoverServiceAsync(serviceName, cancellationToken);

            if (serviceInstance != null)
            {
                // Replace the service name with the actual service URL
                var newUri = ReplaceServiceNameWithUrl(request.RequestUri, serviceInstance);
                request.RequestUri = newUri;

                _logger.LogDebug("Resolved service {ServiceName} to {ServiceUrl}", serviceName, serviceInstance.GetServiceUrl());
            }
            else
            {
                _logger.LogWarning("Could not resolve service {ServiceName} through service discovery", serviceName);
                throw new InvalidOperationException($"Service '{serviceName}' could not be resolved through service discovery");
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private static bool IsServiceDiscoveryUri(Uri uri)
    {
        // Check if the scheme is 'service' or if the host matches a service naming pattern
        return uri.Scheme == "service" ||
               (uri.Scheme == "http" && IsServiceName(uri.Host));
    }

    private static bool IsServiceName(string host)
    {
        // Simple heuristic: if the host doesn't contain dots and doesn't look like an IP,
        // treat it as a service name
        return !host.Contains('.') && !System.Net.IPAddress.TryParse(host, out _);
    }

    private static string ExtractServiceName(Uri uri)
    {
        return uri.Scheme == "service" ? uri.Host : uri.Host;
    }

    private static Uri ReplaceServiceNameWithUrl(Uri originalUri, ServiceInstance serviceInstance)
    {
        var uriBuilder = new UriBuilder(originalUri)
        {
            Scheme = "http", // Could be configurable
            Host = serviceInstance.Address,
            Port = serviceInstance.Port
        };

        return uriBuilder.Uri;
    }
}
