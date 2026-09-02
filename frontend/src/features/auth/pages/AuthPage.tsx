import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '@features/auth/context/AuthContext';
import { useTheme } from '@shared/context/ThemeContext';
import { useAuthForm } from '@features/auth/hooks/useAuthForm';
import { Button } from '@shared/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@shared/components/ui/card';
import {
  Store,
  LogIn,
  UserPlus,
  ArrowLeft,
  KeyRound,
  Mail,
  User as UserIcon,
  Phone,
  FileText,
  Sun,
  Moon,
  ShieldCheck,
  UserCheck,
} from 'lucide-react';

export function AuthPage() {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();
  const { theme, toggleTheme } = useTheme();

  const {
    isLogin,
    formData,
    isLoading,
    error,
    handleChange,
    toggleMode,
    handleSubmit,
    fillTestCredentials,
  } = useAuthForm();

  useEffect(() => {
    if (isAuthenticated) {
      navigate('/dashboard', { replace: true });
    }
  }, [isAuthenticated, navigate]);

  return (
    <div className="min-h-screen bg-slate-50 text-slate-900 dark:bg-slate-950 dark:text-slate-100 flex flex-col items-center justify-center p-6 transition-colors duration-200">
      
      {/* Top Left Navigation Back Button */}
      <Button
        variant="ghost"
        className="absolute top-6 left-6 gap-2 text-slate-600 hover:text-slate-900 dark:text-slate-400 dark:hover:text-slate-100"
        onClick={() => navigate('/')}
      >
        <ArrowLeft className="h-4 w-4" /> Back to Home
      </Button>

      {/* Top Right Theme Switcher */}
      <Button
        variant="outline"
        size="icon"
        onClick={toggleTheme}
        className="absolute top-6 right-6 border-slate-300 dark:border-slate-800 bg-white dark:bg-slate-900 text-slate-700 dark:text-slate-200 hover:bg-slate-100 dark:hover:bg-slate-800"
        title="Toggle Theme"
      >
        {theme === 'dark' ? <Sun className="h-5 w-5 text-amber-400" /> : <Moon className="h-5 w-5 text-indigo-600" />}
      </Button>

      <Card className="max-w-md w-full shadow-xl dark:shadow-2xl bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-800 text-slate-900 dark:text-slate-100">
        <CardHeader className="text-center space-y-2">
          <div className="flex justify-center items-center gap-2">
            <Store className="h-7 w-7 text-indigo-600 dark:text-indigo-400" />
            <CardTitle className="text-2xl font-bold tracking-tight">
              {isLogin ? 'Welcome Back' : 'Create Account'}
            </CardTitle>
          </div>
          <CardDescription className="text-slate-600 dark:text-slate-400">
            {isLogin
              ? 'Enter your credentials to access your account'
              : 'Fill in your details to get started with E-Commerce'}
          </CardDescription>
        </CardHeader>

        <CardContent className="space-y-4">
          {error && (
            <div className="p-3 text-sm rounded-lg bg-red-100 border border-red-300 text-red-800 dark:bg-red-950/50 dark:border-red-800/60 dark:text-red-300">
              {error}
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-3">
            {!isLogin && (
              <>
                <div className="grid grid-cols-2 gap-2">
                  <div className="relative">
                    <UserIcon className="absolute left-3 top-3 h-4 w-4 text-slate-400 dark:text-slate-500" />
                    <input
                      type="text"
                      name="firstName"
                      placeholder="First Name"
                      required
                      value={formData.firstName}
                      onChange={handleChange}
                      className="w-full bg-slate-100 dark:bg-slate-950 border border-slate-300 dark:border-slate-800 rounded-lg py-2 pl-9 pr-3 text-sm focus:outline-none focus:border-indigo-500 text-slate-900 dark:text-slate-200 placeholder:text-slate-400 dark:placeholder:text-slate-600"
                    />
                  </div>
                  <div className="relative">
                    <UserIcon className="absolute left-3 top-3 h-4 w-4 text-slate-400 dark:text-slate-500" />
                    <input
                      type="text"
                      name="lastName"
                      placeholder="Last Name"
                      required
                      value={formData.lastName}
                      onChange={handleChange}
                      className="w-full bg-slate-100 dark:bg-slate-950 border border-slate-300 dark:border-slate-800 rounded-lg py-2 pl-9 pr-3 text-sm focus:outline-none focus:border-indigo-500 text-slate-900 dark:text-slate-200 placeholder:text-slate-400 dark:placeholder:text-slate-600"
                    />
                  </div>
                </div>

                <div className="grid grid-cols-2 gap-2">
                  <div className="relative">
                    <Phone className="absolute left-3 top-3 h-4 w-4 text-slate-400 dark:text-slate-500" />
                    <input
                      type="tel"
                      name="cellphone"
                      placeholder="Cellphone"
                      required
                      value={formData.cellphone}
                      onChange={handleChange}
                      className="w-full bg-slate-100 dark:bg-slate-950 border border-slate-300 dark:border-slate-800 rounded-lg py-2 pl-9 pr-3 text-sm focus:outline-none focus:border-indigo-500 text-slate-900 dark:text-slate-200 placeholder:text-slate-400 dark:placeholder:text-slate-600"
                    />
                  </div>
                  <div className="relative">
                    <FileText className="absolute left-3 top-3 h-4 w-4 text-slate-400 dark:text-slate-500" />
                    <input
                      type="text"
                      name="dni"
                      placeholder="DNI / ID"
                      required
                      value={formData.dni}
                      onChange={handleChange}
                      className="w-full bg-slate-100 dark:bg-slate-950 border border-slate-300 dark:border-slate-800 rounded-lg py-2 pl-9 pr-3 text-sm focus:outline-none focus:border-indigo-500 text-slate-900 dark:text-slate-200 placeholder:text-slate-400 dark:placeholder:text-slate-600"
                    />
                  </div>
                </div>
              </>
            )}

            <div className="relative">
              <Mail className="absolute left-3 top-3 h-4 w-4 text-slate-400 dark:text-slate-500" />
              <input
                type="email"
                name="email"
                placeholder="Email Address"
                required
                value={formData.email}
                onChange={handleChange}
                className="w-full bg-slate-100 dark:bg-slate-950 border border-slate-300 dark:border-slate-800 rounded-lg py-2 pl-9 pr-3 text-sm focus:outline-none focus:border-indigo-500 text-slate-900 dark:text-slate-200 placeholder:text-slate-400 dark:placeholder:text-slate-600"
              />
            </div>

            <div className="relative">
              <KeyRound className="absolute left-3 top-3 h-4 w-4 text-slate-400 dark:text-slate-500" />
              <input
                type="password"
                name="password"
                placeholder="Password"
                required
                value={formData.password}
                onChange={handleChange}
                className="w-full bg-slate-100 dark:bg-slate-950 border border-slate-300 dark:border-slate-800 rounded-lg py-2 pl-9 pr-3 text-sm focus:outline-none focus:border-indigo-500 text-slate-900 dark:text-slate-200 placeholder:text-slate-400 dark:placeholder:text-slate-600"
              />
            </div>

            <Button
              type="submit"
              disabled={isLoading}
              className="w-full gap-2 bg-indigo-600 hover:bg-indigo-500 text-white font-medium py-2 rounded-lg"
            >
              {isLogin ? <LogIn className="h-4 w-4" /> : <UserPlus className="h-4 w-4" />}
              {isLoading ? 'Processing...' : isLogin ? 'Sign In' : 'Register Account'}
            </Button>
          </form>

          {/* Quick Demo Credentials */}
          <div className="pt-2">
            <p className="text-xs text-center text-slate-500 dark:text-slate-400 mb-2 font-medium">
              Quick Test Credentials
            </p>
            <div className="grid grid-cols-2 gap-2">
              <Button
                type="button"
                variant="outline"
                size="sm"
                onClick={() => fillTestCredentials('user@example.com')}
                className="gap-1.5 text-xs border-slate-300 dark:border-slate-800 bg-slate-50 dark:bg-slate-950 hover:bg-slate-100 dark:hover:bg-slate-800 text-slate-700 dark:text-slate-300"
              >
                <UserCheck className="h-3.5 w-3.5 text-emerald-500" />
                Customer Test
              </Button>
              <Button
                type="button"
                variant="outline"
                size="sm"
                onClick={() => fillTestCredentials('admin@example.com')}
                className="gap-1.5 text-xs border-slate-300 dark:border-slate-800 bg-slate-50 dark:bg-slate-950 hover:bg-slate-100 dark:hover:bg-slate-800 text-slate-700 dark:text-slate-300"
              >
                <ShieldCheck className="h-3.5 w-3.5 text-indigo-500" />
                Admin Test
              </Button>
            </div>
          </div>

          <div className="pt-4 border-t border-slate-200 dark:border-slate-800 text-center text-sm text-slate-600 dark:text-slate-400">
            {isLogin ? "Don't have an account?" : 'Already have an account?'}
            <button
              type="button"
              onClick={toggleMode}
              className="ml-2 text-indigo-600 dark:text-indigo-400 hover:underline font-medium focus:outline-none"
            >
              {isLogin ? 'Register now' : 'Log in instead'}
            </button>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}