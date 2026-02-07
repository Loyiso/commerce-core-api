# IdentityService.API (Repository + Service Pattern)

A minimal Identity/Auth Web API using:
- .NET 8
- EF Core InMemory (mock DB)
- JWT access tokens
- Refresh tokens with rotation (stored hashed)

## Run
```bash
dotnet restore
dotnet run
```

Swagger (dev): `https://localhost:<port>/swagger`

## Seeded users
- admin: `admin@demo.local` / `Admin@12345`
- user:  `user@demo.local`  / `User@12345`

## Endpoints
- POST `/auth/register`
- POST `/auth/login`
- POST `/auth/refresh`
- POST `/auth/logout`
- GET  `/auth/me` (Bearer token)

## Important
Update `appsettings.json` -> `Jwt:SigningKey` to a strong secret (>= 32 chars).
