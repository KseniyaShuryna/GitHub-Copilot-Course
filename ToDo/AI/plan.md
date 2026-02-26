## Plan: Minimal ToDo App with Role-Based Auth (.NET + React, No Deployment)

A concise, production-ready plan for a ToDo MVP: .NET backend (REST API, EF Core, SQL Server) with minimal React frontend. Supports role-based auth (e.g., Admin, User), and basic task CRUD. Focus: speed, clarity, and maintainability. No deployment/infrastructure steps included.

**Steps**

### 1. Backend (.NET API)
- Scaffold new ASP.NET Core Web API project ([ToDo.Api/])
- Add EF Core and SQL Server provider
- Define `ToDo` entity (Id, Title, IsComplete, CreatedAt, UserId)
- Define `User` entity (Id, Username, PasswordHash, Role)
- Create `ToDoDbContext` with DbSets for `ToDo` and `User`
- Implement EF Core migrations and seed initial users/roles
- Implement JWT-based authentication (login endpoint issues JWT)
- Add role-based authorization (e.g., `[Authorize(Roles = "Admin")]`)
- Implement REST endpoints:
  - `GET /api/tasks` (list tasks for user)
  - `POST /api/tasks` (create task)
  - `PUT /api/tasks/{id}/toggle` (toggle complete)
  - `DELETE /api/tasks/{id}` (delete task)
- Secure endpoints: only authenticated users, enforce role checks
- Add minimal error handling and validation

### 2. Frontend (React)
- Scaffold new React app ([todo-ui/])
- Add HTTP client (axios or fetch), JWT storage (localStorage)
- Implement login form (calls API, stores JWT)
- Implement task list UI (fetches tasks, displays, allows toggle/delete)
- Implement add task form
- Show/hide UI elements based on user role (if needed)
- Handle API errors and loading states minimally

**Verification**
- Run backend: verify DB migrations, seed users, API endpoints (Swagger/Postman)
- Run frontend: verify login, task CRUD, role restrictions
- Manual test: login as different roles, check permissions

**Decisions**
- Chose JWT for stateless, simple auth
- Used EF Core migrations for DB setup
- Minimal React for fastest UI delivery
- No deployment or infrastructure steps included