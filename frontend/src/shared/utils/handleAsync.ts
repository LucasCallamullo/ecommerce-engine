import { ApiResponse } from '@shared/types/apiTypes';

/**
 * State updater callbacks required to manage the lifecycle of an asynchronous request.
 *
 * @template T - The type of data expected upon a successful API response.
 */
export interface AsyncState<T> {
  /** Setter function to update the main payload state. */
  setData: (data: T | null) => void;

  /** Setter function to toggle the active loading indicator state. */
  setIsLoading: (loading: boolean) => void;

  /** Setter function to capture or clear error message strings. */
  setError: (error: string | null) => void;

  /** Optional localized fallback message used if the API response omits error details. */
  fallbackError?: string;
}

/**
 * Executes an asynchronous API request wrapper while automatically handling
 * common state transitions (`isLoading`, `error`, `data`) and exceptions.
 *
 * @template T - The payload type contained within the `ApiResponse<T>`.
 * 
 * @param requestFn - A callback function executing an HTTP request returning a `Promise<ApiResponse<T>>`.
 * @param state - An object containing state setters (`setData`, `setIsLoading`, `setError`) and optional fallback configuration.
 *
 * @returns Promise<void> A Promise that resolves once all state updates and async operations complete.
 *
 * @example
 * ```ts
 * await handleAsyncRequest(
 *   () => productService.getProducts(params),
 *   {
 *     setData,
 *     setIsLoading,
 *     setError,
 *     fallbackError: 'Failed to retrieve product catalog.'
 *   }
 * );
 * ```
 */
export async function handleAsyncRequest<T>(
  requestFn: () => Promise<ApiResponse<T>>,
  { setData, setIsLoading, setError, fallbackError }: AsyncState<T>
): Promise<void> {
  try {
    // 1. Reset state indicators prior to launching network I/O
    setIsLoading(true);
    setError(null);

    // 2. Await API Promise execution
    const response = await requestFn();

    // 3. Process application-level response envelope
    if (response.success && response.data) {
      setData(response.data);
    } else {
      // Fallback hierarchy: API detail -> Provided fallback string -> Generic message
      setError(response.detail || fallbackError || 'An unexpected error occurred.');
    }
  } catch {
    // 4. Catch network failures or unhandled runtime exceptions
    setError('Could not connect to the server. Please check your connection and try again.');
  } finally {
    // 5. Always finalize the execution phase
    setIsLoading(false);
  }
}