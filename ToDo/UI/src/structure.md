# Clean Architecture Project Structure

src/
  api/           # API clients, DTOs, and types
  components/    # Reusable UI components
  features/      # Feature modules (auth, tasks, etc.)
  hooks/         # Custom React hooks
  layouts/       # Layout components (protected, public)
  pages/         # Route-level components
  providers/     # Context providers (auth, theme, etc.)
  routes/        # Route definitions and guards
  styles/        # Global and modular styles
  utils/         # Utility functions and helpers
  App.tsx        # Root app component
  main.tsx       # Entry point
