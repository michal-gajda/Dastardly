# Infrastructure Layer Structure

This directory follows the Katmai repository pattern with proper separation of concerns for the Infrastructure layer.

## Correct Directory Structure (Following Katmai Pattern)

```text
Infrastructure/
├── Endpoints/                    # FastEndpoints Implementation
│   ├── CreateOrderEndpoint.cs    # POST /api/orders endpoint
│   └── GetOrderEndpoint.cs      # GET /api/orders/{id} endpoint
├── Contracts/                   # API Contract Definitions
│   ├── CreateOrderRequest.cs    # Request contract for order creation
│   ├── CreateOrderResponse.cs   # Response contract for order creation
│   ├── GetOrderResponse.cs      # Response contract for order queries
│   └── ErrorResponse.cs        # Standard error response contract
├── Validators/                  # Request Validation
│   └── CreateOrderRequestValidator.cs # Validation rules for CreateOrderRequest
├── Mappers/                    # Translation Layer
│   └── PublishedLanguageMapper.cs # Maps between contracts and domain
├── Messaging/                  # Background Processing
│   └── BackgroundCommandDispatcher.cs # Hangfire integration
├── ServiceExtensions.cs       # DI configuration
└── README.md                  # This file
```

## Why This Structure is Correct

### 1. **Follows Katmai Pattern**

- **Endpoints** folder at the Infrastructure root level
- Clear separation of technical concerns
- Matches established .NET conventions

### 2. **Infrastructure Layer Responsibilities**

- **Endpoints**: External API interface (FastEndpoints)
- **Contracts**: Stable API contracts (Published Language)
- **Validators**: Input validation logic
- **Mappers**: Translation between external contracts and internal domain
- **Messaging**: External messaging and background processing

### 3. **Benefits of This Organization**

- **Clear Intent**: Each folder has a specific technical purpose
- **Scalability**: Easy to add new endpoints, contracts, validators
- **Maintainability**: Clear separation makes code easy to find and modify
- **Team Familiarity**: Follows established patterns from Katmai

## Key Principles

### Published Language Concepts

The "Published Language" is a DDD concept, not a folder structure. It refers to:

- **Stable API contracts** that consumers can depend on
- **Backward compatibility** guarantees
- **Clear documentation** of the external interface
- **Translation layer** that protects internal domain changes

### Folder Organization

- **Technical concerns** get their own folders (Endpoints, Contracts, etc.)
- **Business concepts** are expressed through naming and documentation
- **Infrastructure layer** handles external concerns and integration

## Migration Benefits

### Before (Incorrect)

```text
Infrastructure/PublishedLanguage/...  # Conceptual grouping
```

### After (Correct)

```text
Infrastructure/Endpoints/...          # Technical grouping
Infrastructure/Contracts/...          # Published Language contracts
```

This structure:

- ✅ Follows Katmai repository patterns
- ✅ Separates technical concerns properly
- ✅ Makes the codebase more familiar to .NET developers
- ✅ Implements Published Language concepts correctly
- ✅ Scales better for additional endpoints and contracts

The Published Language is now properly implemented through the **Contracts** folder and **Mappers**, while **Endpoints** handle the technical FastEndpoints implementation.
