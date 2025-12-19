# Wanankucha

A full-stack .NET 8 application with REST API and Blazor Server web frontend, built with Clean Architecture principles.

## 🏗️ Project Structure

The solution is organized into three main areas:

```
Wanankucha/
├── api/                              # Backend API
│   ├── Wanankucha.Api/              # Presentation Layer (Controllers, Middlewares)
│   ├── Wanankucha.Api.Application/  # Application Layer (Use Cases, DTOs, Services)
│   ├── Wanankucha.Api.Domain/       # Domain Layer (Entities, Repository Interfaces)
│   ├── Wanankucha.Api.Infrastructure/ # Infrastructure Layer (Token, Password Services)
│   └── Wanankucha.Api.Persistence/  # Persistence Layer (Database, EF Core)
├── web/                              # Blazor Web Frontend
│   └── Wanankucha.Web/              # Blazor Server Application
└── shared/                           # Shared Libraries
    └── Wanankucha.Shared/           # Common DTOs and Wrappers
```

## 🛠️ Technologies

### Backend (API)

- **.NET 8** - Framework
- **Entity Framework Core 8** - ORM
- **PostgreSQL** - Database
- **MediatR** - CQRS Pattern
- **FluentValidation** - Request Validation
- **JWT Authentication** - Security
- **Serilog** - Structured Logging
- **Swagger/OpenAPI** - API Documentation

### Frontend (Web)

- **Blazor Server** - Interactive UI
- **JWT Authentication** - Token-based auth
- **Cookie Storage** - Secure token storage
- **Polly** - HTTP resilience (retry, circuit breaker, timeout)
- **Serilog** - Structured Logging
- **Real-time Validation** - Form validation on keystroke

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/)

### Installation

1. Clone the repository:

   ```bash
   git clone <repository-url>
   cd Wanankucha
   ```

2. Restore dependencies:

   ```bash
   dotnet restore
   ```

3. Update the database:

   ```bash
   dotnet ef database update --project api/Wanankucha.Api.Persistence --startup-project api/Wanankucha.Api
   ```

4. Run the API:

   ```bash
   dotnet run --project api/Wanankucha.Api
   ```

5. Run the Web App (in a new terminal):
   ```bash
   dotnet run --project web/Wanankucha.Web
   ```

### Access Points

| Application     | URL                              | Description       |
| --------------- | -------------------------------- | ----------------- |
| **API Swagger** | `https://localhost:5279/swagger` | API Documentation |
| **Web App**     | `https://localhost:5001`         | Blazor Frontend   |

## 🏛️ Architecture

### Clean Architecture Layers

| Layer              | Responsibility                                                         |
| ------------------ | ---------------------------------------------------------------------- |
| **Api**            | HTTP endpoints, request/response handling, global exception middleware |
| **Application**    | Business logic, use cases, DTOs, service interfaces, MediatR handlers  |
| **Domain**         | Core entities, domain logic, repository interfaces                     |
| **Infrastructure** | Token service (JWT), password hashing (BCrypt)                         |
| **Persistence**    | DbContext, migrations, repository implementations, UnitOfWork          |

### CQRS with MediatR

```
Application/Features/
├── Commands/
│   └── AppUser/
│       ├── CreateUser/    # User registration
│       ├── LoginUser/     # Authentication
│       └── RefreshToken/  # Token refresh
└── Queries/
    └── AppUser/
        └── GetAllUsers/   # List users
```

## 📚 API Endpoints

| Method | Endpoint                 | Description               | Auth |
| ------ | ------------------------ | ------------------------- | ---- |
| POST   | `/api/Auth/Register`     | Create new user account   | No   |
| POST   | `/api/Auth/Login`        | User login                | No   |
| POST   | `/api/Auth/RefreshToken` | Refresh JWT token         | No   |
| GET    | `/api/Users`             | Get all users (paginated) | Yes  |

## 🔐 Authentication Flow

1. User submits credentials on login page
2. Web app sends request to API `/api/Auth/Login`
3. API verifies credentials and generates JWT + Refresh Token
4. Tokens are returned and stored in `ProtectedSessionStorage`
5. `JwtAuthenticationStateProvider` manages auth state
6. Protected routes check `AuthorizeRouteView` for access

### Security Features

- **Password Hashing**: BCrypt with salt
- **JWT Signing**: HMAC-SHA256
- **Token Storage**: Encrypted session storage
- **Refresh Tokens**: Stored in database, 7-day validity
- **Role-Based Access**: Users assigned "User" role on registration
- **CORS**: Configured for web app origin

## ⚙️ Configuration

### API (`api/Wanankucha.Api/appsettings.json`)

```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Port=5432;Database=WanankuchaDB;Username=postgres;Password=yourpassword"
  },
  "Token": {
    "Audience": "your-audience",
    "Issuer": "your-issuer",
    "SecurityKey": "your-256-bit-secret-key",
    "Expiration": 15
  }
}
```

### Web (`web/Wanankucha.Web/appsettings.json`)

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:5279/"
  }
}
```

## 📝 Logging

Serilog structured logging in both API and Web projects:

- **Console**: Information level and above
- **File**: JSON logs with daily rolling
  - API: `logs/wanankucha-api-log-{date}.txt`
  - Web: `logs/wanankucha-web-log-{date}.txt`

## 🔄 HTTP Resilience (Web)

The web app uses Polly for HTTP resilience:

| Policy              | Configuration                                    |
| ------------------- | ------------------------------------------------ |
| **Timeout**         | 30 seconds default                               |
| **Retry**           | 3 attempts with exponential backoff (2s, 4s, 8s) |
| **Circuit Breaker** | Opens after 5 failures, 30s recovery             |

## 📄 License

This project is licensed under the MIT License.
