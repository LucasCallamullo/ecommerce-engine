import { apiClient } from '@shared/services/apiClient';
import {
  RegisterDto,
  RegisterResponse,
  LoginDto,
  LoginResponse,
  RefreshTokenDto,
  RefreshTokenResponse,
  UserProfileResponse,
} from '@features/auth/types/authTypes';

const AUTH_BASE_PATH = '/v1/auth';

export const authService = {
  /**
   * Registers a new user account in the system.
   */
  register: async (payload: RegisterDto): Promise<RegisterResponse> => {
    const response = await apiClient.post<RegisterResponse>(`${AUTH_BASE_PATH}/register`, payload);
    return response.data;
  },

  /**
   * Authenticates user credentials and issues session tokens.
   */
  login: async (credentials: LoginDto): Promise<LoginResponse> => {
    const response = await apiClient.post<LoginResponse>(`${AUTH_BASE_PATH}/login`, credentials);
    return response.data;
  },

  /**
   * Rotates access and refresh tokens using a valid refresh token.
   */
  refreshToken: async (payload: RefreshTokenDto): Promise<RefreshTokenResponse> => {
    const response = await apiClient.post<RefreshTokenResponse>(`${AUTH_BASE_PATH}/refresh`, payload);
    return response.data;
  },

  /**
   * Retrieves the current authenticated user's profile from the database.
   */
  getProfile: async (): Promise<UserProfileResponse> => {
    const response = await apiClient.get<UserProfileResponse>(`${AUTH_BASE_PATH}/me`);
    return response.data;
  },

  /**
   * Logs out the user and clears locally stored authentication state.
   */
  logout: async (): Promise<void> => {
    try {
      await apiClient.post(`${AUTH_BASE_PATH}/logout`);
    } finally {
      localStorage.clear();
    }
  },
};