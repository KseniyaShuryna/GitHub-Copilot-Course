# REQUEST
Model: GPT-4.1

Mode: Plan

Context: workspace, src/AI/plan.md

Instruction:
@workspace analyze solution. Analyze #file:plan.md.
Propose step by step plan to create frontend side for API

# REQUEST
Model: GPT-4.1

Mode: Agent

Context: workspace

Instruction:
It suggests create-react-app, 
localStorage JWT storage, and lacks details on routing, token refresh, DTO typing, 
state/data layer, and API contract alignment.
Goal: rewrite the plan so it is production-ready and matches the backend contract.
What to fix / clarify:
1) Project scaffolding:
   - Replace create-react-app with Vite + React + TypeScript.
   - Add baseline tooling: ESLint/Prettier, path alias (@/), environment config.
2) API contract alignment:
   - Before UI work, define/verify endpoints, DTOs, auth requirements, error shapes,
     pagination/sorting (if any), and status codes.
   - Explicitly list assumptions; flag anything that must be confirmed in backend.
3) Token strategy and security:
   - Do not default to localStorage without discussing risks.
   - Propose a safe approach based on best practices
   - Specify exact steps for implementing axios interceptors and retry logic.
4) Routing and auth boundaries:
   - Add react-router setup with public/private routes.
   - Define protected layouts, redirects, and "logged out" behavior.
5) Forms and validation:
   - Use react-hook-form + zod (or justify alternatives).
   - Define validation rules and error display approach.
6) Role-based UI:
   - Do not assume claim names.
   - Add a step to verify role claim format in JWT (e.g., role/roles/schema URI),
     then implement feature gating based on confirmed claims.
7) UX and error handling:
   - Standardize loading/empty/error states.
   - Add toasts/notifications, global error boundary, and consistent API error mapping.
Output format (strict):
- Provide a step-by-step plan
- Include a final section: "Open questions to confirm with backend" as a short list.
- Keep it concise and production-oriented. Use TypeScript terminology.

# REQUEST
Model: GPT-4.1

Mode: Agent

Context: workspace

Instruction:
Update the "Token Strategy & Security" section of the frontend plan.
I want a production-ready and secure approach aligned with common
JWT + refresh token best practices.
Required changes:
1) Access token:
   - Store in memory (React context/provider).
   - Optionally mirror in sessionStorage only if reload persistence is required.
   - On app init, restore access token from sessionStorage if present.
   - Always attach access token via axios interceptor.
2) Refresh token:
   - Do NOT store in memory, localStorage, or sessionStorage.
   - Use HttpOnly cookie managed by the backend.
   - Refresh endpoint must rely on cookie automatically sent by browser.
3) Refresh flow:
   - On 401, trigger a single-flight refresh request.
   - Retry original request once after successful refresh.
   - If refresh fails, clear auth state and redirect to login.
   - Prevent infinite retry loops.

# REQUEST
Model: GPT-4.1

Mode: Agent

Context: workspace, src/AI/frontend-plan.md

Instruction:
You are a senior frontend engineer.
Context:
The attached document contains the approved production-ready frontend plan
for the ToDo API. Use it strictly as the implementation source of truth.
Task:
Start implementing the project step by step according to the plan.
Requirements:
- Follow the plan exactly and in order.
- Apply modern React + TypeScript best practices.
- Use clean architecture principles and clear project structure.
- Keep code production-ready, strongly typed, and maintainable.
- Avoid shortcuts or temporary solutions.
- If an implementation detail is unclear, make a reasonable
  best-practice assumption and proceed.
Begin with project scaffolding and foundational setup.
Proceed incrementally.

# REQUEST
Model: GPT-4.1

Mode: Agent

Instruction:
You are a senior frontend engineer.

Use the attached plan as the source of truth. Do not restate it.
Implement the next step: API contract alignment + API layer skeleton.

Create src/api/ with:
- types.ts (DTOs + ApiError { error: string; details?: string })
- client.ts (axios instance, baseURL from env, withCredentials enabled)
- auth.api.ts (login, register, refresh, logout placeholders)
- tasks.api.ts (CRUD placeholders)

Keep everything strongly typed. No UI changes yet.
Output: what changed, files created, and commands to run.

# REQUEST
Model: Claude Sonnet 4.5

Mode: Agent

Context: src/UI, src/AI/frontend-plan.md

Instruction:
Create these essential components:
Login/Register Forms with react-hook-form + zod validation
Task List Component with CRUD operations
Protected Route Component
Role-based UI Components

# REQUEST
Model: GPT-4.1

Mode: Agent

Context: src/UI

Instruction:
Create a reusable presentational NavBar component.
Then create a MainLayout component that uses NavBar.
Refactor routing to use MainLayout with nested routes.

# REQUEST
Model: GPT-4.1

Mode: Agent

Context: Screenshot

Instruction:
Analyze the current layout and refactor the page structure.
Problems:
- Tasks form is poorly aligned
- Navigation items are not properly spaced
- Layout does not look clean or balanced
