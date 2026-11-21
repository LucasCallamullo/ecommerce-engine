// src/shared/layouts/MainLayout.tsx
import { useState, useRef, useEffect } from 'react';
import { Outlet, Link, useNavigate } from 'react-router-dom';
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
  Search,
  Menu,
  User,
  LayoutDashboard,
  X
} from 'lucide-react';

import { CategoryDropdownTree } from '@/features/products/components/CategoryTree';

export function MainLayout() {

  const { user, isAuthenticated, logout } = useAuth();
  const { theme, toggleTheme } = useTheme();

  // this is for query search
  const navigate = useNavigate();
  const [searchQuery, setSearchQuery] = useState('');

  const [isUserMenuOpen, setIsUserMenuOpen] = useState(false);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  const menuRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (menuRef.current && !menuRef.current.contains(event.target as Node)) {
        setIsUserMenuOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  // search query
  const handleSearchSubmit = (e: React.SubmitEvent) => {
    e.preventDefault();
    const trimmedQuery = searchQuery.trim();
    
    // if writes navigate to /products?searchTerm=...
    if (trimmedQuery) {
      navigate(`/products?searchTerm=${encodeURIComponent(trimmedQuery)}`);
    } else {
      navigate('/products');
    }

    setIsMobileMenuOpen(false);
  };

  return (
    <>
      {/* Top Navbar */}
      <header className="sticky top-0 backdrop-blur-md w-full border-b border-main-400 bg-surface-raised py-2.5 px-4 md:px-0">

        <div className="w-full max-w-full md:max-w-6xl mx-auto items-center flex justify-between
          md:grid md:grid-cols-[23.5%_1fr_180px] gap-4">

          {/* 1. BRAND LOGO */}
          <Link to="/" className="flex items-center gap-2 font-bold text-lg shrink-0">
            <Store className="h-6 w-6 text-primary" />
            <span>E-Commerce</span>
          </Link>

          {/* 2. SEARCH BAR */}
          <div className="hidden md:flex items-center bg-surface-muted border border-border rounded-lg
            transition-all w-full max-w-lg justify-self-start text-sm"
          > 
            <CategoryDropdownTree />

            {/* Category Selector Dropdown */}
            <form
              onSubmit={handleSearchSubmit}
              className="focus-within:ring-2 focus-within:ring-ring/50 flex w-full"
            >
              {/* Input Search Name */}
              <input
                type="text"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                placeholder="Buscar por nombre de producto..."
                className="w-full bg-transparent border-none px-3 py-1.5 text-content-main 
                  placeholder:text-content-muted focus:outline-none"
              />

              <Button type="submit" size="sm" variant="ghost" className="h-7 px-2 text-content-muted 
                hover:text-primary cursor-pointer">
                <Search className="h-5 w-5" />
              </Button>
            </form>
          </div>

          {/* 3. DESKTOP ACTIONS & NAVIGATION */}
          <nav className="hidden md:flex items-center justify-start md:justify-end gap-3 shrink-0">
            {/* Cart Button */}
            <Button variant="secondary" size="icon" className="relative h-9 w-9 cursor-pointer" title="Shopping Cart">
              <ShoppingCart className="h-4.5 w-4.5 text-content-main" />
              <span className="absolute -top-1 -right-1 flex h-4 w-4 items-center justify-center 
                rounded-full bg-primary text-xs font-bold text-primary-foreground">
                0
              </span>
            </Button>

            {/* Theme Toggle */}
            <Button variant="secondary" size="default" onClick={toggleTheme} className="h-9 w-9">
              {theme === 'dark' ? 
                <Sun className="h-4.5 w-4.5 text-warning" /> : 
                <Moon className="h-4.5 w-4.5 text-primary" />
              }
            </Button>

            {/* USER MENU DROPDOWN */}
            {isAuthenticated ? (
              <div className="relative" ref={menuRef}>
                <Button
                  variant="secondary"
                  size="icon"
                  onClick={() => setIsUserMenuOpen(!isUserMenuOpen)}
                  className="h-9 w-9 rounded-full border border-border cursor-pointer"
                >
                  <User className="h-4.5 w-4.5 text-primary" />
                </Button>

                {isUserMenuOpen && (
                  <div className="absolute right-0 mt-2 w-48 bg-surface-raised border border-border rounded-lg shadow-xl py-1 z-50 text-xs animate-in fade-in slide-in-from-top-1 duration-150">
                    <div className="px-3 py-2 border-b border-border">
                      <p className="font-semibold text-content-main">{user?.firstName} {user?.lastName}</p>
                      <p className="text-xs text-content-muted truncate">{user?.email}</p>
                    </div>

                    <Link
                      to="/dashboard"
                      onClick={() => setIsUserMenuOpen(false)}
                      className="flex items-center gap-2 px-3 py-2 hover:bg-surface-muted text-content-main"
                    >
                      <User className="h-4 w-4 text-primary" />
                      <span>Profile</span>
                    </Link>

                    {user?.roles?.includes('Admin') && (
                      <Link
                        to="/admin"
                        onClick={() => setIsUserMenuOpen(false)}
                        className="flex items-center gap-2 px-3 py-2 hover:bg-surface-muted text-warning font-medium"
                      >
                        <LayoutDashboard className="h-4 w-4" />
                        <span>Admin Dashboard</span>
                      </Link>
                    )}

                    <div className="border-t border-border my-1"></div>

                    <button
                      onClick={() => {
                        setIsUserMenuOpen(false);
                        logout();
                      }}
                      className="w-full flex items-center gap-2 px-3 py-2 hover:bg-danger/10 text-danger cursor-pointer text-left"
                    >
                      <LogOut className="h-4 w-4" />
                      <span>Logout</span>
                    </button>
                  </div>
                )}
              </div>
            ) : (
              <Link to="/login">
                <Button size="sm" className="gap-2 h-9 items-center cursor-pointer bg-primary hover:bg-primary-hover text-primary-foreground font-medium">
                  <LogIn className="h-4 w-4" /> Login
                </Button>
              </Link>
            )}
          </nav>

          {/* 4. MOBILE CONTROLS */}
          <div className="flex md:hidden items-center gap-2">
            <Button variant="secondary" size="icon" className="relative h-9 w-9 cursor-pointer">
              <ShoppingCart className="h-5 w-5 text-content-main" />
              <span className="absolute -top-1 -right-1 flex h-4 w-4 items-center justify-center rounded-full bg-primary text-[10px] font-bold text-primary-foreground">
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
          <div className="md:hidden pt-4 pb-2 border-t border-border mt-3 flex flex-col gap-4">


            <CategoryDropdownTree />


            {/* Buscador Mobile */}
            <form onSubmit={handleSearchSubmit} className="flex items-center bg-surface-muted border 
              border-border rounded-lg p-1"
            >
              <input
                type="text"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                placeholder="Buscar por nombre de producto..."
                className="w-full bg-transparent text-xs px-3 py-1.5 text-content-main placeholder:text-content-muted focus:outline-none"
              />
              <Button type="submit" size="sm" variant="ghost" className="h-7 px-2 text-content-muted">
                <Search className="h-4 w-4" />
              </Button>
            </form>

            {/* Selector de Categorías en Mobile
            <div className="flex flex-col gap-1">
              <span className="text-xs font-semibold text-content-muted">Buscar por categoría:</span>
              <select
                value={selectedCategory}
                onChange={(e) => setSelectedCategory(e.target.value)}
                className="bg-surface-muted border border-border rounded-md p-2 text-xs text-content-main"
              >
                {categories.map((cat) => (
                  <option key={cat} value={cat}>{cat}</option>
                ))}
              </select>
            </div> */}

            {/* Links de Usuario en Mobile */}
            {isAuthenticated ? (
              <div className="flex flex-col gap-2 pt-2 border-t border-border">
                <p className="text-xs font-semibold text-content-muted">Mi Cuenta:</p>

                <Link
                  to="/dashboard"
                  onClick={() => setIsMobileMenuOpen(false)}
                  className="flex items-center gap-2 text-sm font-medium py-1 hover:text-primary"
                >
                  <User className="h-4 w-4 text-primary" /> Profile
                </Link>

                {user?.roles?.includes('Admin') && (
                  <Link
                    to="/admin"
                    onClick={() => setIsMobileMenuOpen(false)}
                    className="flex items-center gap-2 text-sm text-warning font-medium py-1"
                  >
                    <LayoutDashboard className="h-4 w-4" /> Admin Dashboard
                  </Link>
                )}
              </div>
            ) : null}

            {/* Acciones de Sistema (Theme & Auth) */}
            <div className="flex items-center justify-between pt-3 border-t border-border">
              <Button variant="outline" size="sm" onClick={toggleTheme} className="gap-2">
                {theme === 'dark' ? <Sun className="h-4 w-4 text-warning" /> : <Moon className="h-4 w-4 text-primary" />}
                <span className="text-xs">Tema</span>
              </Button>

              {isAuthenticated ? (
                <Button variant="ghost" size="sm" onClick={logout} className="gap-2 text-danger">
                  <LogOut className="h-4 w-4" /> Logout
                </Button>
              ) : (
                <Link to="/login" onClick={() => setIsMobileMenuOpen(false)}>
                  <Button size="sm" className="gap-2 bg-primary text-primary-foreground">
                    <LogIn className="h-4 w-4" /> Login
                  </Button>
                </Link>
              )}
            </div>

          </div>
        )}
      </header>

      {/* Contenedor principal global centralizado */}
      <main className="flex-1 w-full bg-surface px-4 md:px-0
        max-w-full md:max-w-6xl mx-auto py-6 md:py-8 space-y-6">
        <Outlet />
      </main>
    </>
  );
}