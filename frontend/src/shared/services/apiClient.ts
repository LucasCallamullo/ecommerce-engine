import axios, { AxiosError, InternalAxiosRequestConfig } from 'axios';
import { ApiResponse } from '@shared/types/commonTypes';
import { AuthData } from '@features/auth/types/authTypes';

/**
 * Pre-configured Axios instance for handling HTTP communications with the backend API.
 */
export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5000/api',
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 10000,
});

/**
 * Extended Axios configuration interface to track request retry state.
 */
interface CustomAxiosRequestConfig extends InternalAxiosRequestConfig {
  /** Internal flag preventing infinite loops when retrying failed 401 requests. */
  _retry?: boolean;
}

/** Flag indicating if a token rotation request is currently in progress. */
let isRefreshing = false;

/** Queue holding failed requests waiting for token rotation to complete. */
let failedQueue: Array<{
  resolve: (value?: unknown) => void;
  reject: (reason?: unknown) => void;
}> = [];

/**
 * Processes the queued promises after token rotation succeeds or fails.
 *
 * @param error - Axios error if rotation failed, or null if successful.
 */
const processQueue = (error: AxiosError | null) => {
  failedQueue.forEach((promise) => {
    if (error) {
      promise.reject(error);
    } else {
      promise.resolve();
    }
  });
  failedQueue = [];
};

/**
 * Request Interceptor: Attaches Bearer JWT access token to outbound HTTP headers.
 */
apiClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = localStorage.getItem('accessToken');
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error: AxiosError) => Promise.reject(error)
);

/**
 * Response Interceptor: Catches 401 Unauthorized errors and transparently rotates tokens.
 */
apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config as CustomAxiosRequestConfig;

    // Skip retry logic if error is not 401, missing response, or already retried
    if (!error.response || error.response.status !== 401 || originalRequest._retry) {
      return Promise.reject(error);
    }

    // Prevent infinite loop if the refresh endpoint itself returns 401
    if (originalRequest.url?.includes('/v1/auth/refresh')) {
      localStorage.clear();
      window.location.href = '/login';
      return Promise.reject(error);
    }

    originalRequest._retry = true;

    // Queue concurrent requests while token rotation is underway
    if (isRefreshing) {
      return new Promise((resolve, reject) => {
        failedQueue.push({ resolve, reject });
      })
        .then(() => apiClient(originalRequest))
        .catch((err) => Promise.reject(err));
    }

    isRefreshing = true;

    try {
      const refreshToken = localStorage.getItem('refreshToken');

      if (!refreshToken) {
        throw new Error('No refresh token available');
      }

      // Execute explicit refresh request using standalone axios instance
      const { data } = await axios.post<ApiResponse<AuthData>>(
        `${apiClient.defaults.baseURL}/v1/auth/refresh`,
        { refreshToken }
      );

      if (!data.data) {
        throw new Error('Invalid refresh response structure');
      }

      const { accessToken: newAccessToken, refreshToken: newRefreshToken, expiresAt: newExpiresAt } = data.data;

      // Update persistent storage
      localStorage.setItem('accessToken', newAccessToken);
      if (newRefreshToken) localStorage.setItem('refreshToken', newRefreshToken);
      if (newExpiresAt) localStorage.setItem('expiresAt', newExpiresAt);

      // Update default headers and retry original request
      apiClient.defaults.headers.common.Authorization = `Bearer ${newAccessToken}`;
      originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;

      processQueue(null);

      return apiClient(originalRequest);
    } catch (refreshError) {
      processQueue(refreshError as AxiosError);
      localStorage.clear();
      window.location.href = '/login';
      return Promise.reject(refreshError);
    } finally {
      isRefreshing = false;
    }
  }
);