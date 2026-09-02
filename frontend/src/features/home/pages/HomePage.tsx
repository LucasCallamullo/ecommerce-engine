import { useNavigate } from 'react-router-dom';
import { useAuth } from '@features/auth/context/AuthContext';
import { Button } from '@shared/components/ui/button';
import { Badge } from '@shared/components/ui/badge';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@shared/components/ui/card';
import { Store, ShoppingBag, LayoutDashboard, LogIn, LogOut, ShieldCheck } from 'lucide-react';

/**
 * Public Landing Home Page presenting navigation choices, theme toggle, and live auth state.
 */
export function HomePage() {
  const navigate = useNavigate();
  const { isAuthenticated, user, logout } = useAuth();

  return (
    // 1. Fondo adaptable: bg-slate-50 en modo claro, dark:bg-slate-950 en modo oscuro
    <div className="min-h-screen bg-slate-50 text-slate-900 dark:bg-slate-950 dark:text-slate-100 flex flex-col items-center justify-center p-6 transition-colors duration-200">
      
      {/* 2. Tarjeta con soporte para ambos modos */}
      <Card className="max-w-lg w-full shadow-xl dark:shadow-2xl bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-800 text-slate-900 dark:text-slate-100">
        <CardHeader className="text-center space-y-2">
          <div className="flex justify-center items-center gap-2">
            <Store className="h-8 w-8 text-indigo-600 dark:text-indigo-400" />
            <CardTitle className="text-2xl font-bold">E-Commerce Platform</CardTitle>
          </div>
          <CardDescription className="text-slate-600 dark:text-slate-400">
            Central navigation and real-time user session status.
          </CardDescription>

          <div className="pt-2 flex justify-center items-center gap-2">
            <Badge
              className={
                isAuthenticated
                  ? 'bg-emerald-100 text-emerald-800 border-emerald-300 dark:bg-emerald-950 dark:text-emerald-300 dark:border-emerald-800'
                  : 'bg-slate-200 text-slate-700 dark:bg-slate-800 dark:text-slate-300'
              }
            >
              {isAuthenticated ? `Active Session (${user?.email})` : 'Guest Visitor'}
            </Badge>
          </div>
        </CardHeader>

        <CardContent className="space-y-4">
          <div className="grid grid-cols-1 gap-2">
            <Button
              variant="outline"
              className="justify-start gap-3 bg-slate-100 hover:bg-slate-200 text-slate-800 border-slate-200 dark:bg-slate-950 dark:border-slate-800 dark:hover:bg-slate-800 dark:text-slate-200"
              onClick={() => navigate('/products')}
            >
              <ShoppingBag className="h-4 w-4 text-indigo-600 dark:text-indigo-400" />
              Product Catalog (/products)
            </Button>

            <Button
              variant="outline"
              className="justify-start gap-3 bg-slate-100 hover:bg-slate-200 text-slate-800 border-slate-200 dark:bg-slate-950 dark:border-slate-800 dark:hover:bg-slate-800 dark:text-slate-200"
              onClick={() => navigate('/dashboard')}
            >
              <LayoutDashboard className="h-4 w-4 text-emerald-600 dark:text-emerald-400" />
              User Dashboard (/dashboard)
            </Button>

            <Button
              variant="outline"
              className="justify-start gap-3 bg-slate-100 hover:bg-slate-200 text-slate-800 border-slate-200 dark:bg-slate-950 dark:border-slate-800 dark:hover:bg-slate-800 dark:text-slate-200"
              onClick={() => navigate('/admin')}
            >
              <ShieldCheck className="h-4 w-4 text-amber-600 dark:text-amber-400" />
              Protected Admin Portal (/admin)
            </Button>
          </div>

          <div className="pt-4 border-t border-slate-200 dark:border-slate-800 flex justify-end">
            {isAuthenticated ? (
              <Button variant="destructive" size="sm" onClick={logout} className="gap-2">
                <LogOut className="h-4 w-4" /> Log Out
              </Button>
            ) : (
              <Button size="sm" onClick={() => navigate('/login')} className="gap-2 bg-indigo-600 hover:bg-indigo-500 text-white">
                <LogIn className="h-4 w-4" /> Sign In / Register
              </Button>
            )}
          </div>
        </CardContent>
      </Card>
    </div>
  );
}