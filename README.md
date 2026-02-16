# 🚀 commerce-core-api

Enterprise-grade **.NET 8 microservices backend platform** built using
Clean Architecture, JWT security, API Gateway routing, and centralized
async logging.

------------------------------------------------------------------------

## 📌 Overview

`commerce-core-api` is a modular microservices backend built with:

-   .NET 8
-   Clean Architecture principles
-   Ocelot API Gateway
-   JWT Authentication
-   Centralized asynchronous logging service
-   EF Core (InMemory for development)
-   Serilog structured logging
-   RESTful microservice communication

Designed for scalability, modularity, and SaaS evolution.

------------------------------------------------------------------------

# 🏗 Architecture

Client\
↓\
API Gateway (Ocelot)\
↓\
IdentityService \| UserService \| CatalogService \| CartService\
↓\
Centralized LoggingService (Async REST Logging API)

------------------------------------------------------------------------

# 🧱 Microservices

  -----------------------------------------------------------------------
  Service                     Responsibility
  --------------------------- -------------------------------------------
  IdentityService.API         JWT issuance, authentication, refresh
                              tokens

  UserService.API             User profiles & management

  CatalogService.API          Product catalog (search, sort, paging)

  CartService.API             Shopping cart domain

  ApiGateway                  Central routing & aggregation

  LoggingService.API          Centralized async logging (EF InMemory +
                              Serilog)
  -----------------------------------------------------------------------

------------------------------------------------------------------------

# 🔐 Authentication Flow

1.  Client authenticates via **IdentityService**
2.  JWT token is issued
3.  Client calls **API Gateway** with Bearer token
4.  Gateway routes to target microservice
5.  Services validate JWT token

------------------------------------------------------------------------

# 📊 Centralized Logging Architecture

All services log using:

-   Local ILogger (structured logging)
-   Async REST call to LoggingService.API
-   Fire-and-forget logging (non-blocking)
-   EF Core InMemory log storage (development)
-   Serilog integration

Logging flow:

Service → HttpClient → LoggingService.API → EF Core InMemory DB

Logging does NOT block request execution.

------------------------------------------------------------------------

# 🛠 Technology Stack

-   .NET 8
-   ASP.NET Core Web API
-   Ocelot API Gateway
-   Serilog
-   EF Core (InMemory)
-   JWT Authentication
-   Swagger / OpenAPI

------------------------------------------------------------------------

# ⚙️ Running the Project

Clone repository:

``` bash
git clone https://github.com/Loyiso/commerce-core-api.git
cd commerce-core-api
```

Run services individually:

``` bash
cd IdentityService.API
dotnet run
```

``` bash
cd UserService.API
dotnet run
```

``` bash
cd CatalogService.API
dotnet run
```

``` bash
cd CartService.API
dotnet run
```

``` bash
cd LoggingService.API
dotnet run
```

Swagger available at:

    https://localhost:{port}/swagger

------------------------------------------------------------------------

# 📦 Development Notes

-   Uses EF Core InMemory for local development
-   Paging implemented using Skip/Take + metadata headers
-   LoggingService is async and non-blocking
-   Microservices communicate via REST
-   Designed to evolve into production with SQL Server/Postgres +
    distributed logging (Loki / ELK)

------------------------------------------------------------------------

# 👨‍💻 Author

**Loyiso Nelani**\
Senior .NET Engineer\
Founder --- Uloyiso Systems Engineering

------------------------------------------------------------------------

MIT License
