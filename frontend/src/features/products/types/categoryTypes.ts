import { ApiResponse } from "@/shared/types/apiTypes";

export interface Subcategory {
  id: number;
  name: string;
  slug: string;
  parentCategoryId: number;
}

export interface Category {
  id: number;
  name: string;
  slug: string;
  subcategories?: Subcategory[] | null;
}

export type CategoriesApiResponse = ApiResponse<Category[]>;