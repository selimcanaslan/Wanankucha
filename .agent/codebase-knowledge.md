# Wanankucha Codebase Knowledge

## Project Overview

.NET 8 full-stack application with Clean Architecture, CQRS pattern, and Blazor Server frontend.

---

## Architecture Layers

### 1. API (Presentation Layer)

**Path:** `api/Wanankucha.Api/`

| Component        | Description                          |
| ---------------- | ------------------------------------ |
| **Controllers/** | AuthController, UsersController      |
| **Middlewares/** | GlobalExceptionHandlerMiddleware     |
| **Program.cs**   | Host config, JWT auth, CORS, Serilog |

**Key Features:**

- JWT Bearer authentication with HMAC-SHA256
- Swagger/OpenAPI documentation
- Global exception handling with structured responses
- Serilog logging with console + file output

---

### 2. Application Layer

**Path:** `api/Wanankucha.Api.Application/`

| Component              | Description                                          |
| ---------------------- | ---------------------------------------------------- |
| **Features/Commands/** | CreateUser, LoginUser, RefreshToken                  |
| **Features/Queries/**  | GetAllUsers                                          |
| **Behaviors/**         | LoggingBehavior, ValidationBehavior                  |
| **Abstractions/**      | Service interfaces (ITokenService, IPasswordService) |
| **DTOs/**              | Response DTOs                                        |

**CQRS Implementation:**

- MediatR for command/query dispatching
- FluentValidation for request validation
- Pipeline behaviors for cross-cutting concerns

---

### 3. Domain Layer

**Path:** `api/Wanankucha.Api.Domain/`

| Component         | Description                                                     |
| ----------------- | --------------------------------------------------------------- |
| **Entities/**     | User, Role, UserRole                                            |
| **Common/**       | BaseEntity<TKey>, IEntity                                       |
| **Repositories/** | IUserRepository, IReadRepository, IWriteRepository, IUnitOfWork |

**BaseEntity Properties:**

- `Id` (TKey - generic)
- `CreatedDate` (auto-set on insert)
- `UpdatedDate` (auto-set on update)
- `IsDeleted` (soft delete)

---

### 4. Infrastructure Layer

**Path:** `api/Wanankucha.Api.Infrastructure/`

| Component                    | Description                              |
| ---------------------------- | ---------------------------------------- |
| **Services/TokenService**    | JWT generation, refresh token management |
| **Services/PasswordService** | BCrypt password hashing                  |
| **Options/TokenSettings**    | JWT configuration options                |

---

### 5. Persistence Layer

**Path:** `api/Wanankucha.Api.Persistence/`

| Component                 | Description                          |
| ------------------------- | ------------------------------------ |
| **Contexts/AppDbContext** | EF Core context with auto-timestamps |
| **Repositories/**         | UserRepository implementation        |
| **Configurations/**       | Entity type configurations           |
| **UnitOfWork**            | Transaction management               |

**DbContext Features:**

- Global query filters for soft delete (`IsDeleted`)
- Auto-set `CreatedDate`/`UpdatedDate` timestamps
- Configuration via `ApplyConfigurationsFromAssembly`

---

### 6. Web (Blazor Server)

**Path:** `web/Wanankucha.Web/`

| Component              | Description                      |
| ---------------------- | -------------------------------- |
| **Components/Pages/**  | Login, Register, Dashboard       |
| **Components/Layout/** | MainLayout, AuthLayout           |
| **Auth/**              | JwtAuthenticationStateProvider   |
| **Services/**          | AuthService, TokenStorageService |

**Features:**

- Interactive Server rendering
- JWT authentication with cookie storage
- Polly HTTP resilience (retry, circuit breaker, 30s timeout)
- Serilog logging (project-specific log files)
- Real-time form validation on keystroke

---

### 7. Shared Library

**Path:** `shared/Wanankucha.Shared/`

| Component     | Description                                                  |
| ------------- | ------------------------------------------------------------ |
| **DTOs/**     | LoginRequest, RegisterRequest, TokenDto, RefreshTokenRequest |
| **Wrappers/** | ApiResponse<T>                                               |

---

## Current Architecture Strengths

1. ✅ Clean Architecture with proper layer separation
2. ✅ CQRS pattern with MediatR
3. ✅ FluentValidation pipeline behavior
4. ✅ Repository pattern with UnitOfWork
5. ✅ Soft delete with global query filters
6. ✅ Auto-timestamps on entities
7. ✅ Structured logging with Serilog
8. ✅ HTTP resilience with Polly
9. ✅ JWT authentication with refresh tokens
10. ✅ Global exception handling middleware

---

## Improvement Recommendations

### 🔴 High Priority

#### 1. Add Result Pattern (Replace Exceptions for Flow Control)

**Current:** Business logic errors throw exceptions caught by middleware
**Improvement:** Use Result<T> pattern for expected failures
**Benefits:** Better performance, clearer intent, easier testing

#### 2. Add Health Checks

**Current:** No health check endpoints
**Improvement:** Add `/health` endpoint for load balancers/k8s

#### 3. Add Response Caching/ETag

**Current:** No caching for read queries
**Improvement:** Add output caching for GetAllUsers

---

### 🟡 Medium Priority

#### 4. Add Rate Limiting

**Current:** No rate limiting on auth endpoints
**Improvement:** Protect login/register from brute force

#### 5. Add Background Job Processing

**Current:** No background processing capability
**Improvement:** Add Hangfire/Quartz for email sending, cleanup jobs

#### 6. Add API Versioning

**Current:** No API versioning

#### 7. Add Distributed Caching

**Current:** No caching layer
**Improvement:** Add Redis/MemoryCache for frequently accessed data

---

### 🟢 Low Priority (Nice to Have)

#### 8. Add Request/Response Compression

#### 9. Add Metrics/Observability

- OpenTelemetry for distributed tracing
- Prometheus metrics endpoint

#### 10. Add Email Service

- IEmailService abstraction
- Implement with SendGrid/SMTP

#### 11. Add Password Reset Flow

#### 12. Add Account Lockout

#### 13. Add Audit Logging

#### 14. Web - Add Error Boundary Components

---

## Performance Optimizations

### Database

1. Add indexes on frequently queried columns (Email, Username, RefreshToken)
2. Use compiled queries for hot paths
3. Add database connection pooling

### API

1. Use minimal APIs for simple endpoints
2. Add response compression (Brotli/Gzip)
3. Use System.Text.Json source generators

### Web

1. Add component virtualization for large lists
2. Prerender pages where possible
3. Lazy load components

---

## Security Enhancements

1. Add CSRF protection on auth forms
2. Implement Content Security Policy headers
3. Rotate refresh tokens on each use
4. Add two-factor authentication (TOTP)

---

## Testing Recommendations

Current state: No tests found

**Recommended test pyramid:**

1. **Unit Tests** - Handlers, validators, services
2. **Integration Tests** - Repository, DbContext
3. **API Tests** - Controller endpoints with WebApplicationFactory
4. **UI Tests** - Playwright for Blazor pages
