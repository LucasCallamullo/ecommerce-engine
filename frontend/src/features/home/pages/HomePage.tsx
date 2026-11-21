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
    <div className="min-h-screen bg-surface text-content-main flex flex-col items-center justify-center p-6 
      transition-colors duration-200">

      <Card className="max-w-lg w-full shadow-xl bg-surface-raised border-border text-content-main">
        <CardHeader className="text-center space-y-2">
          <div className="flex justify-center items-center gap-2">
            <Store className="h-8 w-8 text-primary" />
            <CardTitle className="text-2xl font-bold">E-Commerce Platform</CardTitle>
          </div>
          <CardDescription className="text-content-muted">
            Central navigation and real-time user session status.
          </CardDescription>

          <div className="pt-2 flex justify-center items-center gap-2">
            <Badge
              className={
                isAuthenticated
                  ? 'bg-success/15 text-success border-success/30'
                  : 'bg-surface-muted text-content-muted border-border'
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
              className="justify-start gap-3 bg-surface-muted hover:bg-surface-muted/70 text-content-main border-border"
              onClick={() => navigate('/products')}
            >
              <ShoppingBag className="h-4 w-4 text-primary" />
              Product Catalog (/products)
            </Button>

            <Button
              variant="outline"
              className="justify-start gap-3 bg-surface-muted hover:bg-surface-muted/70 text-content-main border-border"
              onClick={() => navigate('/dashboard')}
            >
              <LayoutDashboard className="h-4 w-4 text-success" />
              User Dashboard (/dashboard)
            </Button>

            <Button
              variant="outline"
              className="justify-start gap-3 bg-surface-muted hover:bg-surface-muted/70 text-content-main border-border"
              onClick={() => navigate('/admin')}
            >
              <ShieldCheck className="h-4 w-4 text-warning" />
              Protected Admin Portal (/admin)
            </Button>
          </div>

          <div className="pt-4 border-t border-border flex justify-end">
            {isAuthenticated ? (
              <Button variant="destructive" size="sm" onClick={logout} className="gap-2">
                <LogOut className="h-4 w-4" /> Log Out
              </Button>
            ) : (
              <Button
                size="sm"
                onClick={() => navigate('/login')}
                className="gap-2 bg-primary hover:bg-primary-hover text-primary-foreground"
              >
                <LogIn className="h-4 w-4" /> Sign In / Register
              </Button>
            )}
          </div>
        </CardContent>
      </Card>
    </div>
  );
}