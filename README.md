# Emp API

Production-ready ASP.NET Core (.NET 9) Web API for managing employees (`Emp`) with EF Core + SQLite, JWT login, validation, Swagger, unit/integration tests, GitHub Actions and Docker.

## Run locally
```bash
dotnet run --project src/EmpApi.Api --environment Development
```
Swagger UI: `/swagger`. Dev credentials (Development only): `admin` / `Admin@123`.

## Auth
`POST /api/auth/login` with `{ "username": "...", "password": "..." }` returns a JWT. Send it in the `Authorization` header using the `Bearer` scheme followed by the token.

## Endpoints (require JWT)
| Method | Route | Description |
|---|---|---|
| GET | `/api/emps?page=1&pageSize=20` | List |
| GET | `/api/emps/{id}` | Get |
| POST | `/api/emps` | Create |
| PUT | `/api/emps/{id}` | Update |
| DELETE | `/api/emps/{id}` | Delete |
| GET | `/health` | Health check (anonymous) |

## Configuration
Set via environment variables in production: `Jwt__Key` (>=32 chars, required), `AdminUser__Username`, `AdminUser__Password` (required to enable login), `ConnectionStrings__Default`.

## Tests
```bash
dotnet test
```

## Docker
```bash
JWT_KEY=<32+ chars> ADMIN_PASSWORD=<secret> docker compose up --build
```
API at http://localhost:8080/swagger; SQLite data persisted in the `empdata` volume.

## CI
`.github/workflows/ci.yml` builds, tests, and builds the Docker image on push/PR.
