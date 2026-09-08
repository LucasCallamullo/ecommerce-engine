import { apiClient } from '@shared/api/apiClient';
import { ProductsApiResponse, ProductQueryParams } from '@products/types/productTypes';

export const productService = {
  getProducts: async (params?: ProductQueryParams): Promise<ProductsApiResponse> => {
    // Asignamos un objeto vacío por defecto para evitar 'undefined'
    const { categorySlug, subcategorySlug, ...queryParams } = params ?? {};

    // 1. Determina la URL base según la presencia de Slugs
    let url = '/v1/products';

    if (categorySlug && subcategorySlug) {
      url = `/v1/categories/${categorySlug}/${subcategorySlug}`;
    } else if (categorySlug) {
      url = `/v1/categories/${categorySlug}`;
    }

    // 2. Envía únicamente los parámetros restantes en las query string
    const response = await apiClient.get<ProductsApiResponse>(url, {
      params: queryParams,
    });

    return response.data;
  },
};