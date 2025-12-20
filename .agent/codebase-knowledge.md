# Wanankucha Codebase Knowledge

## Project Overview

.NET 8 full-stack application with Clean Architecture, CQRS pattern, and Blazor Server frontend.

---

## Architecture Layers

### 1. API (Presentation Layer)

**Path:** `api/Wanankucha.Api/`

| Component        | Description                                    |
| ---------------- | ---------------------------------------------- |
| **Controllers/** | AuthController, UsersController                |
| **Middlewares/** | GlobalExceptionHandlerMiddleware               |
| **Jobs/**        | CleanupExpiredTokensJob (Hangfire)             |
| **Program.cs**   | Host config, JWT auth, CORS, Serilog, Hangfire |

**Key Features:**

- JWT Bearer authentication with HMAC-SHA256
- Swagger/OpenAPI documentation with API versioning
- Global exception handling with structured responses
- Serilog logging with console + file output
- Rate limiting (token bucket)
- Response compression (Brotli + Gzip)
- Health checks (`/health`)
- HSTS in production
- Root endpoint redirects to Swagger

---

### 2. Application Layer

**Path:** `api/Wanankucha.Api.Application/`

| Component              | Description                                                        |
| ---------------------- | ------------------------------------------------------------------ |
| **Features/Commands/** | CreateUser, LoginUser, RefreshToken, ForgotPassword, ResetPassword |
| **Features/Queries/**  | GetAllUsers                                                        |
| **Behaviors/**         | LoggingBehavior, ValidationBehavior                                |
| **Abstractions/**      | ITokenService, IPasswordHasher, IEmailService                      |
| **DTOs/**              | Response DTOs                                                      |

**CQRS Implementation:**

- MediatR for command/query dispatching
- FluentValidation for request validation
- Pipeline behaviors for cross-cutting concerns

---

### 3. Domain Layer

**Path:** `api/Wanankucha.Api.Domain/`

| Component         | Description                                                     |
| ----------------- | --------------------------------------------------------------- |
| **Entities/**     | User (with lockout fields), Role, UserRole                      |
| **Common/**       | BaseEntity<TKey>, IEntity                                       |
| **Repositories/** | IUserRepository, IReadRepository, IWriteRepository, IUnitOfWork |

**User Entity Properties:**

- Core: Id, Email, UserName, PasswordHash, NameSurname
- Tokens: RefreshToken, RefreshTokenEndDate
- Password Reset: PasswordResetToken, PasswordResetTokenExpiry
- Lockout: FailedLoginAttempts, LockoutEnd, LockoutEnabled
- Base: CreatedDate, UpdatedDate, IsDeleted (soft delete)

---

### 4. Infrastructure Layer

**Path:** `api/Wanankucha.Api.Infrastructure/`

| Component                                    | Description                              |
| -------------------------------------------- | ---------------------------------------- |
| **Services/Token/TokenService**              | JWT generation, refresh token management |
| **Services/Email/SmtpEmailService**          | SMTP email with HTML templates           |
| **Services/Encryption/BCryptPasswordHasher** | BCrypt password hashing                  |
| **Options/**                                 | JwtOptions, SmtpOptions                  |

---

### 5. Persistence Layer

**Path:** `api/Wanankucha.Api.Persistence/`

| Component                 | Description                          |
| ------------------------- | ------------------------------------ |
| **Contexts/AppDbContext** | EF Core context with auto-timestamps |
| **Repositories/**         | UserRepository implementation        |
| **Configurations/**       | Entity configs with indexes          |
| **UnitOfWork**            | Transaction management               |

**DbContext Features:**

- Global query filters for soft delete (`IsDeleted`)
- Auto-set `CreatedDate`/`UpdatedDate` timestamps
- Connection resilience with auto-retry
- Database indexes on RefreshToken, PasswordResetToken

---

### 6. Web (Blazor Server)

**Path:** `web/Wanankucha.Web/`

| Component                  | Description                                    |
| -------------------------- | ---------------------------------------------- |
| **Components/Pages/Auth/** | Login, Register, ForgotPassword, ResetPassword |
| **Components/Layout/**     | MainLayout, AuthLayout                         |
| **Auth/**                  | JwtAuthenticationStateProvider                 |
| **Services/**              | AuthService, TokenStorageService               |

**Features:**

- Interactive Server rendering
- JWT authentication with cookie storage
- Polly HTTP resilience (retry, circuit breaker, 30s timeout)
- Serilog logging (project-specific log files)
- Real-time form validation on keystroke
- Failed login attempt tracking with password reset suggestion
- Modern glassmorphism UI design

---

### 7. Shared Library

**Path:** `shared/Wanankucha.Shared/`

| Component     | Description                                                          |
| ------------- | -------------------------------------------------------------------- |
| **DTOs/**     | LoginRequest, RegisterRequest, TokenDto, ForgotPasswordRequest, etc. |
| **Wrappers/** | ApiResponse<T>                                                       |

---

## ✅ Implemented Features

### Security

- [x] JWT authentication with refresh token rotation
- [x] Account lockout (5 attempts → 15 min lock)
- [x] Password reset flow with email
- [x] Rate limiting on auth endpoints
- [x] HSTS (production)
- [x] Secure Hangfire dashboard (localhost only in prod)
- [x] CORS with explicit allowed methods/headers
- [x] BCrypt password hashing

### Performance

- [x] Response compression (Brotli + Gzip)
- [x] Database indexes on token columns
- [x] Output caching
- [x] Connection resilience with retry
- [x] Distributed caching (IDistributedCache)

### Reliability

- [x] Health checks (`/health`)
- [x] Startup database check (fail-fast)
- [x] HTTP resilience with Polly
- [x] Background jobs with Hangfire
- [x] Structured logging with Serilog

### Developer Experience

- [x] Swagger/OpenAPI with API versioning
- [x] User Secrets for sensitive config
- [x] Environment variables template (.env.example)
- [x] Root endpoint redirects to Swagger

---

## Configuration

### Secrets Management

**Development:** Use .NET User Secrets

```bash
cd api/Wanankucha.Api
dotnet user-secrets set "ConnectionStrings:PostgreSQL" "Host=..."
dotnet user-secrets set "Token:SecurityKey" "..."
dotnet user-secrets set "Smtp:Username" "..."
dotnet user-secrets set "Smtp:Password" "..."
```

**Production:** Use environment variables

```bash
export ConnectionStrings__PostgreSQL="..."
export Token__SecurityKey="..."
export Smtp__Username="..."
export Smtp__Password="..."
```

### Key Config Files

| File               | Purpose                         |
| ------------------ | ------------------------------- |
| `appsettings.json` | Base config (placeholders only) |
| `.env.example`     | Environment variables template  |

---

## API Endpoints

| Method | Endpoint                      | Description               | Auth      |
| ------ | ----------------------------- | ------------------------- | --------- |
| POST   | `/api/v1/Auth/Register`       | Register new user         | No        |
| POST   | `/api/v1/Auth/Login`          | Login (with lockout)      | No        |
| POST   | `/api/v1/Auth/RefreshToken`   | Refresh tokens (rotation) | No        |
| POST   | `/api/v1/Auth/ForgotPassword` | Request password reset    | No        |
| POST   | `/api/v1/Auth/ResetPassword`  | Reset with token          | No        |
| GET    | `/api/v1/Users`               | Get all users             | Yes       |
| GET    | `/health`                     | Health check              | No        |
| GET    | `/hangfire`                   | Job dashboard             | Localhost |

---

## Background Jobs

| Job                       | Schedule | Description                          |
| ------------------------- | -------- | ------------------------------------ |
| `CleanupExpiredTokensJob` | Hourly   | Removes expired refresh/reset tokens |

---

## Future Improvements

### 🔴 High Priority

- [ ] Two-factor authentication (TOTP)
- [ ] Audit logging for security events
- [ ] Unit tests for handlers

### 🟡 Medium Priority

- [ ] OpenTelemetry for distributed tracing
- [ ] Prometheus metrics endpoint
- [ ] Integration tests with TestContainers

### 🟢 Low Priority

- [ ] Redis caching (replace in-memory)
- [ ] Component virtualization for large lists
- [ ] Lazy load Blazor components
