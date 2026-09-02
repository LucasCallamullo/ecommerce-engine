import { useAuth } from '@features/auth/context/AuthContext';
import { Card, CardContent, CardHeader, CardTitle } from '@shared/components/ui/card';
import { LayoutDashboard, User, Shield } from 'lucide-react';

/**
 * Protected User Dashboard accessible to any authenticated user.
 */
export function DashboardPage() {
  const { user } = useAuth();

  return (
    <div className="p-6 md:p-8 space-y-6 max-w-5xl mx-auto">
      {/* Page Header */}
      <header className="flex justify-between items-center border-b border-slate-200 dark:border-slate-800 pb-4">
        <div className="flex items-center gap-3">
          <LayoutDashboard className="h-6 w-6 text-indigo-600 dark:text-emerald-400" />
          <h1 className="text-xl font-bold text-slate-900 dark:text-slate-100">User Dashboard</h1>
        </div>
      </header>

      {/* Profile & Roles Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        {/* User Profile Card */}
        <Card className="bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-800 shadow-xs">
          <CardHeader className="flex flex-row items-center gap-3 border-b border-slate-100 dark:border-slate-800/60 pb-3">
            <User className="h-5 w-5 text-indigo-600 dark:text-indigo-400" />
            <CardTitle className="text-slate-900 dark:text-slate-100 text-base">User Profile</CardTitle>
          </CardHeader>
          <CardContent className="pt-4 space-y-2.5 text-sm text-slate-600 dark:text-slate-300">
            <p><strong className="text-slate-900 dark:text-slate-100 font-medium">Name:</strong> {user?.firstName} {user?.lastName}</p>
            <p><strong className="text-slate-900 dark:text-slate-100 font-medium">Email:</strong> {user?.email}</p>
            <p><strong className="text-slate-900 dark:text-slate-100 font-medium">Cellphone:</strong> {user?.cellphone}</p>
            <p><strong className="text-slate-900 dark:text-slate-100 font-medium">DNI:</strong> {user?.dni}</p>
          </CardContent>
        </Card>

        {/* Roles Card */}
        <Card className="bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-800 shadow-xs">
          <CardHeader className="flex flex-row items-center gap-3 border-b border-slate-100 dark:border-slate-800/60 pb-3">
            <Shield className="h-5 w-5 text-amber-600 dark:text-amber-400" />
            <CardTitle className="text-slate-900 dark:text-slate-100 text-base">Assigned Roles</CardTitle>
          </CardHeader>
          <CardContent className="pt-4 flex gap-2 flex-wrap">
            {user?.roles?.map((role) => (
              <span 
                key={role} 
                className="px-3 py-1 bg-slate-100 dark:bg-slate-800 border border-slate-200 dark:border-slate-700 text-slate-700 dark:text-slate-200 text-xs rounded-full font-mono font-medium"
              >
                {role}
              </span>
            ))}
          </CardContent>
        </Card>
      </div>
    </div>
  );
}