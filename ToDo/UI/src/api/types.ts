// DTOs and API error types

export interface ApiError {
  error: string;
  details?: string;
}

// Auth
export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken?: string; // backend may return, but frontend should ignore
  email?: string;
  role?: 'User' | 'Admin';
  // refresh token is handled via HttpOnly cookie
}

// User
export interface UserDto {
  id: number;
  email: string;
  role: 'User' | 'Admin';
}

// Task
export interface TaskDto {
  id: number;
  title: string;
  isComplete: boolean;
  createdAt: string;
  // userId is not returned by backend
}

export interface CreateTaskRequest {
  title: string;
}

export interface UpdateTaskRequest {
  title?: string;
  isComplete?: boolean;
}
