# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

### Build and Development
- **Build entire solution**: `dotnet build src/TodoAppTemplate.sln`
- **Run API**: `dotnet run --project src/TodoApp.Api` (runs on http://localhost:5172 and https://localhost:7188)
- **Run tests**: 
  - Unit tests: `dotnet test src/TodoApp.Tests.Unit`
  - Integration tests: `dotnet test src/TodoApp.Tests.Integration`
  - All tests: `dotnet test src/TodoAppTemplate.sln`
- **Restore packages**: `dotnet restore src/TodoAppTemplate.sln`

### Testing Framework
- Uses **xUnit** with **FluentAssertions** for assertions
- **Moq** for mocking in unit tests  
- **Testcontainers** and **WireMock** for integration tests
- **Microsoft.AspNetCore.Mvc.Testing** for API testing

## Architecture Overview

This is a Clean Architecture implementation in .NET 9 following Uncle Bob's principles with strict dependency inversion. The architecture enforces that business rules never depend on infrastructure details.

### Project Structure and Dependencies
```
TodoApp.Api (ASP.NET Core Web API)
├── References: Application, Core, All Infrastructure projects
└── Entry point with Minimal APIs and middleware

TodoApp.Application (Use Cases/Business Logic)
├── References: Core only
└── Commands, Queries, Validations, EventHandlers, Mappers

TodoApp.Core (Domain/Entities)
├── References: None
└── Entities, Domain Events, Repository interfaces, Business rules

Infrastructure Projects (Technical Details):
├── TodoApp.Infrastructure.Data.SqlServer (EF Core + SQL Server)
├── TodoApp.Infrastructure.Data.Services (External API integration via Refit)  
└── TodoApp.Infrastructure.Broker.AzureServiceBus (Event publishing)

Test Projects:
├── TodoApp.Tests.Unit (Core + Application testing)
└── TodoApp.Tests.Integration (Full stack integration tests)
```

### Key Architectural Patterns

**Domain-Driven Design**: 
- Entities are in `TodoApp.Core/Entities/` with business logic and domain events
- Aggregates organized by business context (e.g., `TodoItemAggregate/`)

**CQRS-like separation**:
- Commands in `Application/[Aggregate]/Commands/` for write operations
- Queries in `Application/[Aggregate]/Queries/` for read operations  
- Separate query repositories using Dapper for optimized reads

**Event-Driven Architecture**:
- Domain events inherit from `DomainEvent` and are automatically published
- Event handlers in `Application/[Aggregate]/EventHandlers/` 
- Azure Service Bus integration for cross-system communication

**Repository + Unit of Work**:
- Generic `IRepository<T>` in Core with concrete implementations in Infrastructure
- `IUnitOfWork` pattern for transaction management
- Custom repositories for complex queries (e.g., `ITodoItemRepository`)

**Dependency Injection Extensions**:
Each project provides registration extensions:
- `AddApplicationAggregates()` - Application services
- `AddSqlServerInfrastructure(connectionString)` - EF Core, repositories  
- `AddDataServices(apiUrl)` - External API clients
- `AddAzureServiceBusBroker(connectionString)` - Event publishing

### Entity Framework Features
- **Interceptors**: Automatic auditing, domain event publishing, soft delete
- **Type Configurations**: Entity mappings in `ModelMapping/` folder
- **Migrations**: Database versioning (run `dotnet ef migrations add` from SqlServer project)

### API Structure
- **Minimal APIs** organized in modules (`Api/Modules/`)
- **Middleware**: Authentication validation, global exception handling
- **OpenAPI/Swagger** automatically generated
- **Health checks** at `/health` endpoint

### Testing Approach
- **Unit tests**: Mock dependencies, test business logic in isolation
- **Integration tests**: Use `TodoAppWebApplicationFactory` with test containers
- Entity tests verify domain events are raised correctly
- Service tests validate business rules and validations

### Common Patterns to Follow
- All entities inherit from `BaseEntity<TKey>` or `BaseAuditableEntity<TKey>`
- Use FluentValidation for input validation in Application layer  
- Domain events for cross-aggregate communication
- DTOs for data transfer between layers with mapper extensions
- Result pattern for operation outcomes with error handling

## Language Response
- Always return in Brazilian Portuguese