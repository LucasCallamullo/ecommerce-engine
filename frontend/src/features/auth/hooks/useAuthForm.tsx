import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '@features/auth/context/AuthContext';
import { authService } from '@features/auth/services/authService';

/**
 * Custom hook isolating form state and submission workflow for login/registration.
 */
export function useAuthForm() {
  const [isLogin, setIsLogin] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const navigate = useNavigate();
  const { login } = useAuth();

  const fillTestCredentials = (email: string) => {
    setIsLogin(true);
    setError(null);
    setFormData((prev) => ({
      ...prev,
      email,
      password: '1234',
    }));
  };

  // Form Fields State
  const [formData, setFormData] = useState({
    email: '',
    password: '',
    firstName: '',
    lastName: '',
    cellphone: '',
    dni: '',
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const toggleMode = () => {
    setIsLogin((prev) => !prev);
    setError(null);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setIsLoading(true);

    try {
      if (isLogin) {
        const response = await authService.login({
          email: formData.email,
          password: formData.password,
        });
        if (response.success && response.data) {
          login(response.data);
          navigate('/dashboard');
        }
      } else {
        const regResponse = await authService.register(formData);

        if (regResponse.success) {
          const loginResponse = await authService.login({
            email: formData.email,
            password: formData.password,
          });
          if (loginResponse.success && loginResponse.data) {
            login(loginResponse.data);
            navigate('/dashboard');
          }
        }
      }
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'An error occurred during authentication');
    } finally {
      setIsLoading(false);
    }
  };

  return {
    isLogin,
    formData,
    isLoading,
    error,
    handleChange,
    toggleMode,
    handleSubmit,
    fillTestCredentials,
  };
}