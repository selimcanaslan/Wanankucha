# Wanankucha

A full-stack .NET 8 application with REST API and Blazor Server web frontend, built with Clean Architecture principles.

## 🏗️ Project Structure

```
Wanankucha/
├── api/                              # Backend API
│   ├── Wanankucha.Api/              # Presentation Layer (Controllers, Middlewares)
│   ├── Wanankucha.Api.Application/  # Application Layer (Use Cases, DTOs, Services)
│   ├── Wanankucha.Api.Domain/       # Domain Layer (Entities, Repository Interfaces)
│   ├── Wanankucha.Api.Infrastructure/ # Infrastructure Layer (Token, Email, Password Services)
│   └── Wanankucha.Api.Persistence/  # Persistence Layer (Database, EF Core)
├── web/                              # Blazor Web Frontend
│   └── Wanankucha.Web/              # Blazor Server Application
└── shared/                           # Shared Libraries
    └── Wanankucha.Shared/           # Common DTOs and Wrappers
```

## 🛠️ Technologies

### Backend (API)

- **.NET 8** - Framework
- **Entity Framework Core 8** - ORM with resilient connections
- **PostgreSQL** - Database
- **MediatR** - CQRS Pattern
- **FluentValidation** - Request Validation
- **JWT Authentication** - Security with refresh token rotation
- **Hangfire** - Background job processing
- **Serilog** - Structured Logging
- **Swagger/OpenAPI** - API Documentation

### Frontend (Web)

- **Blazor Server** - Interactive UI
- **JWT Authentication** - Token-based auth
- **Cookie Storage** - Secure token storage
- **Polly** - HTTP resilience (retry, circuit breaker, timeout)
- **Serilog** - Structured Logging
- **Real-time Validation** - Form validation on keystroke

## ✨ Features

### Security

- **Account Lockout** - Locks account after 5 failed login attempts (15 min)
- **Refresh Token Rotation** - New token generated on each refresh
- **Password Reset Flow** - Email-based password reset with secure tokens
- **Rate Limiting** - Protects against brute-force attacks
- **HSTS** - Strict Transport Security in production
- **Secure Hangfire** - Dashboard restricted to localhost in production
- **CORS Tightening** - Explicit allowed methods and headers

### Performance

- **Response Compression** - Brotli + Gzip compression
- **Database Indexes** - Optimized token lookups
- **Output Caching** - Configurable response caching
- **Connection Resilience** - Auto-retry for transient DB failures

### Reliability

- **Health Checks** - `/health` endpoint for monitoring
- **Background Jobs** - Hangfire for async processing (token cleanup)
- **Startup DB Check** - Fail-fast if database unavailable
- **HTTP Resilience** - Retry, circuit breaker, timeout policies

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/) (or Docker)

### Configuration

1. **Clone and restore:**

   ```bash
   git clone <repository-url>
   cd Wanankucha
   dotnet restore
   ```

2. **Set up secrets (Development):**

   ```bash
   cd api/Wanankucha.Api
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:PostgreSQL" "Host=localhost;Port=5432;Database=WanankuchaDB;Username=postgres;Password=YOUR_PASSWORD"
   dotnet user-secrets set "Token:SecurityKey" "YOUR_64_CHARACTER_MINIMUM_SECRET_KEY"
   dotnet user-secrets set "Smtp:Username" "your-email@gmail.com"
   dotnet user-secrets set "Smtp:Password" "your-app-password"
   ```

3. **Update database:**

   ```bash
   dotnet ef database update --project api/Wanankucha.Api.Persistence --startup-project api/Wanankucha.Api
   ```

4. **Run the applications:**

   ```bash
   # Terminal 1 - API
   dotnet run --project api/Wanankucha.Api

   # Terminal 2 - Web
   dotnet run --project web/Wanankucha.Web
   ```

### Access Points

| Application      | URL                               | Description          |
| ---------------- | --------------------------------- | -------------------- |
| **API Root**     | `https://localhost:7230`          | Redirects to Swagger |
| **API Swagger**  | `https://localhost:7230/swagger`  | API Documentation    |
| **Health Check** | `https://localhost:7230/health`   | Health status        |
| **Hangfire**     | `https://localhost:7230/hangfire` | Job dashboard        |
| **Web App**      | `http://localhost:5279`           | Blazor Frontend      |

## 📚 API Endpoints

### Authentication

| Method | Endpoint                      | Description             | Auth |
| ------ | ----------------------------- | ----------------------- | ---- |
| POST   | `/api/v1/Auth/Register`       | Create new user account | No   |
| POST   | `/api/v1/Auth/Login`          | User login              | No   |
| POST   | `/api/v1/Auth/RefreshToken`   | Refresh JWT token       | No   |
| POST   | `/api/v1/Auth/ForgotPassword` | Request password reset  | No   |
| POST   | `/api/v1/Auth/ResetPassword`  | Reset with token        | No   |

### Users

| Method | Endpoint        | Description   | Auth |
| ------ | --------------- | ------------- | ---- |
| GET    | `/api/v1/Users` | Get all users | Yes  |
| POST   | `/api/v1/Users` | Create user   | Yes  |

## 🔐 Security Features

### Authentication Flow

1. User submits credentials on login page
2. API verifies credentials and checks account lockout status
3. JWT + Refresh Token generated and returned
4. Tokens stored in encrypted session storage
5. Refresh token rotation on each refresh request
6. Account locks after 5 failed attempts (15 min lockout)

### Password Reset Flow

1. User clicks "Forgot Password?" on login page
2. Enters email → API generates secure reset token (1 hour expiry)
3. Email sent with reset link
4. User sets new password via `/reset-password` page

### Security Measures

| Feature              | Description                      |
| -------------------- | -------------------------------- |
| **Password Hashing** | BCrypt with salt                 |
| **JWT Signing**      | HMAC-SHA256                      |
| **Token Rotation**   | New refresh token on each use    |
| **Account Lockout**  | 5 failures → 15 min lock         |
| **Rate Limiting**    | Token bucket with sliding window |
| **HSTS**             | Enabled in production            |

## ⚙️ Configuration

### User Secrets (Development)

Sensitive values are stored in .NET User Secrets:

```bash
# View all secrets
dotnet user-secrets list

# Set a secret
dotnet user-secrets set "Key:SubKey" "value"
```

### Environment Variables (Production)

Use double underscores for nested keys:

```bash
export ConnectionStrings__PostgreSQL="Host=..."
export Token__SecurityKey="..."
export Smtp__Username="..."
export Smtp__Password="..."
```

See `.env.example` for all available variables.

## 📝 Logging

Serilog structured logging in both API and Web projects:

- **Console**: Information level and above
- **File**: JSON logs with daily rolling
  - API: `logs/wanankucha-api-log-{date}.txt`
  - Web: `logs/wanankucha-web-log-{date}.txt`

## 🔄 Background Jobs (Hangfire)

| Job                       | Schedule | Description                          |
| ------------------------- | -------- | ------------------------------------ |
| `CleanupExpiredTokensJob` | Hourly   | Removes expired refresh/reset tokens |

Dashboard: `https://localhost:7230/hangfire`

## 📄 License

This project is licensed under the MIT License.
