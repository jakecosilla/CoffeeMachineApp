# Coffee Machine API

An HTTP API that simulates an imaginary internet-connected coffee machine. The API implements various business logic rules including call counting, service unavailability cycles, and April Fools' Day behavior.

## Requirements Met

### 1. Normal Operation (200 OK)
- **Endpoint**: `GET /brew-coffee`
- **Response**: 
  ```json
  {
    "message": "Your piping hot coffee is ready",
    "prepared": "2026-03-18T21:17:09.1208160+08:00"
  }
  ```
- **Status Code**: 200 OK
- **Feature**: Returns a JSON response with the current timestamp in ISO-8601 format

### 2. Every 5th Call Returns Service Unavailable (503)
- **Trigger**: Every 5th call to the endpoint (5, 10, 15, etc.)
- **Response**: Empty body
- **Status Code**: 503 Service Unavailable
- **Reason**: Simulates the coffee machine running out of coffee

### 3. April 1st Returns I'm a Teapot (418)
- **Trigger**: When the system date is April 1st
- **Response**: Empty body
- **Status Code**: 418 I'm a Teapot
- **Reason**: April Fools' Day joke per RFC 2324

## Architecture

The solution follows a clean, layered architecture pattern consistent with the CustomerOnboardingApp:

### Layers

1. **Domain** (`Domain.csproj`)
   - Core business entities
   - `CoffeeMachine` entity for state representation

2. **Application** (`Application.csproj`)
   - Business logic and use cases
   - `ICoffeeMachineService` interface and `CoffeeMachineService` implementation
   - Service registration extensions

3. **Infrastructure** (`Infrastructure.csproj`)
   - Data access and state management
   - `ICoffeeMachineStateManager` for tracking call counts
   - Thread-safe in-memory state management
   - Service registration extensions

4. **Api** (`Api.csproj`)
   - HTTP endpoint definitions
   - `CoffeeEndpoints` extension with the `/brew-coffee` endpoint
   - Program.cs with logging, exception handling, and CORS configuration
   - Swagger/OpenAPI documentation

5. **Tests** (`Tests.csproj`)
   - Comprehensive test suite using XUnit
   - Service tests with Moq for unit testing business logic
   - Endpoint integration tests using WebApplicationFactory
   - FluentAssertions for expressive test assertions

## Test Organization

Tests are organized following the application structure:

- **Tests/Application/Services/CoffeeMachineServiceTests.cs** - Unit tests for the CoffeeMachineService
- **Tests/Api/Endpoints/CoffeeEndpointsTests.cs** - Integration tests for the API endpoints

## Running the Application

### Build

```bash
dotnet build
```

### Run API Server

```bash
dotnet run --project backend/Api
```

The API will start on `https://localhost:7000` (HTTPS) and `http://localhost:5000` (HTTP).

Access Swagger UI at: `https://localhost:7000/swagger/ui`

### Run Tests

```bash
dotnet test
```

## Testing Examples

### Using curl

```bash
# First call - should return 200 OK
curl -i http://localhost:5000/brew-coffee

# 5th call in sequence - should return 503
# (Make 4 calls first, then)
curl -i http://localhost:5000/brew-coffee

# On April 1st - should return 418
curl -i http://localhost:5000/brew-coffee
```

## Test Coverage

The project includes 14 comprehensive tests:

### Service Tests (8 tests)
- First call returns 200 OK with correct message and timestamp
- Response timestamp is valid ISO-8601 format
- Every 5th call returns 503 Service Unavailable
- 10th call returns 503
- Calls after 503 resume with 200 OK
- 12 consecutive calls follow expected pattern
- April Fools' Day special behavior (418)

### Endpoint Tests (6 tests)
- First call returns 200 OK with brewed coffee
- Valid JSON response structure
- Every 5th call returns 503
- 10th call returns 503
- 6th call after 503 resumes with 200 OK
- 12 consecutive calls follow expected pattern
- April Fools' Day returns 418
- Response is valid JSON

All tests pass successfully.

## Design Decisions

### State Management
- Used in-memory singleton `CoffeeMachineStateManager` for simplicity
- Thread-safe using lock mechanism
- Could be replaced with database persistence if needed

### Testing Framework
- **XUnit**: Modern .NET testing framework with parameterized tests
- **Moq**: Industry-standard mock library for dependency injection
- **FluentAssertions**: Expressive assertion library for readable assertions
- **WebApplicationFactory**: Built-in testing infrastructure for integration tests

### Status Codes
- 200 OK for successful brew
- 503 Service Unavailable to indicate service degradation
- 418 I'm a Teapot as per RFC 2324 for April Fools' joke

### Architecture Benefits
- Clear separation of concerns
- Testable business logic layer
- Easy to swap implementations (e.g., database for state)
- Scalable structure for additional features
- Consistent patterns with CustomerOnboardingApp

## Development Features

- **Logging**: Request/response logging middleware for debugging
- **Exception Handling**: Global exception handler with JSON error responses
- **CORS**: Open CORS policy for development (configurable for production)
- **Swagger/OpenAPI**: API documentation and interactive testing in development
- **Request Tracing**: HTTP method and path logging for all requests
