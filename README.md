# Dastardly - .NET 9 Microservices Application

A comprehensive .NET 9 application implementing **Clean Architecture**, **Domain-Driven Design**, **CQRS**, **OpenTelemetry Observability**, and **Service Discovery** patterns.

## 🏗️ Architecture Overview

This application follows the **C4 Model** and implements a clean, layered architecture:

```
┌─ System: Dastardly E-commerce Platform ─────────────────────────┐
│                                                                  │
│  ┌─ Container: Order API ──────────────────────────────────────┐ │
│  │                                                              │ │
│  │  ┌─ Component: Web API ─────────────┐                       │ │
│  │  │ • FastEndpoints (External API)   │                       │ │
│  │  │ • Traditional Controllers        │                       │ │
│  │  │ • OpenTelemetry Observability   │                       │ │
│  │  │ • Service Discovery             │                       │ │
│  │  └──────────────────────────────────┘                       │ │
│  │                                                              │ │
│  │  ┌─ Component: Application Layer ──┐                       │ │
│  │  │ • CQRS Commands/Queries         │                       │ │
│  │  │ • MediatR Handlers              │                       │ │
│  │  │ • Application Services          │                       │ │
│  │  └──────────────────────────────────┘                       │ │
│  │                                                              │ │
│  │  ┌─ Component: Infrastructure ─────┐                       │ │
│  │  │ • Hangfire Background Jobs      │                       │ │
│  │  │ • Service Discovery (Consul/K8s)│                       │ │
│  │  │ • HTTP Clients                  │                       │ │
│  │  │ • API Contract Mappers          │                       │ │
│  │  └──────────────────────────────────┘                       │ │
│  │                                                              │ │
│  │  ┌─ Component: Domain Layer ───────┐                       │ │
│  │  │ • Order Entity                  │                       │ │
│  │  │ • Value Objects                 │                       │ │
│  │  │ • Business Rules                │                       │ │
│  │  └──────────────────────────────────┘                       │ │
│  └──────────────────────────────────────────────────────────────┘ │
└──────────────────────────────────────────────────────────────────┘
```

## 🚀 Key Features

### ✅ Clean Architecture & DDD
- **Domain Layer**: Pure business logic with Order entities and value objects
- **Application Layer**: CQRS commands/queries with MediatR
- **Infrastructure Layer**: External concerns (databases, APIs, messaging)
- **Presentation Layer**: FastEndpoints and traditional ASP.NET Core controllers

### ✅ API Contract Management
- **FastEndpoints**: High-performance API endpoints with automatic validation
- **API Contract Mapper**: Translation layer between external contracts and domain models
- **FluentValidation**: Comprehensive request validation
- **Swagger Documentation**: Auto-generated API documentation

### ✅ Observability (OpenTelemetry)
- **Distributed Tracing**: End-to-end request tracing across services
- **Metrics Collection**: Application and system metrics
- **Structured Logging**: Correlated logs with trace context
- **Health Checks**: Service health monitoring at `/health`
- **OTLP Export**: Compatible with Jaeger, Zipkin, Prometheus, Grafana

### ✅ Service Discovery
- **Multi-Provider Support**: Consul and Kubernetes implementations
- **Automatic Registration**: Services self-register on startup
- **Health Monitoring**: Integrated health checks for service instances
- **HTTP Client Integration**: Seamless service-to-service communication
- **Load Balancing**: Random selection with extensible algorithms

### ✅ Background Processing
- **Hangfire**: Reliable background job processing
- **Command Dispatching**: Async command execution
- **Retry Logic**: Automatic retry for failed operations

## 🏗️ Project Structure

