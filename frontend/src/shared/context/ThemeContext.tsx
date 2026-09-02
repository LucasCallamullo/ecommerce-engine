import { createContext, useContext, useEffect, useState, ReactNode } from 'react';

/**
 * Supported color scheme themes for the application.
 */
type Theme = 'dark' | 'light' | 'system';

/**
 * Interface representing the shape of the Theme Context value.
 */
interface ThemeContextType {
  /** The currently active theme mode. */
  theme: Theme;
  /** Function to explicitly set a specific theme mode. */
  setTheme: (theme: Theme) => void;
  /** Helper function to toggle directly between light and dark modes. */
  toggleTheme: () => void;
}

const ThemeContext = createContext<ThemeContextType | undefined>(undefined);

interface ThemeProviderProps {
  children: ReactNode;
}

/**
 * Provider component that manages global theme state, persists preferences 
 * to localStorage, and updates the HTML root DOM element accordingly.
 */
export function ThemeProvider({ children }: ThemeProviderProps) {
  const [theme, setTheme] = useState<Theme>(() => {
    return (localStorage.getItem('theme') as Theme) || 'system';
  });

  useEffect(() => {
    const root = document.documentElement;
    root.classList.remove('light', 'dark');

    if (theme === 'system') {
      const systemTheme = window.matchMedia('(prefers-color-scheme: dark)').matches 
        ? 'dark' 
        : 'light';
      root.classList.add(systemTheme);
      return;
    }

    root.classList.add(theme);
    localStorage.setItem('theme', theme);
  }, [theme]);

  const toggleTheme = () => {
    setTheme((prev) => (prev === 'dark' ? 'light' : 'dark'));
  };

  return (
    <ThemeContext.Provider value={{ theme, setTheme, toggleTheme }}>
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