import { ApiResponse } from '@/shared/types/commonTypes';

// + ===============================================================
// +           To Requests DTOs   
// + ===============================================================

export interface RegisterDto {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  cellphone: string;
  dni: string;
}

export interface LoginDto {
  email: string;
  password: string;
}

export interface RefreshTokenDto {
  refreshToken: string;
}

// + ===============================================================
// +           Responses DTOs   
// + ===============================================================

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  cellphone?: string;
  dni?: string;
  roles: string[];
}

export interface AuthData {
  user: User;
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
}

// Aliases using standard ApiResponse
export type RegisterResponse = ApiResponse<User>;
export type LoginResponse = ApiResponse<AuthData>;
export type RefreshTokenResponse = ApiResponse<AuthData>;
export type UserProfileResponse = ApiResponse<User>;

/**
 * Shape of the React Context value managing global application authentication state.
 */
export interface AuthContextType {
  /** The currently authenticated user, or `null` if unauthenticated. */
  user: User | null;

  /** Active access token string, or `null` if unauthenticated. */
  token: string | null;

  /** Quick boolean flag indicating if an active user session exists. */
  isAuthenticated: boolean;

  /** Indicates whether the initial session restoration from persistent storage is in progress. */
  isLoading: boolean;

  /**
   * Updates state and persists session credentials following a successful login.
   * @param authData - The complete payload returned from the login API.
   */
  login: (authData: AuthData) => void;

  /** Clears local session storage and resets authentication state to unauthenticated. */
  logout: () => void;
}