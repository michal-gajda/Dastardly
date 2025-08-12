# Published Language Implementation with FastEndpoints

## Overview

This implementation follows Domain-Driven Design (DDD) principles by establishing a **Published Language** using FastEndpoints. The Published Language is located in the Infrastructure layer and provides a stable API contract that abstracts internal domain changes.

## Architecture

```text

Infrastructure/
├── PublishedLanguage/
│   ├── Contracts/
│   │   └── OrderContracts.cs          # API contract definitions
│   ├── Endpoints/
│   │   ├── CreateOrderEndpoint.cs     # FastEndpoints implementation
│   │   └── GetOrderEndpoint.cs        # Query endpoint
│   └── PublishedLanguageMapper.cs     # Translation layer
```

## Key Concepts

### 1. Published Language Contracts

- **Stable API contracts** that external consumers can depend on
- **Version-controlled** and backwards-compatible
- **Independent** of internal domain model changes

### 2. Translation Layer (PublishedLanguageMapper)

- Maps between Published Language contracts and internal domain objects
- Ensures internal changes don't break the published API
- Provides a clean abstraction boundary

### 3. FastEndpoints Benefits

- **Performance**: Faster than traditional MVC controllers
- **Type Safety**: Strongly typed requests/responses
- **Validation**: Built-in FluentValidation support
- **Documentation**: Automatic OpenAPI/Swagger generation
- **Minimal API style** with better organization

## Published Language Endpoints

### Create Order

- **Endpoint**: `POST /api/orders`
- **Contract**: `CreateOrderRequest` → `CreateOrderResponse`
- **Purpose**: Stable interface for order creation
- **Implementation**: Translates to internal `CreateOrderCommand`

### Get Order

- **Endpoint**: `GET /api/orders/{id}`
- **Contract**: `CreateOrderResponse`
- **Purpose**: Query order status using published language
- **Implementation**: Returns order data in published format

## Usage Examples

### Creating an Order

```json
POST /api/orders
{
  "customerId": "123e4567-e89b-12d3-a456-426614174000",
  "items": ["Widget A", "Widget B"]
}
```

### Response

```json
{
  "orderId": "987fcdeb-51a2-4567-8901-234567890abc",
  "status": "Pending",
  "createdAt": "2025-08-11T10:30:00Z",
  "message": "Order has been queued for processing"
}
```

## Benefits of This Approach

1. **Stability**: Published Language provides API stability
2. **Evolution**: Internal domain can evolve without breaking clients
3. **Documentation**: Clear contracts with automatic documentation
4. **Performance**: FastEndpoints provide better performance than MVC
5. **Type Safety**: Compile-time checking of API contracts
6. **Validation**: Built-in request validation
7. **C4 Compliance**: Clear separation between layers

## Integration with Existing Architecture

- **Domain Layer**: Contains business entities (`Order`, `OrderStatus`)
- **Application Layer**: Contains commands and handlers (`CreateOrderCommand`, `CreateOrderHandler`)
- **Infrastructure Layer**: Contains published language and external concerns
- **WebApi Layer**: Hosts both traditional controllers and FastEndpoints

This implementation allows for gradual migration from traditional controllers to FastEndpoints while maintaining backwards compatibility.
