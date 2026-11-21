import { Link, useParams } from 'react-router-dom';
import { useState } from 'react';
import { ChevronDown, ChevronRight } from 'lucide-react';

import { useCategories } from '@/features/products/hooks/useCategories';

export function CategoryDropdownTree() {
  const { categories, isLoading } = useCategories();
  const { categorySlug, subcategorySlug } = useParams<{
    categorySlug?: string;
    subcategorySlug?: string;
  }>();

  const [isOpen, setIsOpen] = useState(false);
  const [openCategoryId, setOpenCategoryId] = useState<number | null>(null);

  const toggleCategory = (id: number) => {
    setOpenCategoryId((prev) => (prev === id ? null : id));
  };

  if (isLoading) return null;

  return (
    <div className="relative border-r border-border shrink-0 text-sm">
      <button
        type="button"
        onClick={() => setIsOpen(!isOpen)}
        onBlur={() => setTimeout(() => setIsOpen(false), 200)}
        className="flex items-center gap-1.5 px-3 py-1.5 font-medium text-content-main hover:text-primary"
      >
        <span>Todas las Categorías</span>
        <ChevronDown
          className={`h-3.5 w-3.5 text-content-muted transition-transform ${
            isOpen ? 'rotate-180' : ''
          }`}
        />
      </button>

      {isOpen && (
        <ul className="absolute top-full left-0 mt-2 w-56 bg-surface-raised border border-border 
          rounded-lg shadow-lg py-1 z-50">
          {categories.map((category) => {
            const isExpanded = openCategoryId === category.id;
            const subcategoriesList = category.subcategories || [];
            const hasSubcategories = subcategoriesList.length > 0;
            const isCategoryActive =
              categorySlug === category.slug && !subcategorySlug;

            return (
              <li key={category.id}>
                {/* Fila principal: link + chevron para expandir */}
                <div
                  className={`flex items-center justify-between hover:bg-surface-muted ${
                    isCategoryActive ? 'bg-surface-muted text-primary' : ''
                  }`}
                >
                  <Link
                    to={`/products/${category.slug}`}
                    onClick={() => setIsOpen(false)}
                    className="flex-1 px-3 py-2 text-left text-content-main hover:text-primary w-min"
                  >
                    {category.name}
                  </Link>

                  {hasSubcategories && (
                    <button
                      type="button"
                      onMouseDown={(e) => e.preventDefault()} // evita que el onBlur cierre el dropdown
                      onClick={() => toggleCategory(category.id)}
                      className="px-2 py-2 text-content-muted hover:text-content-on-main hover:bg-main-600"
                      aria-label={`Expandir ${category.name}`}
                    >
                      <ChevronRight
                        className={`h-3.5 w-3.5 transition-transform ${
                          isExpanded ? 'rotate-90' : ''
                        }`}
                      />
                    </button>
                  )}
                </div>

                {/* Subcategorías */}
                {hasSubcategories && isExpanded && (
                  <ul className="pl-4 pb-1">
                    {subcategoriesList.map((sub) => {
                      const isSubActive =
                        categorySlug === category.slug &&
                        subcategorySlug === sub.slug;

                      return (
                        <li key={sub.id}>
                          <Link
                            to={`/products/${category.slug}/${sub.slug}`}
                            onClick={() => setIsOpen(false)}
                            className={`block px-3 py-1.5 rounded hover:text-primary hover:bg-surface-muted ${
                              isSubActive
                                ? 'text-primary font-medium'
                                : 'text-content-main'
                            }`}
                          >
                            {sub.name}
                          </Link>
                        </li>
                      );
                    })}
                  </ul>
                )}
              </li>
            );
          })}
        </ul>
      )}
    </div>
  );
}