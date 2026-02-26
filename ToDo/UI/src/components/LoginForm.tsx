import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { loginSchema, type LoginFormData } from '../schemas/auth.schema';
import { authApi } from '../api/auth.api';
import { useAuth } from '../providers/AuthProvider';
import type { ApiError } from '../api/types';
import { Link } from 'react-router-dom';

export const LoginForm = () => {
  const navigate = useNavigate();
  const { setAuth } = useAuth();
  const [apiError, setApiError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
  });

  const onSubmit = async (data: LoginFormData) => {
    setApiError(null);
    setIsLoading(true);

    try {
      const response = await authApi.login(data);
      
      // Decode user from token and set auth
      if (response.accessToken) {
        const user = {
          id: 0, // Will be populated from token by AuthProvider
          email: response.email || data.email,
          role: (response.role || 'User') as 'User' | 'Admin',
        };
        setAuth(user, response.accessToken);
        navigate('/', { replace: true });
      }
    } catch (error) {
      const apiErr = error as ApiError;
      setApiError(apiErr.error || 'Login failed');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="auth-form">
      <h2>Login</h2>
      
      {apiError && (
        <div className="error-alert" role="alert">
          {apiError}
        </div>
      )}

      <form onSubmit={handleSubmit(onSubmit)}>
        <div className="form-group">
          <label htmlFor="email">Email</label>
          <input
            id="email"
            type="email"
            {...register('email')}
            disabled={isLoading}
            aria-invalid={errors.email ? 'true' : 'false'}
          />
          {errors.email && (
            <span className="error-message" role="alert">
              {errors.email.message}
            </span>
          )}
        </div>

        <div className="form-group">
          <label htmlFor="password">Password</label>
          <input
            id="password"
            type="password"
            {...register('password')}
            disabled={isLoading}
            aria-invalid={errors.password ? 'true' : 'false'}
          />
          {errors.password && (
            <span className="error-message" role="alert">
              {errors.password.message}
            </span>
          )}
        </div>

        <button type="submit" disabled={isLoading}>
          {isLoading ? 'Logging in...' : 'Login'}
        </button>
      </form>

      <p className="auth-link">
        Don't have an account?{' '}
        <Link to="/register">Register here</Link>
      </p>
    </div>
  );
};
