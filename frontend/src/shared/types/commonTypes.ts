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