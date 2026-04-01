# Coffee Machine API

An HTTP API that simulates an imaginary internet-connected coffee machine. The API implements business rules such as call counting, service unavailability cycles, April Fools' behavior, and weather-based coffee selection.

---

# ☕ Features

## 1. Normal Operation (200 OK)

* **Endpoint**: `GET /brew-coffee`
* **Response**:

```json
{
  "message": "Your piping hot coffee is ready",
  "prepared": "2026-03-18T21:17:09+08:00"
}
```

* Returns ISO-8601 timestamp with timezone

---

## 2. Every 5th Call Returns 503

* **Trigger**: 5th, 10th, 15th calls
* **Response**: Empty body
* **Status**: `503 Service Unavailable`
* Simulates machine running out of coffee

---

## 3. April 1st Returns 418

* **Trigger**: April 1
* **Response**: Empty body
* **Status**: `418 I'm a Teapot`
* Based on RFC 2324

---

## 4. 🌤 Weather-Based Coffee

The API integrates with OpenWeather API:

* If **temperature > 30°C**

  ```json
  {
    "message": "Your refreshing iced coffee is ready"
  }
  ```

* Otherwise:

  ```json
  {
    "message": "Your piping hot coffee is ready"
  }
  ```

* If weather API fails → fallback to hot coffee

---

# 🏗 Architecture

Follows **Clean Architecture**

### Layers

### 1. Domain

* Core entities

### 2. Application

* Business logic
* `CoffeeMachineService`
* Interfaces (contracts)
* DTOs

### 3. Infrastructure

* External integrations
* Weather API client
* State management

### 4. API

* Endpoint definitions
* Middleware
* DI configuration

### 5. Tests

* Unit + Integration tests
* XUnit + Moq + FluentAssertions

---

# ⚙️ Setup Instructions

## 1. Clone Repo

```bash
git clone https://github.com/jakecosilla/CoffeeMachineApp.git
cd CoffeeMachineApp/backend
```

---

## 2. Install Dependencies

```bash
dotnet restore
```

---

## 3. Configure Weather API

Create or update:

### `appsettings.Development.json`

```json
{
  "Weather": {
    "ApiKey": "YOUR_API_KEY",
    "BaseUrl": "https://api.openweathermap.org/",
    "City": "Manila"
  }
}
```

---

## 🔑 Getting API Key

1. Go to: https://openweathermap.org/api
2. Create a free account
3. Generate API key
4. Replace:

```json
"ApiKey": "YOUR_API_KEY"
```

---

## ⚠️ Important Notes

* API key may take a few minutes to activate
* Invalid key will return:

  ```json
  { "cod": 401, "message": "Invalid API key" }
  ```

---

## ✅ Alternative: Environment Variable

```bash
export Weather__ApiKey=your_api_key
```

---

# 🚀 Run the Application

```bash
dotnet run --project Api
```

API available at:

* https://localhost:7000
* http://localhost:5000

Swagger:

```
https://localhost:7000/swagger
```

---

# 🧪 Run Tests

```bash
dotnet test
```

---

# 🧪 Test Coverage

### ✅ Core Behavior

* 200 OK response
* 503 every 5th call
* 418 April Fools

### ✅ Weather Feature

* Hot weather → iced coffee
* Cold weather → hot coffee
* API failure → fallback

### ✅ Integration Tests

* Endpoint validation
* JSON structure
* Response patterns

---

# 🧠 Design Decisions

## Clean Architecture

* Separation of concerns
* Testable business logic
* Replaceable infrastructure

---

## Weather Integration

* Implemented in Infrastructure layer
* Uses HttpClientFactory
* Config-driven (Options pattern)

---

## Date Handling

* Uses `DateTimeOffset`
* Ensures timezone-aware ISO format

---

## State Management

* In-memory singleton
* Thread-safe
* Easily replaceable with DB

---

## Testing Strategy

* Unit tests (Application)
* Integration tests (API)
* Mocked dependencies (Weather, Time)

---

# 📌 Development Features

* Logging middleware
* Global exception handling
* Swagger/OpenAPI
* Config validation (fail fast)
* Dependency Injection

---

# 🏆 Summary

This project demonstrates:

* Clean Architecture in .NET
* External API integration
* Robust error handling
* Production-ready testing strategy
* Strong separation of concerns

---

# 💡 Notes

* Weather API is optional for core functionality
* System gracefully degrades if weather service fails
* Designed for extensibility and scalability

---

# 👨‍💻 Author

Jake Ray Osilla
