import { createContext, useContext, useState, useEffect, ReactNode, useCallback } from 'react';
import { User, AuthData, AuthContextType } from '@features/auth/types/authTypes';
import { authService } from '@features/auth/services/authService';

/** React Context for managing and broadcasting global authentication state. */
const AuthContext = createContext<AuthContextType | undefined>(undefined);

/**
 * Provider component that wraps the application to manage authentication lifecycle.
 * Handles initial session hydration, background profile verification, login, and logout.
 */
export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(true);

  /**
   * Clears session persistent storage and resets the active user and token state.
   */
  const logout = useCallback(() => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('expiresAt');
    localStorage.removeItem('user');

    setUser(null);
    setToken(null);
  }, []);

  /**
   * Persists fresh session credentials received from the login endpoint and updates state.
   *
   * @param authData - Payload containing access token, refresh token, expiry timestamp, and user profile.
   */
  const login = (authData: AuthData) => {
    localStorage.setItem('accessToken', authData.accessToken);
    localStorage.setItem('refreshToken', authData.refreshToken);
    localStorage.setItem('expiresAt', authData.expiresAt);
    localStorage.setItem('user', JSON.stringify(authData.user));

    setUser(authData.user);
    setToken(authData.accessToken);
  };

  /**
   * Hydrates the session on mount:
   * 1. Loads cached user details from localStorage for immediate UI rendering.
   * 2. Validates session against the database via GET `/v1/auth/me`.
   * 3. Clears local state if session validation or token refresh fails.
   */
  useEffect(() => {
    const initAuth = async () => {
      const storedToken = localStorage.getItem('accessToken');
      const storedUser = localStorage.getItem('user');

      if (!storedToken) {
        setIsLoading(false);
        return;
      }

      // Step 1: Optimistic UI hydration from local storage
      if (storedUser) {
        try {
          setUser(JSON.parse(storedUser));
          setToken(storedToken);
        } catch {
          localStorage.removeItem('user');
        }
      }

      // Step 2: Source-of-truth verification against the database (/v1/auth/me)
      try {
        const response = await authService.getProfile();
        if (response.success && response.data) {
          const updatedUser = {
            ...response.data,
            // If the backend sends null/undefined in /me, we preserve the current roles or assign an empty array
            roles: response.data.roles ?? user?.roles ?? []
          };

          setUser(updatedUser);
          localStorage.setItem('user', JSON.stringify(updatedUser));
          setToken(storedToken);
        }
      } catch {
        // Fallback: If /me fails and the Axios response interceptor cannot refresh, clear storage
        logout();
      } finally {
        setIsLoading(false);
      }
    };

    initAuth();
  }, [logout]);

  return (
    <AuthContext.Provider
      value={{
        user,
        token,
        isAuthenticated: !!user,
        isLoading,
        login,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

/**
 * Custom hook to consume the AuthContext value.
 *
 * @throws {Error} If called outside an AuthProvider hierarchy.
 * @returns {AuthContextType} The active authentication context state and actions.
 */
export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};