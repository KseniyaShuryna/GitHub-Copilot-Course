import axios, { AxiosError } from 'axios';
import type {
  AxiosInstance,
  AxiosRequestConfig,
  InternalAxiosRequestConfig,
} from 'axios';

type RefreshResponse = {
  accessToken: string;
};

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL as string;

const api: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  withCredentials: true, // send cookies (refresh token)
});

// -------------------------
// Access token storage
// -------------------------
// Optional: persist access token across reloads
const ACCESS_TOKEN_STORAGE_KEY = 'accessToken';

export function setAccessToken(token: string | null): void {
  // `persist` is kept for API compatibility, but this module now uses
  // sessionStorage as the single source of truth for the access token.
  if (token) {
    sessionStorage.setItem(ACCESS_TOKEN_STORAGE_KEY, token);
  } else {
    sessionStorage.removeItem(ACCESS_TOKEN_STORAGE_KEY);
  }
}

export function getAccessToken(): string | null {
  const fromStorage = sessionStorage.getItem(ACCESS_TOKEN_STORAGE_KEY);
  return fromStorage || null;
}

export function clearAuthState(): void {
  setAccessToken(null);
}

// -------------------------
// Request interceptor (attach access token)
// -------------------------
api.interceptors.request.use((config: InternalAxiosRequestConfig) => {
  const token = getAccessToken();

  // Avoid adding token to auth endpoints if you want; harmless if you do.
  if (token) {
    config.headers = config.headers ?? {};
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

// -------------------------
// Refresh logic (single-flight)
// -------------------------
let isRefreshing = false;
let refreshPromise: Promise<string> | null = null;

function isAuthEndpoint(url?: string): boolean {
  if (!url) return false;
  // adjust to your real endpoints if needed
  return (
    url.includes('/auth/login') ||
    url.includes('/auth/register') ||
    url.includes('/auth/refresh-token') ||
    url.includes('/auth/logout')
  );
}

async function handleTokenRefresh(): Promise<string> {
  // IMPORTANT: use a bare axios instance to avoid interceptor recursion
  const res = await axios.post<RefreshResponse>(
    `${API_BASE_URL}/auth/refresh-token`,
    undefined,
    { withCredentials: true }
  );

  const newToken = res.data?.accessToken;
  if (!newToken) {
    throw new Error('Refresh endpoint did not return accessToken');
  }

  // persist = true if you want to survive reload; set false if strictly in-memory
  setAccessToken(newToken);

  return newToken;
}

// -------------------------
// Response interceptor (retry on 401)
// -------------------------
api.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const status = error.response?.status;
    const originalConfig = error.config as AxiosRequestConfig & {
      _retry?: boolean;
    };

    // If there's no config (rare), just reject
    if (!originalConfig) return Promise.reject(error);

    // Don't try to refresh for auth endpoints themselves (avoid loops)
    if (isAuthEndpoint(originalConfig.url)) {
      return Promise.reject(error);
    }

    // Only handle 401
    if (status !== 401) {
      return Promise.reject(error);
    }

    // Prevent infinite retry loop for the same request
    if (originalConfig._retry) {
      return Promise.reject(error);
    }
    originalConfig._retry = true;

    // If refresh already in progress, wait and retry
    if (isRefreshing && refreshPromise) {
      try {
        await refreshPromise;
        return api.request(originalConfig);
      } catch (e) {
        clearAuthState();
        return Promise.reject(e);
      }
    }

    isRefreshing = true;
    refreshPromise = handleTokenRefresh();

    try {
      await refreshPromise;
      return api.request(originalConfig);
    } catch (e) {
      // Refresh failed: clear state; UI layer should redirect to /login
      clearAuthState();
      return Promise.reject(e);
    } finally {
      isRefreshing = false;
      refreshPromise = null;
    }
  }
);

export default api;