import { useAuth } from '@features/auth/context/AuthContext';
import { Card, CardContent, CardHeader, CardTitle } from '@shared/components/ui/card';
import { ShieldCheck, Settings, Users } from 'lucide-react';

/**
 * Restricted Administration Portal accessible exclusively to users with the 'Admin' role.
 */
export function AdminPage() {
  const { user } = useAuth();

  return (
    <div className="p-6 md:p-8 space-y-6 max-w-5xl mx-auto">
      {/* Page Header */}
      <header className="flex justify-between items-center border-b border-slate-200 dark:border-slate-800 pb-4">
        <div className="flex items-center gap-3">
          <ShieldCheck className="h-6 w-6 text-amber-600 dark:text-amber-400" />
          <h1 className="text-xl font-bold text-slate-900 dark:text-slate-100">
            Admin Control Center
          </h1>
        </div>
      </header>

      {/* Main Admin Card */}
      <Card className="bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-800 shadow-xs">
        <CardHeader className="border-b border-slate-100 dark:border-slate-800/60 pb-4">
          <CardTitle className="text-slate-900 dark:text-slate-100 text-lg">
            Welcome, Administrator {user?.firstName}
          </CardTitle>
        </CardHeader>
        <CardContent className="pt-6 space-y-6">
          <p className="text-sm text-slate-600 dark:text-slate-400">
            You are viewing this privileged view because your account contains the{' '}
            <code className="text-amber-600 dark:text-amber-400 font-mono font-medium bg-amber-50 dark:bg-amber-950/40 px-1.5 py-0.5 rounded border border-amber-200 dark:border-amber-900/50">
              Admin
            </code>{' '}
            role.
          </p>

          {/* Feature Grid */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div className="p-4 bg-slate-50 dark:bg-slate-950/60 border border-slate-200 dark:border-slate-800 rounded-lg flex items-center gap-3 transition-colors">
              <Users className="h-6 w-6 text-indigo-600 dark:text-indigo-400 shrink-0" />
              <div>
                <h3 className="font-semibold text-sm text-slate-900 dark:text-slate-100">
                  User Management
                </h3>
                <p className="text-xs text-slate-500 dark:text-slate-400">
                  Inspect system users and assign roles
                </p>
              </div>
            </div>

            <div className="p-4 bg-slate-50 dark:bg-slate-950/60 border border-slate-200 dark:border-slate-800 rounded-lg flex items-center gap-3 transition-colors">
              <Settings className="h-6 w-6 text-indigo-600 dark:text-indigo-400 shrink-0" />
              <div>
                <h3 className="font-semibold text-sm text-slate-900 dark:text-slate-100">
                  System Settings
                </h3>
                <p className="text-xs text-slate-500 dark:text-slate-400">
                  Configure global platform options
                </p>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}