import api, { setAccessToken, clearAuthState } from './client';
import type { LoginRequest, RegisterRequest, AuthResponse, ApiError } from './types';

// Helper for error extraction
function extractApiError(err: unknown): ApiError {
  if (typeof err === 'object' && err !== null && 'response' in err) {
    const data = (err as any).response?.data;

    if (data) {
      if (typeof data.error === 'string') {
        return {
          error: data.error,
          details: typeof data.details === 'string' ? data.details : undefined,
        };
      }

      if (typeof data.message === 'string') {
        return { error: data.message };
      }
    }
  }

  return { error: 'Unknown error' };
}

export const authApi = {
  async login(data: LoginRequest): Promise<AuthResponse> {
    try {
      const res = await api.post<AuthResponse>('/auth/login', data);
      if (res.data?.accessToken) {
        setAccessToken(res.data.accessToken);
      }
      return res.data;
    } catch (error) {
      throw extractApiError(error);
    }
  },

  async register(data: RegisterRequest): Promise<AuthResponse> {
    try {
      const res = await api.post<AuthResponse>('/auth/register', data);
      if (res.data?.accessToken) {
        setAccessToken(res.data.accessToken);
      }
      return res.data;
    } catch (error) {
      throw extractApiError(error);
    }
  },

  async refresh(): Promise<AuthResponse> {
    try {
      // No payload, cookie is sent automatically
      const res = await api.post<AuthResponse>('/auth/refresh-token');

      if (res.data?.accessToken) {
        setAccessToken(res.data.accessToken);
      }
      return res.data;
    } catch (error) {
      throw extractApiError(error);
    }
  },

  async logout(): Promise<void> {
    try {
      // No payload, cookie is sent automatically
      await api.post('/auth/logout');
    } catch (error) {
      throw extractApiError(error);
    } finally {
      clearAuthState();
    }
  }
};
