import { useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { ChevronRight, Layers, Check } from 'lucide-react';
import { useCategories } from '../hooks/useCategories';

export function CategoryFilter() {
  const { categorySlug, subcategorySlug } = useParams<{
    categorySlug?: string;
    subcategorySlug?: string;
  }>();
  
  const { categories, isLoading } = useCategories();
  const [openCategoryId, setOpenCategoryId] = useState<number | null>(null);

  const toggleCategory = (id: number) => {
    setOpenCategoryId((prev) => (prev === id ? null : id));
  };

  if (isLoading) {
    return (
      <div className="p-4 bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 space-y-3 animate-pulse">
        <div className="h-4 bg-slate-200 dark:bg-slate-800 rounded w-1/2" />
        <div className="h-8 bg-slate-100 dark:bg-slate-800/60 rounded" />
      </div>
    );
  }

  return (
    <div className="p-4 bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 space-y-3 shadow-sm">
      <div className="flex items-center justify-between border-b border-slate-100 dark:border-slate-800 pb-2.5">
        <h2 className="text-sm font-semibold text-slate-900 dark:text-slate-100 flex items-center gap-2">
          <Layers className="h-4 w-4 text-violet-600 dark:text-violet-400" /> Categorías
        </h2>
        {(categorySlug || subcategorySlug) && (
          <Link
            to="/products"
            className="text-[11px] font-medium text-violet-600 dark:text-violet-400 hover:underline cursor-pointer"
          >
            Limpiar
          </Link>
        )}
      </div>

      <div className="space-y-1">
        {categories.map((category) => {
          const isOpen = openCategoryId === category.id;
          const subcategoriesList = category.subcategories || [];
          const hasSubcategories = subcategoriesList.length > 0;
          // Es activo si coincide la categoría raíz y NO hay subcategoría seleccionada
          const isCategoryActive = categorySlug === category.slug && !subcategorySlug;

          return (
            <div key={category.id} className="space-y-1">
              {/* Enlace Categoría Padre */}
              <div
                className={`flex items-center justify-between p-2 rounded-lg text-xs font-medium transition-colors ${
                  isCategoryActive
                    ? 'bg-violet-50 text-violet-700 dark:bg-violet-950/40 dark:text-violet-300'
                    : 'text-slate-700 dark:text-slate-300 hover:bg-slate-100 dark:hover:bg-slate-800/60'
                }`}
              >
                <Link
                  to={`/products/${category.slug}`}
                  className="flex-1 flex items-center gap-1.5 cursor-pointer"
                >
                  {isCategoryActive && <Check className="h-3 w-3 text-violet-600" />}
                  {category.name}
                </Link>

                {hasSubcategories && (
                  <button
                    type="button"
                    onClick={() => toggleCategory(category.id)}
                    className="p-1 rounded hover:bg-slate-200 dark:hover:bg-slate-700 text-slate-400 cursor-pointer"
                  >
                    <ChevronRight
                      className={`h-3.5 w-3.5 transition-transform duration-200 ${isOpen ? 'rotate-90' : ''}`}
                    />
                  </button>
                )}
              </div>

              {/* Enlaces Subcategorías */}
              {hasSubcategories && isOpen && (
                <div className="ml-3 pl-2.5 border-l-2 border-slate-100 dark:border-slate-800 space-y-1 my-1">
                  {subcategoriesList.map((sub) => {
                    const isSubActive = subcategorySlug === sub.slug;
                    return (
                      <Link
                        key={sub.id}
                        to={`/products/${category.slug}/${sub.slug}`}
                        className={`block px-2 py-1.5 rounded text-[11px] transition-colors cursor-pointer ${
                          isSubActive
                            ? 'font-semibold text-violet-600 dark:text-violet-400 bg-violet-50/50 dark:bg-violet-950/20'
                            : 'text-slate-500 dark:text-slate-400 hover:text-slate-900 dark:hover:text-slate-200'
                        }`}
                      >
                        {sub.name}
                      </Link>
                    );
                  })}
                </div>
              )}
            </div>
          );
        })}
      </div>
    </div>
  );
}