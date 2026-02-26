# REQUEST
Model: GPT-4.1

Mode: Plan

Instruction:
You are assisting a .NET developer building a fast MVP.

Create a minimal but production-ready implementation plan for a simple ToDo application with a small UI + backend API.

Context: The app must support basic task management: list tasks, create task, toggle complete, delete. Backend exposes a REST API with a relational database SQL Server via EF Core. Add simple structured logging. Add role-based authorization with two roles: Admin and User.
Frontend is a very small UI that calls the API: minimal React. Goal: fastest implementation with clean structure. 

Output format: concise, structured plan (no code) with concrete artifacts and steps.

# REQUEST
Model: GPT-4.1

Mode: Agent

Context: workspace, plan.md 

Instruction:
@workspace You are a senior .NET developer. 
Create a new C#/.NET solution in src\ToDo according following #file:plan.md 
Solution name: ToDo
Architecture: Layered Architecture
Environment:
- .NET Web API project (ASP.NET Core)
- Use C#
- Use EF Core
- Use SQL Server
- Use JWT authentication
- Architecture layers: API, Application, Infrastructure

# REQUEST
Model: GPT-4.1

Mode: Agent

Context: workspace

Instruction:
@workspace You are a senior .NET developer. 
Create a new C#/.NET solution in C:\dev\Mastery 2025.07 - Kseniya Shuryna\ToDo according following #file:plan.md 
Solution name: ToDo
Architecture: Layered Architecture
Environment:
- .NET Web API project (ASP.NET Core)
- Use C#
- Use EF Core
- Use SQL Server
- Use JWT authentication
- Architecture layers: API, Application, Infrastructure

# REQUEST
Model: GPT-4.1

Mode: Agent

Context: workspace

Instruction:
@workspace Initial Folder Structure Example according plan

# REQUEST
Model: GPT-4.1

Mode: Agent

Context: workspace

Instruction:
create enum to store roles.
Implement loging flow with JWT-based authentication (register, login, refresh token).

# REQUEST
Model: GPT-4.1

Mode: Agent

Context: workspace

Instruction:
@workspace  Analyze project. Implement token rotation relating best practises

# REQUEST
Model: GPT-4.1

Mode: Agent

Context: workspace

Instruction:
Add JWT settings to appsettings.Development.json.
Create a strongly-typed JwtOptions class.
Bind configuration section to JwtOptions using builder.Services.Configure<JwtOptions>() in Program.cs

# REQUEST
Model: GPT-4.1

Mode: Agent

Context: workspace

Instruction:
@workspace Analyze the entire solution. 
Create custom exceptions.
Create a global Exception Handling Middleware to handle these exceptions properly (map to correct HTTP status codes, return consistent error response model).
Follow clean architecture and best practices.

# REQUEST
Model: GPT-4.1

Mode: Agent

Context: workspace, ToDoService.cs

Instruction:
Analyze #file:ToDoService.cs 
ropose realistic business constraints and validation rules following best practices.
Define field limits (length, format, language restrictions), business invariants, and error handling strategy.
Justify each rule and specify the proper layer for enforcement.

# REQUEST
Model: GPT-4.1

Mode: Agent

Context: workspace, AuthService.cs, ToDoService.cs

Instruction:
@workspace analyze implementation of #file:AuthService.cs, #file:ToDoService.cs.
Create 2 controllers according services in the API layer.
For each public service method create one HTTP endpoint.
Apply REST conventions and proper HTTP verbs.
Use best practices 

# REQUEST
Model: GPT-4.1

Mode: Agent

Context: workspace, ToDoService.cs, ToDoController.cs

Instruction:
@workspace Replace the controller GetUserId() helper with a reusable service.
Create ICurrentUserService in Application and an implementation that
uses IHttpContextAccessor to read ClaimTypes.NameIdentifier.
Expose int GetUserId().
Register the service in DI.
Remove GetUserId(). Implement ICurrentUserService only in #file:ToDoService.cs, not #file:ToDoController.cs 

# REQUEST
Model: GPT-4.1

Mode: Agent

Context: workspace, appsettings.Development.json 

Instruction:
Act as a senior .NET backend security engineer.
Generate a secure JWT configuration including Key, Issuer, and Audience.
The project uses JWT Bearer authentication with symmetric signing 
(HMAC-SHA256). Save result in #file:appsettings.Development.json 


# REQUEST
Model: GPT-4.1

Mode: Agent

Context: workspace

Instruction:
@workspace analyze implementation of #file:AuthService.cs, #file:ToDoService.cs.
Create 2 controllers according services in the API layer.
For each public service method create one HTTP endpoint.
Apply REST conventions and proper HTTP verbs.
Use best practices 


# REQUEST
Model: GPT-4.1

Mode: Agent

Context: workspace

Instruction:
@workspace analyze #file:AuthController.cs , #file:AuthService.cs.
Create logout endpoint and service method to invalidate refresh tokens

# REQUEST
Model: GPT-4.1

Mode: Agent

Context: workspace

Instruction:
@workspace You are a senior ASP.NET Core backend developer working in a Clean
Architecture solution. 
Configure CORS properly so that requests from the frontend origin
are allowed while keeping the configuration secure.
Requirements:
- Do not allow AllowAnyOrigin in production mode.
- Use a named CORS policy.
- Read the allowed origin from configuration (appsettings).
- Apply the policy globally.
- Show all required code changes.
- Do not modify unrelated logic.

Output:
- Updated Program.cs configuration.
- Example appsettings.json section.

