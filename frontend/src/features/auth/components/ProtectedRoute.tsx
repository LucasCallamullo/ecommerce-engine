import { ReactNode } from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '@features/auth/context/AuthContext';

/**
 * Props for the ProtectedRoute component.
 */
interface ProtectedRouteProps {
  /** The child components/pages to render if access criteria are met. */
  children: ReactNode;

  /**
   * Optional array of role names allowed to access this route.
   * If provided, the user must possess at least one matching role.
   */
  allowedRoles?: string[];
}

/**
 * Guard component for restricting route access based on authentication status and user roles.
 *
 * Behavior:
 * 1. Displays a loading indicator while session state is being hydrated.
 * 2. Redirects unauthenticated users to `/login`.
 * 3. Redirects authenticated users without the required roles to the home route `/`.
 * 4. Renders child components when access authorization succeeds.
 */
export function ProtectedRoute({ children, allowedRoles }: ProtectedRouteProps) {
  const { isAuthenticated, user, isLoading } = useAuth();

  if (isLoading) {
    return <div className="p-4 text-center">Loading session...</div>;
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  const userRoles = user?.roles ?? [];
  const hasRequiredRole = allowedRoles 
    ? userRoles.some((role) => allowedRoles.includes(role)) 
    : true;

  if (allowedRoles && !hasRequiredRole) {
    return <Navigate to="/" replace />;
  }

  return <>{children}</>;
}