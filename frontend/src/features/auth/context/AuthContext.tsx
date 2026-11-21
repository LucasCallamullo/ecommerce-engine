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
  const login = useCallback((authData: AuthData) => {
    localStorage.setItem('accessToken', authData.accessToken);
    localStorage.setItem('refreshToken', authData.refreshToken);
    localStorage.setItem('expiresAt', authData.expiresAt);
    localStorage.setItem('user', JSON.stringify(authData.user));

    setUser(authData.user);
    setToken(authData.accessToken);
  }, []);

  /**
   * Hydrates the session on mount:
   * 1. Loads cached user details from localStorage for immediate UI rendering.
   * 2. Validates session against the database via GET `/v1/auth/me`.
   * 3. Clears local state if session validation or token refresh fails.
   */
  useEffect(() => {
    let isMounted = true;

    const initAuth = async () => {
      const storedToken = localStorage.getItem('accessToken');
      const storedUser = localStorage.getItem('user');

      if (!storedToken) {
        if (isMounted) setIsLoading(false);
        return;
      }

      // Step 1: Optimistic UI hydration from local storage
      let parsedStoredUser: User | null = null;
      if (storedUser) {
        try {
          parsedStoredUser = JSON.parse(storedUser);
          if (isMounted) {
            setUser(parsedStoredUser);
            setToken(storedToken);
          }
        } catch {
          localStorage.removeItem('user');
        }
      }

      // Step 2: Source-of-truth verification against the database (/v1/auth/me)
      try {
        const response = await authService.getProfile();

        if (!isMounted) return;

        if (response.success && response.data) {
          const updatedUser: User = {
            ...response.data,
            // Preserva los roles de localStorage si el endpoint /me no los devuelve
            roles: response.data.roles ?? parsedStoredUser?.roles ?? [],
          };

          setUser(updatedUser);
          localStorage.setItem('user', JSON.stringify(updatedUser));
          setToken(storedToken);
        }
      } catch {
        // Fallback: If /me fails and the Axios response interceptor cannot refresh, clear storage
        if (isMounted) logout();
      } finally {
        if (isMounted) setIsLoading(false);
      }
    };

    initAuth();

    return () => {
      isMounted = false;
    };
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