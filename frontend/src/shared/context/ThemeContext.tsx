import { createContext, useContext, useEffect, useState, ReactNode } from 'react';

/**
 * Supported color scheme themes for the application.
 * Agrega aquí cualquier tema futuro (ej: 'brand', 'midnight').
 */
type Theme = 'dark' | 'light' | 'system' | 'brand';

/**
 * Interface representing the shape of the Theme Context value.
 */
interface ThemeContextType {
  /** The currently active theme mode (incluye 'system' como preferencia). */
  theme: Theme;
  /** El tema realmente aplicado al DOM ('light' | 'dark' | 'brand'...). */
  resolvedTheme: Exclude<Theme, 'system'>;
  /** Function to explicitly set a specific theme mode. */
  setTheme: (theme: Theme) => void;
  /** Helper function to toggle directly between light and dark modes. */
  toggleTheme: () => void;
}

const ThemeContext = createContext<ThemeContextType | undefined>(undefined);

interface ThemeProviderProps {
  children: ReactNode;
}

const STORAGE_KEY = 'theme';

/**
 * Resuelve 'system' al tema real según la preferencia del SO.
 */
function getSystemTheme(): 'light' | 'dark' {
  return window.matchMedia('(prefers-color-scheme: dark)').matches
    ? 'dark'
    : 'light';
}

/**
 * Provider component that manages global theme state, persists preferences
 * to localStorage, and updates the HTML root DOM element accordingly.
 */
export function ThemeProvider({ children }: ThemeProviderProps) {
  const [theme, setTheme] = useState<Theme>(() => {
    return (localStorage.getItem(STORAGE_KEY) as Theme) || 'system';
  });

  // Tema efectivo aplicado al DOM
  const [resolvedTheme, setResolvedTheme] = useState<Exclude<Theme, 'system'>>(
    () => (theme === 'system' ? getSystemTheme() : theme)
  );

  useEffect(() => {
    const root = document.documentElement;

    const applyTheme = (next: Exclude<Theme, 'system'>) => {
      root.setAttribute('data-theme', next);
      setResolvedTheme(next);
    };

    if (theme === 'system') {
      applyTheme(getSystemTheme());

      // Escucha cambios del SO mientras esté en modo system
      const mql = window.matchMedia('(prefers-color-scheme: dark)');
      const onChange = () => applyTheme(getSystemTheme());
      mql.addEventListener('change', onChange);
      return () => mql.removeEventListener('change', onChange);
    }

    applyTheme(theme);
    localStorage.setItem(STORAGE_KEY, theme);
  }, [theme]);

  const toggleTheme = () => {
    setTheme((prev) => {
      // Si está en system, resuelve primero al tema actual y luego invierte
      const current = prev === 'system' ? getSystemTheme() : prev;
      return current === 'dark' ? 'light' : 'dark';
    });
  };

  return (
    <ThemeContext.Provider value={{ theme, resolvedTheme, setTheme, toggleTheme }}>
      {children}
    </ThemeContext.Provider>
  );
}

/**
 * Custom hook to consume the ThemeContext state and helper functions.
 * @throws {Error} If used outside of a ThemeProvider hierarchy.
 */
export const useTheme = () => {
  const context = useContext(ThemeContext);
  if (!context) {
    throw new Error('useTheme must be used within a ThemeProvider');
  }
  return context;
};