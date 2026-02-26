## ToDo API Solution: Backend Architecture Overview

### Overview
The ToDo API solution is structured using a clean, layered architecture that separates concerns across three main projects:

- **ToDo.Api** (API Layer)
- **ToDo.Application** (Application Layer)
- **ToDo.Infrastructure** (Infrastructure Layer)

This design promotes maintainability, testability, and scalability by enforcing clear boundaries between HTTP handling, business logic, and data access.

---

### Project Structure & Responsibilities

#### 1. ToDo.Api (API Layer)
- **Purpose:** Exposes HTTP endpoints, handles requests/responses, manages authentication, and orchestrates the application layer.
- **Responsibilities:**
  - Defines controllers for authentication and ToDo operations.
  - Handles global exception management via middleware.
  - Configures dependency injection and application startup.
  - Reads configuration (e.g., JWT settings).

#### 2. ToDo.Application (Application Layer)
- **Purpose:** Contains core business logic, DTOs, service interfaces, and exception handling.
- **Responsibilities:**
  - Defines service interfaces (e.g., `IAuthService`, `IToDoService`).
  - Implements business logic in service classes.
  - Maps between domain entities and DTOs.
  - Handles application-specific exceptions.

#### 3. ToDo.Infrastructure (Infrastructure Layer)
- **Purpose:** Implements data access, persistence, and integration with external systems.
- **Responsibilities:**
  - Defines Entity Framework Core DbContext and entity models.
  - Implements repositories for data access (e.g., `TaskRepository`).
  - Manages database migrations.
  - Provides infrastructure-level dependency injection.

---

### Layer Separation

- **API Layer** depends on **Application Layer** (never the reverse).
- **Application Layer** depends on **Infrastructure Layer** only via abstractions (interfaces), not concrete implementations.
- **Infrastructure Layer** implements interfaces defined in the Application Layer.

This ensures a strict separation of concerns and allows for easy testing and replacement of infrastructure components.

---

### Authentication Flow (JWT + Refresh Tokens)

1. **User Login:**
   - User submits credentials to the AuthController.
   - Credentials are validated by the `AuthService`.
2. **JWT Issuance:**
   - On successful authentication, a JWT access token and a refresh token are generated.
   - JWT contains user claims and is signed using configured secret.
   - Refresh token is stored in the database (via `RefreshTokenEntity`).
3. **Token Usage:**
   - Client includes JWT in the Authorization header for protected endpoints.
   - Middleware validates JWT and sets the current user context.
4. **Token Refresh:**
   - When the access token expires, the client can use the refresh token to obtain a new JWT.
   - The refresh token is validated against the database and, if valid, a new JWT and refresh token are issued.

---

### Main Services & Their Roles

- **AuthService:** Handles user authentication, registration, JWT and refresh token generation/validation.
- **ToDoService:** Manages ToDo item business logic (CRUD operations, validation, mapping).
- **CurrentUserService:** Provides access to the current authenticated user's context.

---

### Dependency Injection Configuration

- **Startup Configuration:**
  - Each project exposes a `DependencyInjection` static class to register its services.
  - The API project composes all registrations in `Program.cs`.
  - Services, repositories, DbContext, and options are registered with appropriate lifetimes (Scoped, Singleton, etc.).

---

### Request Flow: Controller to Database

1. **HTTP Request:**
   - Received by a controller (e.g., `ToDoController`).
2. **Controller Action:**
   - Validates input and calls the appropriate service (e.g., `ToDoService`).
3. **Service Layer:**
   - Executes business logic, validation, and mapping.
   - Calls repository interfaces for data access.
4. **Repository Layer:**
   - Interacts with the DbContext to query or persist data.
5. **Database:**
   - Data is read from or written to the database.
6. **Response:**
   - Data is mapped to DTOs and returned to the controller, which sends the HTTP response.

---

### Summary Diagram

```mermaid
flowchart TD
    Client -->|HTTP| API[API Layer (Controllers, Middleware)]
    API -->|Service Calls| Application[Application Layer (Services, DTOs, Interfaces)]
    Application -->|Repository Interfaces| Infrastructure[Infrastructure Layer (Repositories, DbContext, Entities)]
    Infrastructure -->|EF Core| Database[(Database)]
```

---

## Quick Start

### Install Dependencies
```bash
dotnet restore
```

### Build the Solution
```bash
dotnet build
```

### Run the API (Development)
```bash
dotnet run --project ToDo.Api/ToDo.Api.csproj
```

### Apply Database Migrations
```bash
dotnet ef database update --project ToDo.Infrastructure/ToDo.Infrastructure.csproj
```

---