import { BrowserRouter, Routes, Route } from 'react-router-dom';

import { ThemeProvider } from '@shared/context/ThemeContext';

import { AuthProvider } from '@features/auth/context/AuthContext';
import { ProtectedRoute } from '@features/auth/components/ProtectedRoute';

import { HomePage } from '@features/home/pages/HomePage';
import { AdminPage } from '@features/home/pages/AdminPage';
import { DashboardPage } from '@features/home/pages/DashboardPage';

import { AuthPage } from '@features/auth/pages/AuthPage';

/**
 * Root Application Router with modular routes and role-protected guards.
 */
export default function App() {
  return (
    <ThemeProvider>
      <AuthProvider>
        <BrowserRouter>
          <Routes>
            {/* Public Routes */}
            <Route path="/" element={<HomePage />} />
            <Route path="/login" element={<AuthPage />} />

            {/* Protected Routes */}
            <Route
              path="/dashboard"
              element={
                <ProtectedRoute>
                  <DashboardPage />
                </ProtectedRoute>
              }
            />
            <Route
              path="/admin"
              element={
                <ProtectedRoute allowedRoles={['Admin']}>
                  <AdminPage />
                </ProtectedRoute>
              }
            />
          </Routes>
        </BrowserRouter>
      </AuthProvider>
    </ThemeProvider>
  );
}