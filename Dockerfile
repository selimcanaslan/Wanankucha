# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY *.sln .
COPY api/Wanankucha.Api/*.csproj ./api/Wanankucha.Api/
COPY api/Wanankucha.Api.Application/*.csproj ./api/Wanankucha.Api.Application/
COPY api/Wanankucha.Api.Domain/*.csproj ./api/Wanankucha.Api.Domain/
COPY api/Wanankucha.Api.Infrastructure/*.csproj ./api/Wanankucha.Api.Infrastructure/
COPY api/Wanankucha.Api.Persistence/*.csproj ./api/Wanankucha.Api.Persistence/
COPY shared/Wanankucha.Shared/*.csproj ./shared/Wanankucha.Shared/
COPY web/Wanankucha.Web/*.csproj ./web/Wanankucha.Web/

# Restore dependencies
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish api/Wanankucha.Api/Wanankucha.Api.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Wanankucha.Api.dll"]
