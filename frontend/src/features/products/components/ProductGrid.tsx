import { Product } from '@products/types/productTypes';
import { ProductCard } from '@products/components/ProductCard';
import { JSX } from 'react/jsx-runtime';

interface ProductGridProps {
  /** List of products retrieved from the catalog API */
  products: Product[];
}

/**
 * Renders a responsive grid of product variants.
 * 
 * Flattens all product variants into individual displayable items and assigns 
 * a fallback image strategy: uses the variant's main image if available, 
 * otherwise defaults to the parent product's main image.
 *
 * @param {ProductGridProps} props - Component props containing the list of products
 * @returns {JSX.Element} The rendered product grid or an empty state message
 */
export function ProductGrid({ products }: ProductGridProps): JSX.Element {
  // Flatten variants while assigning each variant its main image or the parent fallback image
  const allVariants = products.flatMap((product) =>
    product.variants.map((variant) => ({
      variant,
      slug: product.slug,
      mainImage: variant.mainImageUrl ?? product.mainImageUrl,
    }))
  );

  if (allVariants.length === 0) {
    return (
      <div className="text-center py-12 text-slate-500 dark:text-slate-400">
        No se encontraron productos disponibles.
      </div>
    );
  }

  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-3 gap-4 md:gap-6">
      {allVariants.map((item) => (
        // 'variant', 'slug', 'mainImage' are passed automatically
        <ProductCard key={item.variant.id} {...item} />
      ))}
    </div>
  );
}