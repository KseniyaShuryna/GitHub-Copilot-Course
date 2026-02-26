# Frontend Architecture Overview

## Project Structure & Folder Responsibilities

The UI project is organized for scalability and clarity:

- **src/api/**: API integration layer (axios client, endpoints, types)
- **src/components/**: Reusable UI components (forms, layout, navigation, guards)
- **src/pages/**: Route-level pages (Login, Register, Tasks)
- **src/providers/**: Context providers for global state (e.g., AuthProvider)
- **src/schemas/**: Form validation schemas (zod)
- **src/assets/**: Static assets (images, icons)
- **src/styles/**: CSS modules for scoped styling
- **App.tsx / main.tsx**: App bootstrap, routing, provider setup

## Routing Architecture

Uses **react-router-dom** for client-side routing:

- **App.tsx** configures routes with nested structure
- **ProtectedRoute** and **RoleGuard** components enforce authentication and role-based access
- Route pages (Login, Register, Tasks) are mapped in **src/pages/**
- Layouts (MainLayout, NavBar) wrap protected routes for consistent UI

## Authentication Flow

- **JWT access token** is used for API authorization
- **Refresh token** is managed via HttpOnly cookie for security
- **AuthProvider** tracks user state, token, and handles login/logout/refresh
- On login/register, tokens are received and stored; refresh logic is triggered on token expiry
- Protected routes require valid authentication; role checks are enforced via **RoleGuard**

## API Integration Layer

- **src/api/client.ts**: Configures axios instance with base URL, interceptors
- **Interceptors**: Attach access token to requests, handle 401 errors, trigger token refresh
- **Error handling**: Centralized in interceptors; errors are surfaced to UI as needed
- **tasks.api.ts / auth.api.ts**: Encapsulate API calls for tasks and authentication
- **types.ts**: Shared DTOs and response types

## State Management Approach

- **React Context** via **AuthProvider** for authentication state
- Custom hooks (e.g., useAuth) expose context values and actions
- Local state for forms and UI components; global state only for auth/session

## Form Handling & Validation

- **react-hook-form** for performant form state management
- **zod** schemas for declarative validation (see **src/schemas/**)
- Forms (LoginForm, RegisterForm, TaskForm) use schema-driven validation and error display

## Role-Based UI & Protected Routes

- **RoleGuard** component restricts access based on user role
- **ProtectedRoute** ensures authentication before rendering children
- UI adapts to user role (e.g., admin vs user) for navigation and actions

## Layout Structure

- **MainLayout**: Wraps protected pages, provides consistent structure
- **NavBar**: Dynamic navigation based on auth state and role
- **Nested routes**: Enable modular page layouts and route protection

## Quick Start

### Install Dependencies
```bash
npm install
```

### Run Development Server
```bash
npm run dev
```

### Build for Production
```bash
npm run build
```

### Preview Production Build
```bash
npm run preview
```

---

## Backend API Integration

This frontend integrates with the ToDo API backend. For endpoint details and integration requirements, see:
- [../API/backend-requests.md](../API/backend-requests.md)
- [../API/backend-responses.md](../API/backend-responses.md)

---


