import { apiClient } from '@shared/api/apiClient';
import { CategoriesApiResponse, Category } from '@products/types/categoryTypes';

export const categoryService = {
  fetchCategories: async () : Promise<Category[]> => {
    const response = await apiClient.get<CategoriesApiResponse>('/v1/categories/subcategories');
    
    var categories = response.data?.data || [];
    return categories;
  },
};