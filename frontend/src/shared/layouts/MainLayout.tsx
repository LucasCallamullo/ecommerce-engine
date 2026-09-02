// src/shared/layouts/MainLayout.tsx
import { useState, useRef, useEffect } from 'react';
import { Outlet, Link } from 'react-router-dom';
import { useAuth } from '@features/auth/context/AuthContext';
import { useTheme } from '@shared/context/ThemeContext';
import { Button } from '@shared/components/ui/button';
import { 
  Store, 
  Sun, 
  Moon, 
  LogOut, 
  ShoppingCart, 
  LogIn, 
  ChevronDown, 
  Search,
  Menu,
  User,
  LayoutDashboard,
  X
} from 'lucide-react';

export function MainLayout() {
  const { user, isAuthenticated, logout } = useAuth();
  const { theme, toggleTheme } = useTheme();

  const [searchQuery, setSearchQuery] = useState('');
  const [selectedCategory, setSelectedCategory] = useState('Seleccionar categoría');
  const [isCategoryOpen, setIsCategoryOpen] = useState(false);
  const [isUserMenuOpen, setIsUserMenuOpen] = useState(false);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  const menuRef = useRef<HTMLDivElement>(null);

  const categories = ['Todas las categorías', 'Electrónica', 'Ropa', 'Hogar', 'Deportes'];

  // Cerrar User Dropdown si se hace click afuera
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (menuRef.current && !menuRef.current.contains(event.target as Node)) {
        setIsUserMenuOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const handleSearchSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    console.log('Searching:', { searchQuery, selectedCategory });
  };

  return (
    <div className="min-h-screen bg-slate-50 dark:bg-slate-950 text-slate-900 dark:text-slate-100 flex flex-col">
      {/* Top Navbar */}
      <header className="sticky top-0 z-40 border-b border-slate-200 dark:border-slate-800 bg-white/80 dark:bg-slate-900/80 backdrop-blur-md px-4 md:px-6 py-3">
        
        {/* Main Grid Container */}
        <div className="max-w-7xl mx-auto flex md:grid md:grid-cols-[200px_1fr_200px] items-center justify-between gap-4">
          
          {/* 1. BRAND LOGO */}
          <Link to="/" className="flex items-center gap-2 font-bold text-lg shrink-0">
            <Store className="h-6 w-6 text-indigo-600 dark:text-indigo-400" />
            <span>E-Commerce</span>
          </Link>

          {/* 2. SEARCH BAR */}
          <form 
            onSubmit={handleSearchSubmit} 
            className="hidden md:flex items-center bg-slate-100 dark:bg-slate-900 border border-slate-300 dark:border-slate-800 rounded-lg p-1 focus-within:ring-2 focus-within:ring-indigo-500/50 transition-all w-full max-w-lg justify-self-center"
          >
            {/* Category Selector Dropdown */}
            <div className="relative border-r border-slate-300 dark:border-slate-800 shrink-0">
              <button
                type="button"
                onClick={() => setIsCategoryOpen(!isCategoryOpen)}
                onBlur={() => setTimeout(() => setIsCategoryOpen(false), 150)}
                className="flex items-center gap-1.5 px-3 py-1.5 text-xs font-medium text-slate-700 dark:text-slate-300 hover:text-indigo-600 dark:hover:text-indigo-400 cursor-pointer"
              >
                <span>{selectedCategory}</span>
                <ChevronDown className={`h-3.5 w-3.5 text-slate-400 transition-transform ${isCategoryOpen ? 'rotate-180' : ''}`} />
              </button>

              {isCategoryOpen && (
                <ul className="absolute top-full left-0 mt-2 w-44 bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-lg shadow-lg py-1 z-50 text-xs">
                  {categories.map((category) => (
                    <li key={category}>
                      <button
                        type="button"
                        onClick={() => {
                          setSelectedCategory(category);
                          setIsCategoryOpen(false);
                        }}
                        className="w-full text-left px-3 py-2 hover:bg-slate-100 dark:hover:bg-slate-800 text-slate-700 dark:text-slate-200 cursor-pointer"
                      >
                        {category}
                      </button>
                    </li>
                  ))}
                </ul>
              )}
            </div>

            {/* Input Search Name */}
            <input
              type="text"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              placeholder="Buscar por nombre de producto..."
              className="w-full bg-transparent border-none text-xs px-3 py-1.5 text-slate-900 dark:text-slate-100 placeholder:text-slate-400 focus:outline-none"
            />

            <Button type="submit" size="sm" variant="ghost" className="h-7 px-2 text-slate-500 hover:text-indigo-600 cursor-pointer">
              <Search className="h-4 w-4" />
            </Button>
          </form>

          {/* 3. DESKTOP ACTIONS & NAVIGATION */}
          <nav className="hidden md:flex items-center justify-end gap-3 shrink-0">
            {/* Cart Button */}
            <Button variant="secondary" size="icon" className="relative h-9 w-9 cursor-pointer" title="Shopping Cart">
              <ShoppingCart className="h-5 w-5 text-slate-700 dark:text-slate-200" />
              <span className="absolute -top-1 -right-1 flex h-4 w-4 items-center justify-center rounded-full bg-indigo-600 text-[10px] font-bold text-white">
                0
              </span>
            </Button>

            {/* Theme Toggle */}
            <Button variant="secondary" size="icon" onClick={toggleTheme} className="h-9 w-9 cursor-pointer">
              {theme === 'dark' ? <Sun className="h-4 w-4 text-amber-400" /> : <Moon className="h-4 w-4 text-indigo-600" />}
            </Button>

            {/* USER MENU DROPDOWN */}
            {isAuthenticated ? (
              <div className="relative" ref={menuRef}>
                <Button 
                  variant="secondary" 
                  size="icon" 
                  onClick={() => setIsUserMenuOpen(!isUserMenuOpen)}
                  className="h-9 w-9 rounded-full border border-slate-300 dark:border-slate-700 cursor-pointer"
                >
                  <User className="h-5 w-5 text-indigo-600 dark:text-indigo-400" />
                </Button>

                {isUserMenuOpen && (
                  <div className="absolute right-0 mt-2 w-48 bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-lg shadow-xl py-1 z-50 text-xs animate-in fade-in slide-in-from-top-1 duration-150">
                    <div className="px-3 py-2 border-b border-slate-100 dark:border-slate-800">
                      <p className="font-semibold text-slate-900 dark:text-slate-100">{user?.firstName} {user?.lastName}</p>
                      <p className="text-[11px] text-slate-400 truncate">{user?.email}</p>
                    </div>

                    <Link 
                      to="/dashboard" 
                      onClick={() => setIsUserMenuOpen(false)}
                      className="flex items-center gap-2 px-3 py-2 hover:bg-slate-100 dark:hover:bg-slate-800 text-slate-700 dark:text-slate-200"
                    >
                      <User className="h-4 w-4 text-indigo-500" />
                      <span>Profile</span>
                    </Link>

                    {user?.roles?.includes('Admin') && (
                      <Link 
                        to="/admin" 
                        onClick={() => setIsUserMenuOpen(false)}
                        className="flex items-center gap-2 px-3 py-2 hover:bg-slate-100 dark:hover:bg-slate-800 text-amber-600 dark:text-amber-400 font-medium"
                      >
                        <LayoutDashboard className="h-4 w-4" />
                        <span>Admin Dashboard</span>
                      </Link>
                    )}

                    <div className="border-t border-slate-100 dark:border-slate-800 my-1"></div>

                    <button
                      onClick={() => {
                        setIsUserMenuOpen(false);
                        logout();
                      }}
                      className="w-full flex items-center gap-2 px-3 py-2 hover:bg-red-50 dark:hover:bg-red-950/30 text-red-600 dark:text-red-400 cursor-pointer text-left"
                    >
                      <LogOut className="h-4 w-4" />
                      <span>Logout</span>
                    </button>
                  </div>
                )}
              </div>
            ) : (
              <Link to="/login">
                <Button size="sm" className="gap-2 h-9 items-center cursor-pointer bg-indigo-600 hover:bg-indigo-500 text-white font-medium">
                  <LogIn className="h-4 w-4" /> Login
                </Button>
              </Link>
            )}
          </nav>

          {/* 4. MOBILE CONTROLS */}
          <div className="flex md:hidden items-center gap-2">
            <Button variant="secondary" size="icon" className="relative h-9 w-9 cursor-pointer">
              <ShoppingCart className="h-5 w-5 text-slate-700 dark:text-slate-200" />
              <span className="absolute -top-1 -right-1 flex h-4 w-4 items-center justify-center rounded-full bg-indigo-600 text-[10px] font-bold text-white">
                0
              </span>
            </Button>

            <Button 
              variant="outline" 
              size="icon" 
              onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
              className="h-9 w-9"
            >
              {isMobileMenuOpen ? <X className="h-5 w-5" /> : <Menu className="h-5 w-5" />}
            </Button>
          </div>
        </div>

        {/* 5. MOBILE DROPDOWN MENU */}
        {isMobileMenuOpen && (
          <div className="md:hidden pt-4 pb-2 border-t border-slate-200 dark:border-slate-800 mt-3 flex flex-col gap-4">
            
            {/* Buscador Mobile */}
            <form onSubmit={handleSearchSubmit} className="flex items-center bg-slate-100 dark:bg-slate-900 border border-slate-300 dark:border-slate-800 rounded-lg p-1">
              <input
                type="text"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                placeholder="Buscar por nombre de producto..."
                className="w-full bg-transparent text-xs px-3 py-1.5 focus:outline-none"
              />
              <Button type="submit" size="sm" variant="ghost" className="h-7 px-2 text-slate-500">
                <Search className="h-4 w-4" />
              </Button>
            </form>

            {/* Selector de Categorías en Mobile */}
            <div className="flex flex-col gap-1">
              <span className="text-xs font-semibold text-slate-400">Buscar por categoría:</span>
              <select
                value={selectedCategory}
                onChange={(e) => setSelectedCategory(e.target.value)}
                className="bg-slate-100 dark:bg-slate-900 border border-slate-300 dark:border-slate-800 rounded-md p-2 text-xs text-slate-700 dark:text-slate-200"
              >
                {categories.map((cat) => (
                  <option key={cat} value={cat}>{cat}</option>
                ))}
              </select>
            </div>

            {/* Links de Usuario en Mobile */}
            {isAuthenticated ? (
              <div className="flex flex-col gap-2 pt-2 border-t border-slate-200 dark:border-slate-800">
                <p className="text-xs font-semibold text-slate-400">Mi Cuenta:</p>
                
                <Link 
                  to="/dashboard" 
                  onClick={() => setIsMobileMenuOpen(false)}
                  className="flex items-center gap-2 text-sm font-medium py-1 hover:text-indigo-600"
                >
                  <User className="h-4 w-4 text-indigo-500" /> Profile
                </Link>

                {user?.roles?.includes('Admin') && (
                  <Link 
                    to="/admin" 
                    onClick={() => setIsMobileMenuOpen(false)}
                    className="flex items-center gap-2 text-sm text-amber-600 dark:text-amber-400 font-medium py-1"
                  >
                    <LayoutDashboard className="h-4 w-4" /> Admin Dashboard
                  </Link>
                )}
              </div>
            ) : null}

            {/* Acciones de Sistema (Theme & Auth) */}
            <div className="flex items-center justify-between pt-3 border-t border-slate-200 dark:border-slate-800">
              <Button variant="outline" size="sm" onClick={toggleTheme} className="gap-2">
                {theme === 'dark' ? <Sun className="h-4 w-4 text-amber-400" /> : <Moon className="h-4 w-4 text-indigo-600" />}
                <span className="text-xs">Tema</span>
              </Button>

              {isAuthenticated ? (
                <Button variant="ghost" size="sm" onClick={logout} className="gap-2 text-red-600 dark:text-red-400">
                  <LogOut className="h-4 w-4" /> Logout
                </Button>
              ) : (
                <Link to="/login" onClick={() => setIsMobileMenuOpen(false)}>
                  <Button size="sm" className="gap-2 bg-indigo-600 text-white">
                    <LogIn className="h-4 w-4" /> Login
                  </Button>
                </Link>
              )}
            </div>

          </div>
        )}
      </header>

      {/* Dynamic Content */}
      <main className="flex-1">
        <Outlet />
      </main>
    </div>
  );
}