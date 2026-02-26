import React, { createContext, useContext, useEffect, useRef, useState } from 'react';
import type { UserDto, AuthResponse } from '../api/types';
import { authApi } from '../api/auth.api';
import api from '../api/client';
import type { InternalAxiosRequestConfig } from 'axios';
import { useNavigate } from 'react-router-dom';

interface AuthContextValue {
  user: UserDto | null;
  accessToken: string | null;
  setAuth: (user: UserDto, accessToken: string) => void;
  clearAuth: () => void;
  isLoading: boolean;
}

interface RetryAxiosRequestConfig extends InternalAxiosRequestConfig {
  _retry?: boolean;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

const ACCESS_TOKEN_KEY = 'accessToken';

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<UserDto | null>(null);
  const [accessToken, setAccessToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const refreshInFlight = useRef<Promise<string | null> | null>(null);
  const navigate = useNavigate();

  // Restore access token from sessionStorage if present
  useEffect(() => {
    const token = sessionStorage.getItem(ACCESS_TOKEN_KEY);
    if (token) {
      setAccessToken(token);
      // Optionally decode user info from JWT here
      setUser(decodeUserFromToken(token));
    }
    setIsLoading(false);
  }, []);

  // Attach access token to axios
  useEffect(() => {
    const interceptor = api.interceptors.request.use((config: InternalAxiosRequestConfig) => {
      if (accessToken) {
        config.headers = config.headers || {};
        config.headers['Authorization'] = `Bearer ${accessToken}`;
      }
      return config;
    });
    return () => api.interceptors.request.eject(interceptor);
  }, [accessToken]);

  // Axios response interceptor for refresh flow
  useEffect(() => {
    const interceptor = api.interceptors.response.use(
      (response) => response,
      async (error) => {
        const originalRequest = error?.config as RetryAxiosRequestConfig | undefined;

        const shouldTryRefresh =
          error.response?.status === 401 &&
          !!originalRequest &&
          !originalRequest._retry &&
          !!accessToken;

        if (!shouldTryRefresh) return Promise.reject(error);

        originalRequest._retry = true;

        const newToken = await singleFlightRefresh();

        if (!newToken) {
          clearAuth();
          navigate('/login', { replace: true });
          return Promise.reject(error);
        }

        originalRequest.headers = originalRequest.headers ?? {};
        originalRequest.headers['Authorization'] = `Bearer ${newToken}`;
        
        return api(originalRequest);
      },
    );
    return () => api.interceptors.response.eject(interceptor);
  }, [accessToken, navigate]);

  function setAuth(user: UserDto, token: string) {
    setUser(user);
    setAccessToken(token);
    sessionStorage.setItem(ACCESS_TOKEN_KEY, token);
  }

  function clearAuth() {
    setUser(null);
    setAccessToken(null);
    sessionStorage.removeItem(ACCESS_TOKEN_KEY);
  }

  // Single-flight refresh logic
  async function singleFlightRefresh(): Promise<string | null> {
    if (!refreshInFlight.current) {
      refreshInFlight.current = (async () => {
        try {
          const res: AuthResponse = await authApi.refresh();
          setAccessToken(res.accessToken);
          sessionStorage.setItem(ACCESS_TOKEN_KEY, res.accessToken);
          setUser(decodeUserFromToken(res.accessToken));
          return res.accessToken;
        } catch {
          clearAuth();
          return null;
        } finally {
          refreshInFlight.current = null;
        }
      })();
    }
    return refreshInFlight.current.catch(() => null);
  }

  // JWT decode helper (minimal, production-ready)
 function decodeUserFromToken(token: string): UserDto | null {
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    return {
      id: typeof payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] === 'string'
        ? parseInt(payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'], 10)
        : payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'],
      email: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
      role: payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'],
    };
  } catch {
    return null;
  }
}

  return (
    <AuthContext.Provider value={{ user, accessToken, setAuth, clearAuth, isLoading }}>
      {children}
    </AuthContext.Provider>
  );
};

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