```
Dastardly/
├── src/
│   ├── Domain/                     # Pure business logic
│   │   ├── Entities/
│   │   │   └── Order.cs           # Order aggregate root
│   │   └── ValueObjects/
│   │       └── OrderItem.cs       # Order item value object
│   │
│   ├── Application/                # Use cases and application services
│   │   ├── Commands/
│   │   │   ├── CreateOrderCommand.cs
│   │   │   └── CreateOrderHandler.cs
│   │   └── Interfaces/
│   │       └── IBackgroundCommandDispatcher.cs
│   │
│   ├── Infrastructure/             # External concerns
│   │   ├── ServiceDiscovery/       # Service discovery implementations
│   │   │   ├── IServiceDiscovery.cs
│   │   │   ├── ConsulServiceDiscovery.cs
│   │   │   ├── KubernetesServiceDiscovery.cs
│   │   │   └── ServiceDiscoveryExtensions.cs
│   │   ├── Endpoints/             # FastEndpoints API
│   │   │   ├── CreateOrderEndpoint.cs
│   │   │   └── GetOrderEndpoint.cs
│   │   ├── Clients/               # HTTP service clients
│   │   │   └── ExampleServiceClient.cs
│   │   ├── Mappers/               # API contract translation
│   │   │   └── ApiContractMapper.cs
│   │   ├── Contracts/             # External API contracts
│   │   ├── Validators/            # Request validation
│   │   └── Messaging/             # Background processing
│   │
│   └── WebApi/                    # HTTP interface
│       ├── Program.cs             # Application entry point
│       ├── ObservabilityExtensions.cs # OpenTelemetry setup
│       └── Controllers/           # Traditional ASP.NET Core controllers
│
├── docs/                          # Documentation
├── Dastardly.slnx                # Solution file
└── README.md                      # This file
```

## 🛠️ Technology Stack

### Core Framework
- **.NET 9**: Latest .NET framework with native AOT support
- **ASP.NET Core**: Web framework with minimal APIs and controllers
- **C# 13**: Latest language features

### Architecture Patterns
- **MediatR 13.0**: CQRS implementation
- **FastEndpoints 5.30**: High-performance API endpoints
- **FluentValidation 11.11**: Request validation

### Observability
- **OpenTelemetry 1.12**: Distributed tracing, metrics, and logging
- **Health Checks**: Service health monitoring
- **OTLP Exporter**: Export to observability platforms

### Service Discovery
- **Consul 1.7.14**: Production service discovery
- **Microsoft.Extensions.ServiceDiscovery 9.1**: .NET service discovery abstractions
- **Kubernetes**: Native DNS-based service discovery

### Background Processing
- **Hangfire 1.8**: Background job processing
- **In-Memory Storage**: Development storage (configurable for production)

## 🚀 Getting Started

### Prerequisites
- .NET 9 SDK
- (Optional) Consul server for service discovery
- (Optional) Docker for containerization

### Run Locally

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd Dastardly
   ```

2. **Build the solution**
   ```bash
   dotnet build
   ```

3. **Run the application**
   ```bash
   cd src/WebApi
   dotnet run
   ```

4. **Access the application**
   - Web API: `https://localhost:5001`
   - Swagger UI: `https://localhost:5001/swagger`
   - Hangfire Dashboard: `https://localhost:5001/hangfire`
   - Health Checks: `https://localhost:5001/health`

### Configuration

#### Service Discovery (appsettings.json)
```json
{
  "ServiceDiscovery": {
    "Provider": "Consul",
    "Consul": {
      "Address": "http://localhost:8500"
    },
    "Registration": {
      "ServiceName": "order-api",
      "Address": "localhost",
      "Port": 5000,
      "Tags": ["api", "orders", "v1"]
    }
  }
}
```

#### Environment Variables
- `ASPNETCORE_ENVIRONMENT`: Application environment (Development, Production)
- `OTEL_EXPORTER_OTLP_ENDPOINT`: OpenTelemetry collector endpoint
- `KUBERNETES_NAMESPACE`: Kubernetes namespace (for K8s service discovery)

## 📋 API Endpoints

### FastEndpoints (External API)
- `POST /api/orders` - Create a new order
- `GET /api/orders/{id}` - Get order by ID

