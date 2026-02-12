# 🚀 commerce-core-api

Enterprise-grade **.NET 8 microservices backend platform** built using
Clean Architecture, JWT security, API Gateway routing, and centralized
async logging.

------------------------------------------------------------------------

## 📌 Overview

`commerce-core-api` is a modular microservices backend built with:

-   .NET 9
-   Clean Architecture principles
-   Ocelot API Gateway
-   JWT Authentication
-   Centralized asynchronous logging
-   EF Core (InMemory for development)
-   Serilog structured logging

Designed for scalability, modularity, and SaaS evolution.

------------------------------------------------------------------------

# 🏗 Architecture

                        ┌─────────────────────┐
                        │       Client        │
                        └─────────┬───────────┘
                                  │
                                  ▼
                        ┌─────────────────────┐
                        │     API Gateway     │
                        │       (Ocelot)      │
                        └─────────┬───────────┘
                                  │
            ┌─────────────────────┼─────────────────────┐
            ▼                     ▼                     ▼
    ┌───────────────┐    ┌────────────────┐    ┌────────────────┐
    │ IdentityService│    │  UserService   │    │ CatalogService │
    │  JWT Provider  │    │   User Domain  │    │ Product Domain │
    └───────────────┘    └────────────────┘    └────────────────┘
                                                   │
                                                   ▼
                                           ┌────────────────┐
                                           │  CartService   │
                                           │  Cart Domain   │
                                           └────────────────┘

                      ─────────────────────────────────────────
                          Shared.Logging (Async Channel)
                      ─────────────────────────────────────────

------------------------------------------------------------------------

# 🧱 Microservices

  Service               Responsibility
  --------------------- -----------------------------------
  IdentityService.API   JWT issuance & authentication
  UserService.API       User profiles & management
  CatalogService.API    Product catalog
  CartService.API       Shopping cart domain
  ApiGateway            Central routing
  Shared.Logging        Async centralized logging library

------------------------------------------------------------------------

# 🔐 Authentication Flow

1.  Client authenticates via IdentityService\
2.  JWT token is issued\
3.  Client calls API Gateway with Bearer token\
4.  Gateway routes to target service\
5.  Services validate token

------------------------------------------------------------------------

# 🛠 Technology Stack

-   .NET 8
-   ASP.NET Core
-   Ocelot
-   Serilog
-   EF Core
-   JWT
-   Swagger / OpenAPI

------------------------------------------------------------------------

# ⚙️ Running the Project

``` bash
git clone https://github.com/Loyiso/commerce-core-api.git
cd commerce-core-api
cd UserService.API
dotnet run
```

Swagger available at:

    https://localhost:{port}/swagger

------------------------------------------------------------------------

# 📊 Logging Pipeline

Serilog → FireAndForgetInMemorySink → ChannelLogDispatcher → ILogStore →
EF Core

Non-blocking and async.

------------------------------------------------------------------------
  
# 👨‍💻 Author

**Loyiso Nelani**\
Senior .NET Engineer\
Founder --- Uloyiso Systems Engineering

------------------------------------------------------------------------

MIT License
