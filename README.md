# Task Management API

A backend REST API for managing Projects and Tasks, built with **.NET 9**, **Clean Architecture**, and production-level coding practices.

🔗 **Live API (Swagger):** [https://taskmanagementsapi.runasp.net/swagger/index.html](https://taskmanagementsapi.runasp.net/swagger/index.html)

---

## Tech Stack

| Category | Technology |
|---|---|
| Framework | ASP.NET Core Web API (.NET 9) |
| Architecture | Clean Architecture |
| ORM | Entity Framework Core |
| Database | SQL Server |
| Authentication | JWT Bearer + Refresh Tokens |
| Messaging | MediatR (CQRS) |
| Validation | FluentValidation |
| Testing | xUnit + Moq |
| Containerization | Docker + Docker Compose |
| API Design | Versioning, Generic Response Wrapper, Global Exception Handling |

---

## Architecture Overview

```
TaskManagement/
├── TaskManagement.API/              → Controllers, Middleware, Extensions
├── TaskManagement.Application/      → CQRS Handlers, DTOs, Interfaces, Validators
├── TaskManagement.Domain/           → Entities, Enums
├── TaskManagement.Infastructure/    → EF Core, Repositories, JWT, UnitOfWork
└── TaskManagement.UnitTests/        → Unit Tests (xUnit + Moq)
```

Dependency direction — each layer only depends inward:
```
API → Application → Domain
Infrastructure → Application → Domain
```

---

## Architecture & Design Principles

### Clean Architecture
Strict separation of concerns across 4 layers. Business logic lives entirely in the Application layer and has zero dependency on Infrastructure or the Web framework.

### SOLID Principles
- **S** — Every class has a single responsibility (handlers, validators, services are all separate)
- **O** — New features are added via new commands/queries, not by modifying existing ones
- **L** — Repositories and services are substitutable through interfaces
- **I** — Interfaces are small and focused (`IProjectRepository`, `ITaskRepository`, `ICacheService`)
- **D** — All dependencies injected via interfaces, never concrete implementations

### CQRS + MediatR
Commands (write) and Queries (read) are fully separated. Every operation is its own handler class.

### Repository Pattern + Unit of Work
All database access is abstracted behind repository interfaces. `IUnitOfWork` wraps all repositories in a single transaction scope — one `SaveChangesAsync()` call commits everything atomically.

### Global Exception Handling
A single `GlobalExceptionMiddleware` catches all unhandled exceptions and returns consistent error responses. No try/catch scattered across controllers.

### Generic Response Wrapper
Every endpoint returns the same structure:
```json
{
  "success": true,
  "message": "Operation completed successfully.",
  "data": { }
}
```

---

## Features

### Authentication
- **Register** — create a new account with role (Admin / User)
- **Login** — returns JWT access token + refresh token
- **Refresh Token** — get a new access token without re-login
- Role-based Authorization (Admin / User)

### Projects
- Create, update, delete your own projects
- Get all projects / get projects by owner / get by ID
- Each project belongs to the authenticated user

### Tasks
- Create and delete tasks *(Admin only)*
- Get tasks by project
- Update task status *(any authenticated user)*

---

## Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local instance or Docker)

### 1. Clone the repository
```bash
git clone https://github.com/your-username/TaskManagement.git
cd TaskManagement
```

### 2. Configure the database

Update `appsettings.json` inside `TaskManagement.API`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=TaskManagementDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "SecretKey": "YourSecretKeyHereAtLeast32Characters!",
    "Issuer": "TaskManagementAPI",
    "Audience": "TaskManagementClient",
    "ExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7
  }
}
```

### 3. Apply migrations
```bash
cd TaskManagement.API
dotnet ef database update
```

### 4. Run
```bash
dotnet run
```

Swagger available at: `http://localhost:5000/swagger`

---

## Run with Docker

```bash
# Build and start API + SQL Server
docker-compose up --build
```

Swagger available at: `http://localhost:5000/swagger`

SQL Server available at: `localhost,1433` (user: `sa`)

---

## Run Unit Tests

```bash
dotnet test
```

Tests cover: query handlers, command handlers, and validation logic using **xUnit** and **Moq**.

---

## API Endpoints

### Auth
| Method | Endpoint | Access |
|---|---|---|
| POST | `/api/Auth/register` | Public |
| POST | `/api/Auth/login` | Public |

### Projects
| Method | Endpoint | Access |
|---|---|---|
| GET | `/api/Projects/Get-All-Projects` | Authenticated |
| GET | `/api/Projects/Get-Projects-By-Owner` | Authenticated |
| GET | `/api/Projects/Get-Project-By-Id/{id}` | Authenticated |
| POST | `/api/Projects/Create-Project` | Authenticated |
| PUT | `/api/Projects/Update-Project/{id}` | Authenticated |
| DELETE | `/api/Projects/Delete/Project{id}` | Authenticated |

### Tasks
| Method | Endpoint | Access |
|---|---|---|
| GET | `/api/Tasks/Get-Task-GetBy-ProjectId/{projectId}` | Authenticated |
| POST | `/api/Tasks/Create-Task` | Admin only |
| PATCH | `/api/Tasks/Update/{taskId}/status` | Authenticated |
| DELETE | `/api/Tasks/Delete-Task/{taskId}` | Admin only |

---

## Models

### Task
| Field | Type | Values |
|---|---|---|
| Status | Enum | `Todo` · `InProgress` · `Done` |
| Priority | Enum | `Low` · `Medium` · `High` |
| DueDate | DateTime | — |

### Project
| Field | Type |
|---|---|
| Id | int |
| Name | string |
| Description | string |
| CreatedAt | DateTime |

---

## Code Quality

- **Naming Conventions** — PascalCase for classes/methods, camelCase for variables, consistent across all layers
- **Clean Code** — small focused methods, meaningful names, no magic strings or numbers
- **DTO Usage** — domain entities never exposed directly; all responses go through DTOs
- **FluentValidation** — all input validated in the Application layer before reaching handlers
- **No business logic in controllers** — controllers only receive requests and delegate to MediatR
- **Dependency Injection** — everything registered and resolved through the DI container

---

## Evaluation Checklist

| Requirement | Status |
|---|---|
| Clean Architecture | ✅ |
| SOLID Principles | ✅ |
| CQRS + MediatR | ✅ |
| Repository Pattern + Unit of Work | ✅ |
| JWT Authentication + Refresh Token | ✅ |
| Role-based Authorization | ✅ |
| FluentValidation | ✅ |
| Global Exception Handling | ✅ |
| Generic Response Wrapper | ✅ |
| DTO Usage | ✅ |
| Unit Tests | ✅ |
| Docker | ✅ |
| Naming Conventions | ✅ |
| EF Core + SQL Server | ✅ |
| Database Migrations | ✅ |
| Swagger Documentation | ✅ |
