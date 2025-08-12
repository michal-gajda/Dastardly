# Service Discovery Implementation Guide

## Overview

This document describes the comprehensive service discovery implementation added to the Dastardly application. The service discovery infrastructure enables microservices to dynamically find and communicate with each other without hardcoded endpoints.

## Architecture

### Service Discovery Components

```
src/Infrastructure/ServiceDiscovery/
├── IServiceDiscovery.cs                    # Core service discovery interface
├── ServiceInstance.cs                      # Service instance model
├── ServiceDiscoveryOptions.cs              # Configuration options
├── ServiceDiscoveryExtensions.cs           # DI registration extensions
├── ServiceDiscoveryHttpMessageHandler.cs  # HTTP client integration
├── ServiceRegistrationHostedService.cs     # Automatic registration/deregistration
├── ConsulServiceDiscovery.cs              # Consul implementation
└── KubernetesServiceDiscovery.cs          # Kubernetes implementation
```

### Service Discovery Providers

#### 1. Consul Service Discovery
- **Purpose**: Production-ready service discovery with health checking
- **Features**: 
  - Automatic service registration/deregistration
  - Health check monitoring
  - Load balancing with random selection
  - Service metadata and tagging
- **Configuration**: Requires Consul server

#### 2. Kubernetes Service Discovery
- **Purpose**: Native Kubernetes service resolution
- **Features**: 
  - DNS-based service discovery
  - Automatic Kubernetes service resolution
  - No external dependencies
- **Configuration**: Works out-of-the-box in Kubernetes

## Configuration

### appsettings.json

```json
{
  "ServiceDiscovery": {
    "Provider": "Consul",
    "Consul": {
      "Address": "http://localhost:8500",
      "Datacenter": "dc1",
      "Timeout": "00:00:10"
    },
    "Registration": {
      "ServiceName": "order-api",
      "ServiceId": "",
      "Address": "localhost",
      "Port": 5000,
      "Tags": [ "api", "orders", "dastardly", "v1" ],
      "HealthCheckEndpoint": "/health",
      "HealthCheckInterval": "00:00:10",
      "HealthCheckTimeout": "00:00:03",
      "AutoRegister": true,
      "DeregisterOnShutdown": true
    }
  }
}
```

### Provider Configuration

#### Consul Setup
```json
{
  "ServiceDiscovery": {
    "Provider": "Consul"
  }
}
```

#### Kubernetes Setup
```json
{
  "ServiceDiscovery": {
    "Provider": "Kubernetes"
  }
}
```

## Usage Examples

### 1. HTTP Client with Service Discovery

```csharp
// Register typed HTTP client with service discovery
services.AddServiceDiscoveryHttpClient<ExampleServiceClient>();

// Usage in service
public class ExampleServiceClient
{
    private readonly HttpClient _httpClient;

    public ExampleServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ExampleResponse?> GetDataAsync(string id)
    {
        // Service name 'example-service' is automatically resolved
        var response = await _httpClient.GetAsync($"http://example-service/api/data/{id}");
        // ... process response
    }
}
```

### 2. Direct Service Discovery

```csharp
public class OrderService
{
    private readonly IServiceDiscovery _serviceDiscovery;

    public OrderService(IServiceDiscovery serviceDiscovery)
    {
        _serviceDiscovery = serviceDiscovery;
    }

    public async Task<ServiceInstance?> FindPaymentService()
    {
        return await _serviceDiscovery.DiscoverServiceAsync("payment-service");
    }

    public async Task<IReadOnlyList<ServiceInstance>> FindAllInventoryServices()
    {
        return await _serviceDiscovery.DiscoverServicesAsync("inventory-service");
    }
}
```

### 3. Service Registration

```csharp
// Manual service registration
var serviceInstance = new ServiceInstance
{
    ServiceName = "order-api",
    ServiceId = "order-api-instance-1",
    Address = "localhost",
    Port = 5000,
    Tags = new[] { "api", "orders" },
    Metadata = new Dictionary<string, string>
    {
        ["version"] = "1.0.0",
        ["environment"] = "production"
    }
};

await serviceDiscovery.RegisterServiceAsync(serviceInstance);
```

## Integration with Observability

### Health Checks
Service discovery integrates with the existing observability infrastructure:

- **Health Endpoint**: `/health` (provided by `ObservabilityExtensions`)
- **Health Checks**: Used by Consul for service health monitoring
- **OpenTelemetry**: Service discovery calls are automatically traced

### Service Metadata
Automatically registered metadata includes:
- Service version (from assembly)
- Environment (from `ASPNETCORE_ENVIRONMENT`)
- Machine name
- Process ID
- Start time

## Deployment Scenarios

### Local Development
```json
{
  "ServiceDiscovery": {
    "Provider": "Consul",
    "Registration": {
      "Address": "localhost",
      "Port": 5000
    }
  }
}
```

### Docker Compose
```json
{
  "ServiceDiscovery": {
    "Provider": "Consul",
    "Registration": {
      "Address": "host.docker.internal",
      "Port": 5000
    }
  }
}
```

### Kubernetes
```json
{
  "ServiceDiscovery": {
    "Provider": "Kubernetes"
  }
}
```

## Advanced Features

### Load Balancing
- **Random Selection**: Default implementation uses random selection
- **Extensible**: Can be extended with more sophisticated algorithms

### Circuit Breaker Integration
- **HTTP Resilience**: Can be combined with `Microsoft.Extensions.Http.Resilience`
- **Fault Tolerance**: Automatic retries and circuit breaking

### Service Tags and Metadata
- **Filtering**: Services can be filtered by tags
- **Versioning**: Support for service versioning through tags
- **Environment Isolation**: Separate services by environment tags

## Monitoring and Debugging

### Logging
Service discovery operations are logged at appropriate levels:
- **Debug**: Service resolution details
- **Information**: Registration/deregistration events
- **Warning**: Service resolution failures
- **Error**: Critical service discovery failures

### Observability
- **Traces**: All service discovery operations are traced
- **Metrics**: Service resolution success/failure rates
- **Health**: Service discovery health is monitored

## Best Practices

1. **Service Naming**: Use consistent, descriptive service names
2. **Health Checks**: Implement comprehensive health checks
3. **Graceful Shutdown**: Ensure proper deregistration on shutdown
4. **Error Handling**: Handle service discovery failures gracefully
5. **Environment Isolation**: Use tags to separate environments
6. **Monitoring**: Monitor service discovery health and performance

## Security Considerations

1. **Consul Security**: Use Consul ACLs and TLS in production
2. **Network Security**: Secure service-to-service communication
3. **Service Identity**: Implement proper service authentication
4. **Metadata**: Avoid sensitive information in service metadata

## Troubleshooting

### Common Issues
1. **Service Not Found**: Check service name spelling and registration
2. **Health Check Failures**: Verify health endpoint implementation
3. **Consul Connection**: Verify Consul server availability
4. **Kubernetes DNS**: Ensure proper Kubernetes DNS configuration

### Debug Steps
1. Enable debug logging for `Dastardly.Infrastructure.ServiceDiscovery`
2. Check service registration in Consul UI
3. Verify health check endpoint responses
4. Monitor OpenTelemetry traces for service discovery calls
