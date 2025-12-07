# Wanankucha

A .NET 8 Web API application built with Clean Architecture principles.

## 🏗️ Project Structure

The solution follows Clean Architecture with the following layers:

```
Wanankucha/
├── Wanankucha.Api/              # Presentation Layer (Controllers, Middlewares)
├── Wanankucha.Application/      # Application Layer (Use Cases, DTOs, Services)
├── Wanankucha.Domain/           # Domain Layer (Entities, Repository Interfaces)
├── Wanankucha.Infrastructure/   # Infrastructure Layer (External Services)
└── Wanankucha.Persistence/      # Persistence Layer (Database, EF Core, Repositories)
```

## 🛠️ Technologies

- **.NET 8** - Framework
- **Entity Framework Core 8** - ORM
- **PostgreSQL** - Database
- **MediatR** - CQRS Pattern
- **FluentValidation** - Request Validation
- **JWT Authentication** - Security
- **Serilog** - Structured Logging
- **Swagger/OpenAPI** - API Documentation

## 🏛️ Architecture Patterns

### Unit of Work Pattern

The project implements the Unit of Work pattern for centralized transaction management:

```
IUnitOfWork (Domain)
    └── UnitOfWork (Persistence)
            ├── SaveChangesAsync()
            ├── BeginTransactionAsync()
            ├── CommitTransactionAsync()
            └── RollbackTransactionAsync()
```

### Repository Pattern

Generic and entity-specific repositories for data access:

```
Domain/Repositories/
├── IRepository<T>           # Base marker interface
├── IReadRepository<T>       # Read operations (GetAll, GetWhere, GetById)
├── IWriteRepository<T>      # Write operations (Add, Update, Remove)
├── IUnitOfWork              # Transaction management
└── IUserRepository          # User-specific operations

Persistence/Repositories/
├── ReadRepository<T>        # Generic read implementation
├── WriteRepository<T>       # Generic write implementation
└── UserRepository           # User-specific implementation
```

### CQRS with MediatR

Commands and Queries are separated using MediatR:

```
Application/Features/
├── Commands/
│   └── AppUser/
│       ├── CreateUser/
│       ├── LoginUser/
│       └── RefreshToken/
└── Queries/
    └── AppUser/
        └── GetAllUsers/
```

### Validation Pipeline

FluentValidation integrated as MediatR pipeline behavior for automatic request validation.

## 📁 Layer Responsibilities

| Layer              | Responsibility                                                         |
| ------------------ | ---------------------------------------------------------------------- |
| **Api**            | HTTP endpoints, request/response handling, global exception middleware |
| **Application**    | Business logic, use cases, DTOs, service interfaces, MediatR handlers  |
| **Domain**         | Core entities, domain logic, repository interfaces, IUnitOfWork        |
| **Infrastructure** | Token service, password hashing (BCrypt)                               |
| **Persistence**    | DbContext, migrations, repository implementations, UnitOfWork          |

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
   dotnet ef database update --project Wanankucha.Persistence --startup-project Wanankucha.Api
   ```

4. Run the application:
   ```bash
   dotnet run --project Wanankucha.Api
   ```

### Configuration

Update `appsettings.json` with your configuration:

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
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    }
  }
}
```

## 📚 API Documentation

Once the application is running, access Swagger UI at:

```
https://localhost:<port>/swagger
```

### Available Endpoints

| Method | Endpoint                 | Description               | Auth Required |
| ------ | ------------------------ | ------------------------- | ------------- |
| POST   | `/api/Users`             | Create a new user         | No            |
| GET    | `/api/Users`             | Get all users (paginated) | Yes           |
| POST   | `/api/Auth/Login`        | User login                | No            |
| POST   | `/api/Auth/RefreshToken` | Refresh JWT token         | No            |

## 📝 Logging

The application uses Serilog for structured logging:

- **Console Output**: All logs at Information level and above
- **File Output**: JSON formatted logs in `logs/` folder with daily rolling
- **Startup Logging**: Application startup information is logged automatically

## 🔐 Authentication

JWT Bearer token authentication with:

- Access Token (configurable expiration)
- Refresh Token (7 days validity)
- BCrypt password hashing

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License.
