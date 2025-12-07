# Wanankucha

A .NET 8 Web API application built with Clean Architecture principles.

## 🏗️ Project Structure

The solution follows Clean Architecture with the following layers:

```
Wanankucha/
├── Wanankucha.Api/              # Presentation Layer (Controllers, Middlewares)
├── Wanankucha.Application/      # Application Layer (Use Cases, DTOs, Services)
├── Wanankucha.Domain/           # Domain Layer (Entities, Repositories Interfaces)
├── Wanankucha.Infrastructure/   # Infrastructure Layer (External Services)
└── Wanankucha.Persistence/      # Persistence Layer (Database, EF Core)
```

## 🛠️ Technologies

- **.NET 8** - Framework
- **Entity Framework Core 8** - ORM
- **JWT Authentication** - Security
- **Serilog** - Logging
- **Swagger/OpenAPI** - API Documentation

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- A database (configure connection string in `appsettings.json`)

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
    "DefaultConnection": "your-connection-string"
  },
  "Jwt": {
    "Key": "your-secret-key",
    "Issuer": "your-issuer",
    "Audience": "your-audience"
  }
}
```

## 📚 API Documentation

Once the application is running, access Swagger UI at:

```
https://localhost:<port>/swagger
```

## 📁 Project Layers

| Layer              | Responsibility                                           |
| ------------------ | -------------------------------------------------------- |
| **Api**            | HTTP endpoints, request/response handling, middleware    |
| **Application**    | Business logic, use cases, DTOs, service interfaces      |
| **Domain**         | Core entities, domain logic, repository interfaces       |
| **Infrastructure** | External service implementations                         |
| **Persistence**    | Database context, migrations, repository implementations |

## 📝 Logging

The application uses Serilog for structured logging. Logs are written to the `logs/` folder.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License.
