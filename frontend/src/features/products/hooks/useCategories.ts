import { useState, useEffect } from 'react';
import { Category } from '@products/types/categoryTypes';
import { categoryService } from '@products/services/categoryService';

export function useCategories() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    categoryService.fetchCategories()
      .then(setCategories)
      .catch(console.error)
      .finally(() => setIsLoading(false));
  }, []);

  return { categories, isLoading };
}