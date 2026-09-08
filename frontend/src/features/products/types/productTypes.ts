import { ApiResponse, PaginatedResponse } from '@/shared/types/apiTypes';

export interface ProductVariant {
  id: number;
  productId: number;
  name: string;
  mainImageUrl: string | null;

  discountPercentageArs: number;
  priceArs: number;
  finalPriceArs: number;
  comparisonPriceArs?: number | null;
  
  stock: number;
  isActive: boolean;
  size: string | null;
  color: string | null;
  hexColor: string | null;
  createdAt: string;
  updatedAt: string | null;
}

export interface Product {
  id: number;
  slug: string;
  mainImageUrl: string | null;
  categoryId: number;
  subcategoryId: number | null;
  brandId: number | null;
  variants: ProductVariant[];
  isActive?: boolean;
}

/** Type alias for paginated Product queries. */
export type PaginatedProducts = PaginatedResponse<Product>;
export type ProductsApiResponse = ApiResponse<PaginatedProducts>;


/**
 * Criteria parameters used to filter, sort, and paginate product catalog queries.
 * Maps directly to the backend's `ProductFilterQuery` record DTO.
 */
export interface ProductQueryParams {
  categoryId?: number | null;
  subcategoryId?: number | null;
  brandId?: number | null;
  minPrice?: number | null;
  maxPrice?: number | null;
  searchTerm?: string | null;
  sortBy?: 'price_asc' | 'price_desc' | 'name_asc' | 'name_desc' | string | null;
  pageNumber?: number;
  pageSize?: number;
  categorySlug?: string | null;
  subcategorySlug?: string | null;
}