### Traditional Controllers
- `GET /weatherforecast` - Sample weather data

### System Endpoints
- `GET /health` - Health check endpoint
- `GET /hangfire` - Hangfire dashboard

## 🔍 Observability

### OpenTelemetry Features
- **Automatic Instrumentation**: ASP.NET Core and HttpClient
- **Custom Tracing**: Application-specific traces
- **Metrics**: Request/response metrics
- **Logging**: Structured logging with trace correlation

### Health Checks
- **Self Check**: Basic application health
- **Service Discovery**: Service registration health
- **External Dependencies**: Database, external APIs (configurable)

### Monitoring Stack (Recommended)
- **Jaeger**: Distributed tracing
- **Prometheus**: Metrics collection
- **Grafana**: Dashboards and visualization
- **AlertManager**: Alerting rules

## 🌐 Service Discovery

### Providers

#### Consul
- **Production Ready**: Proven service discovery solution
- **Health Checks**: Automatic health monitoring
- **Service Mesh**: Can integrate with Consul Connect
- **Web UI**: Built-in service registry browser

#### Kubernetes
- **Cloud Native**: Native Kubernetes integration
- **DNS-Based**: Uses Kubernetes DNS for resolution
- **No Dependencies**: Works out-of-the-box in K8s

### HTTP Client Integration
```csharp
// Automatic service resolution
services.AddServiceDiscoveryHttpClient<PaymentServiceClient>();

// Usage
await httpClient.GetAsync("http://payment-service/api/process");
// Resolves 'payment-service' automatically
```

## 🐳 Docker & Kubernetes

### Docker
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0
COPY . /app
WORKDIR /app
EXPOSE 80
ENTRYPOINT ["dotnet", "Dastardly.WebApi.dll"]
```

### Kubernetes
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: order-api
spec:
  replicas: 3
  selector:
    matchLabels:
      app: order-api
  template:
    metadata:
      labels:
        app: order-api
    spec:
      containers:
      - name: order-api
        image: dastardly/order-api:latest
        ports:
        - containerPort: 80
        env:
        - name: ServiceDiscovery__Provider
          value: "Kubernetes"
```

## 🧪 Testing

### Unit Tests
```bash
dotnet test
```

### Integration Tests
```bash
dotnet test --filter Category=Integration
```

### API Testing
- **Swagger UI**: Interactive API testing
- **HTTP Files**: `.http` files for API testing
- **Postman Collection**: Available in `/docs`

## 📚 Documentation

- [Service Discovery Guide](Service-Discovery-Documentation.md)
- [API Contract Documentation](Terminology-Update-Summary.md)
- [Architecture Analysis](C4-Architecture-Analysis.md)
- [Infrastructure Guide](src/Infrastructure/README.md)

## 🤝 Contributing

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/amazing-feature`)
3. **Commit** your changes (`git commit -m 'Add amazing feature'`)
4. **Push** to the branch (`git push origin feature/amazing-feature`)
5. **Open** a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🎯 Roadmap

### Completed ✅
- [x] Clean Architecture implementation
- [x] OpenTelemetry observability
- [x] FastEndpoints API
- [x] Service Discovery (Consul & Kubernetes)
- [x] Background processing with Hangfire
- [x] API contract management

### Planned 🚧
- [ ] Event Sourcing implementation
- [ ] Distributed caching (Redis)
- [ ] Database integration (Entity Framework Core)
- [ ] Authentication & Authorization (JWT/OAuth2)
- [ ] Rate limiting and throttling
- [ ] Circuit breaker pattern
- [ ] Event-driven architecture
- [ ] DAPR integration
- [ ] Prometheus metrics
- [ ] Comprehensive test suite

## 🆘 Support

- **Documentation**: Check the `/docs` folder
- **Issues**: Create an issue on GitHub
- **Discussions**: Use GitHub Discussions for questions

---

**Built with ❤️ using .NET 9 and modern architectural patterns**
