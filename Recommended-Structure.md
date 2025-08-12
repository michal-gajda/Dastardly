# Recommended Project Structure for C4 Compliance

## Current Structure (Not C4-aligned)

```text
src/
└── WebApi/
    ├── Controllers/
    ├── CreateOrderCommand.cs      # ❌ Should be in Application
    ├── CreateOrderHandler.cs      # ❌ Should be in Application  
    ├── BackgroundCommandDispatcher.cs # ❌ Should be in Infrastructure
    └── Program.cs
```

## Recommended C4-Aligned Structure

```text
src/
├── Dastardly.Domain/              # Core business rules
│   ├── Entities/
│   │   └── Order.cs
│   ├── ValueObjects/
│   │   └── CustomerId.cs
│   └── Services/
│       └── OrderDomainService.cs
│
├── Dastardly.Application/         # Use cases and application logic
│   ├── Commands/
│   │   ├── CreateOrderCommand.cs
│   │   └── CreateOrderHandler.cs
│   ├── Queries/
│   └── Interfaces/
│       └── IBackgroundCommandDispatcher.cs
│
├── Dastardly.Infrastructure/      # External concerns
│   ├── Messaging/
│   │   └── BackgroundCommandDispatcher.cs
│   ├── Persistence/
│   └── ServiceExtensions.cs
│
└── Dastardly.WebApi/             # HTTP interface only
    ├── Controllers/
    │   └── OrdersController.cs
    ├── ObservabilityExtensions.cs
    └── Program.cs
```
