import { useState, useEffect, useCallback, useMemo } from 'react';
import { productService } from '@products/services/productService';
import { ProductQueryParams, PaginatedProducts, ProductDetail, ProductVariant } from '@products/types/productTypes';
import { handleAsyncRequest } from '@/shared/utils/handleAsync';
import { useSearchParams } from 'react-router-dom';

/** Default query parameters to prevent creating inline object references on every render */
const DEFAULT_PARAMS: ProductQueryParams = {
  pageNumber: 1,
  pageSize: 50,
}; 

/**
 * Custom hook to manage product catalog fetching, loading states, and error handling.
 * 
 * @param params Optional criteria to filter and paginate product queries.
 * @returns An object containing catalog `data`, `isLoading` status, `error` message, and a `refetch` trigger.
 */
export function useProducts(params?: ProductQueryParams) {
  // 1. Reactive State Declarations
  const [data, setData] = useState<PaginatedProducts | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  // 2. Value Serialization:
  // Converts parameters into a primitive string key so React compares by VALUE rather than object REFERENCE.
  const queryKey = JSON.stringify(params ?? DEFAULT_PARAMS);

  // 3. Memoized Fetch Function:
  // useCallback caches this function reference in memory. It only recreates if `queryKey` changes.
  const fetchProducts = useCallback(async (overrideParams?: ProductQueryParams) => {
    const activeParams = overrideParams ?? (params ? params : DEFAULT_PARAMS);
    
    await handleAsyncRequest(
      () => productService.getProducts(activeParams),
      { setData, setIsLoading, setError, fallbackError: 'Failed to load catalog.' }
    );
  }, [queryKey]); // Watch list: Re-create fetchProducts ONLY if queryKey changes

  // 4. Mount Side Effect:
  // Triggers the initial network fetch after the UI renders.
  useEffect(() => {
    fetchProducts();
  }, [fetchProducts]); // Watch list: Only re-run if the fetchProducts function reference changes

  // 5. Encapsulated Output:
  // Returns current state snapshot and the manual execution handler to the consuming component
  return {
    data,
    isLoading,
    error,
    refetch: fetchProducts,
  };
}



export function useProductDetail(slug?: string) {
  const [product, setProduct] = useState<ProductDetail | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [searchParams, setSearchParams] = useSearchParams();

  useEffect(() => {
    if (!slug) return;
    let isMounted = true;

    const fetchProduct = async () => {
      setIsLoading(true);
      setError(null);
      try {
        const res = await productService.getProductBySlug(slug);
        if (isMounted && res.success && res.data) {
          setProduct(res.data);
        }
      } catch {
        if (isMounted) setError('No se pudo cargar el producto.');
      } finally {
        if (isMounted) setIsLoading(false);
      }
    };

    fetchProduct();
    return () => {
      isMounted = false;
    };
  }, [slug]);

  // Active variant derived from ?variant=id or the first one available in stock
  const selectedVariant: ProductVariant | null = useMemo(() => {
    if (!product?.variants?.length) return null;

    const variantIdParam = Number(searchParams.get('variant'));
    if (variantIdParam) {
      const found = product.variants.find((v) => v.id === variantIdParam && v.isActive);
      if (found) return found;
    }

    return (
      product.variants.find((v) => v.isActive && v.stock > 0) ??
      product.variants.find((v) => v.isActive) ??
      product.variants[0]
    );
  }, [product, searchParams]);

  // Change the variant by updating the URL without reloading
  const selectVariant = (variantId: number) => {
    setSearchParams((prev) => {
      const next = new URLSearchParams(prev);
      next.set('variant', String(variantId));
      return next;
    }, { replace: true });
  };

  return {
    product,
    selectedVariant,
    selectVariant,
    isLoading,
    error,
  };
}