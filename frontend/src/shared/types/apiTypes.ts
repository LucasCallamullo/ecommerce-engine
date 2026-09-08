/**
 * Standard HTTP response envelope for the API.
 * Supports both successful payloads with typed data and RFC 7807-style error details.
 *
 * @template T - The type of the payload contained within the `data` property.
 */
export interface ApiResponse<T = unknown> {
  /** Indicates whether the operation was successfully processed by the server. */
  success: boolean;

  /** HTTP status code returned by the server (e.g., 200, 201, 400, 409, 500). */
  status: number;

  /** Primary payload returned on successful operations (2xx status codes). */
  data?: T;

  /** Detailed human-readable explanation specific to this occurrence of the problem. */
  detail?: string | null;

  /** Dictionary of field-specific validation errors (e.g., FluentValidation/Spring Binding) or error list. */
  errors?: Record<string, string[]> | string[] | null;

  /** Request URI path invoked on the server, used for traceability and logging. */
  path: string;

  /** ISO UTC timestamp indicating when the request was processed by the server. */
  timestamp?: string;
}


/**
 * Generic paginated response wrapper for list endpoints across the API.
 * 
 * @template T - The type of the individual items contained within the `items` collection.
 */
export interface PaginatedResponse<T> {
  /** Array of entity items for the current requested page. */
  items: T[];

  /** Total number of records matching the query criteria in the database. */
  totalCount: number;

  /** Current active page index (typically 1-based). */
  pageNumber: number;

  /** Maximum amount of items configured to return per page. */
  pageSize: number;

  /** Optional metadata or contextual payload provided by the backend endpoint. */
  extraData: unknown | null;

  /** Total calculated number of available pages based on `totalCount` and `pageSize`. */
  totalPages: number;

  /** Flag indicating if a subsequent page exists after the current one. */
  hasNextPage: boolean;

  /** Flag indicating if a preceding page exists prior to the current one. */
  hasPreviousPage: boolean;
}