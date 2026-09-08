import { useState, useEffect, useCallback } from 'react';
import { productService } from '@products/services/productService';
import { ProductQueryParams, PaginatedProducts } from '@products/types/productTypes';
import { handleAsyncRequest } from '@/shared/utils/handleAsync';

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