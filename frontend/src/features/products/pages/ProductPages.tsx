import { useParams } from 'react-router-dom';
import { Button } from '@shared/components/ui/button';
import { Loader2, PackageSearch, RefreshCw } from 'lucide-react';

import { useProducts } from '@products/hooks/useProducts';
import { ProductGrid } from '@products/components/ProductGrid';
import { CategoryFilter } from '@products/components/CategoryFilter';


/**
 * Presentational component for displaying the product catalog page.
 * Delegates state management, error handling, and API fetching to the `useProducts` hook.
 */
export function ProductsPage() {
  // Consumes the encapsulated logic and Reactive state from the custom hook
  // const { data, isLoading, error, refetch } = useProducts();

  const { categorySlug, subcategorySlug } = useParams();

  // Se re-ejecuta automáticamente el hook cuando cambian los params de la URL
  const { data, isLoading, error, refetch } = useProducts({
    categorySlug,
    subcategorySlug,
    pageNumber: 1,
    pageSize: 50,
  });

  // 1. Loading State UI
  if (isLoading) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[50vh] gap-3 text-slate-500">
        <Loader2 className="h-8 w-8 animate-spin text-indigo-600" />
        <span className="text-sm font-medium">Loading catalog...</span>
      </div>
    );
  }

  // 2. Error State UI
  if (error) {
    return (
      <div className="p-6 max-w-md mx-auto text-center space-y-4">
        <div className="p-4 bg-red-50 text-red-700 dark:bg-red-950/40 dark:text-red-300 rounded-lg text-sm border border-red-200 dark:border-red-900/50">
          {error}
        </div>
        <Button onClick={() => refetch()} variant="outline" size="sm" className="gap-2 cursor-pointer">
          <RefreshCw className="h-4 w-4" /> Retry
        </Button>
      </div>
    );
  }

  // 3. Success State UI
  return (
    <>
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 border-b border-slate-200 dark:border-slate-800 pb-4">
        <div>
          <h1 className="text-2xl font-bold text-slate-900 dark:text-slate-100 flex items-center gap-2">
            <PackageSearch className="h-6 w-6 text-indigo-600 dark:text-indigo-400" />
            Product Catalog
          </h1>
          <p className="text-xs text-slate-500 dark:text-slate-400 mt-1">
            Showing {data?.items?.length || 0} of {data?.totalCount || 0} products
          </p>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-4 gap-6">
        {/* Sidebar de Filtros (1 columna en lg) */}
        <aside className="lg:col-span-1 space-y-4">
          <CategoryFilter />
        </aside>

        {/* Grilla de Productos (3 columnas en lg) */}
        <main className="lg:col-span-3">
          <ProductGrid products={data?.items || []} />
        </main>
      </div>
    </>
  );
}