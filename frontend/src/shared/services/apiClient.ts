// src/shared/services/apiClient.ts
import axios, { AxiosError, InternalAxiosRequestConfig } from 'axios';

/**
 * Base Axios instance for HTTP requests across the application.
 */
export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5000/api',
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 10000,
});

/**
 * Request Interceptor:
 * Automatically injects the JWT Bearer token into the Authorization header
 * if a valid token exists in localStorage.
 */
apiClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = localStorage.getItem('token');
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error: AxiosError) => Promise.reject(error)
);

/**
 * Response Interceptor:
 * Handles global HTTP errors (e.g., 401 Unauthorized for token expiration).
 */
apiClient.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      // Optional: Redirect to login or trigger an auth event
    }
    return Promise.reject(error);
  }
);