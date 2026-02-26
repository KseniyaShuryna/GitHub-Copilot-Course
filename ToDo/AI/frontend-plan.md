# Production-Ready Frontend Plan for ToDo API

## 1. Project Scaffolding & Tooling
- Scaffold with Vite: `npm create vite@latest todo-ui -- --template react-ts`
- Install and configure:
  - ESLint + Prettier for code quality and formatting
  - Path aliasing in `tsconfig.json` (`@/` → `src/`)
  - Environment config via `.env` and `import.meta.env`
  - (Optional) Husky + lint-staged for pre-commit checks

## 2. API Contract Alignment
- Define all endpoint URLs, DTOs, and error shapes in `src/api/types.ts`:
  - Auth: login, register, refresh (rotating refresh token via HttpOnly cookie), logout
  - Tasks: CRUD endpoints, DTOs
  - Error: `{ error: string, details?: string }`
- Document status codes and payloads for each endpoint

## 3. Token Strategy & Security (Production-Ready)

### Access Token
- Store in memory (React context/provider) for runtime use.
- Optionally mirror in sessionStorage only if reload persistence is required.
- On app initialization, restore access token from sessionStorage if present.
- Always attach access token to Authorization header via axios interceptor.

### Refresh Token
- Never store refresh token in memory, localStorage, or sessionStorage.
- Backend issues refresh token as an HttpOnly, Secure cookie with an appropriate SameSite policy (e.g., `SameSite=Lax` or `SameSite=Strict`) to mitigate CSRF and man-in-the-middle risks.
- Refresh endpoint relies on browser to send the cookie automatically; frontend does not access or manage the refresh token directly.

### Refresh Flow
- On receiving a 401 from the API, trigger a single-flight refresh request (prevent concurrent refreshes).
- If refresh succeeds, update access token in memory/sessionStorage and retry the original request once.
- If refresh fails, clear all auth state and redirect user to login.
- Ensure logic prevents infinite retry loops on repeated 401s.

## 4. Routing & Auth Boundaries
- Use `react-router-dom` for routing
- Define public (login, register) and private (tasks) routes
- Implement protected route components/layouts
- Redirect to login on logout or unauthorized access

## 5. Forms & Validation
- Use `react-hook-form` with `zod` for schema-based validation
- Define validation schemas for login, registration, and task forms
- Display API and validation errors in forms

## 6. Role-Based UI
- After login, decode JWT and extract `role` claim (string: "User" or "Admin")
- Implement feature gating and conditional rendering based on `role`
- Use context/provider to expose user info and role throughout the app

## 7. UX & Error Handling
- Standardize loading, empty, and error states for all async UI
- Implement global error boundary for React errors
- Use a notification/toast system (e.g., `react-toastify`) for API and auth errors
- Map backend error responses to user-friendly messages