import { Link } from 'react-router-dom';
import { ShoppingCart, Maximize2, Image as ImageIcon } from 'lucide-react';

import { Button } from '@shared/components/ui/button';
import { Card, CardContent, CardFooter, CardHeader } from '@shared/components/ui/card';

import { ProductVariant } from '@products/types/productTypes';

interface ProductCardProps {
  variant: ProductVariant;
  mainImage: string | null;
  slug: string;
}

export function ProductCard({ variant, mainImage, slug }: ProductCardProps) {
  const formatPrice = (amount: number) => {
    return new Intl.NumberFormat('es-AR', {
      style: 'currency',
      currency: 'ARS',
      maximumFractionDigits: 0,
    }).format(amount);
  };

  const isOutOfStock = !variant.isActive || variant.stock <= 0;
  const activePrice = Math.min(variant.priceArs, variant.finalPriceArs);
  const originalPrice = Math.max(variant.comparisonPriceArs || 0, variant.priceArs);
  const hasDiscount = variant.discountPercentageArs > 0;

  const handleQuickView = (e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();
    alert('openmodal');
  };

  return (
    <Card className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 
      flex flex-col justify-between overflow-hidden hover:shadow-md transition-shadow duration-200 p-0 m-0">
      
      {/* HEADER: Imagen pegada al borde superior */}
      <CardHeader className="p-0">
        <div className="relative aspect-4/3 w-full bg-slate-50 dark:bg-slate-800/50 overflow-hidden group ">
          <Link 
            to={`/products/${slug}`} 
            className="w-full h-full flex items-center justify-center cursor-pointer"
          >
            {mainImage ? (
              <img
                src={mainImage}
                alt={variant.name}
                className="w-full h-full object-contain group-hover:scale-105 transition-transform duration-300"
              />
            ) : (
              <div className="flex flex-col items-center gap-1 text-slate-400 dark:text-slate-600">
                <ImageIcon className="h-6 w-6 stroke-[1.5]" />
                <span className="text-[10px]">Sin imagen</span>
              </div>
            )}
          </Link>

          {/* Botón Quick View (Esquina superior derecha) */}
          <button
            onClick={handleQuickView}
            title="Vista rápida"
            className="absolute top-2 right-2 z-20 p-1.5 rounded-md bg-white/90 dark:bg-slate-900/90 
              text-slate-700 dark:text-slate-200 shadow-sm opacity-0 group-hover:opacity-100 
              hover:bg-violet-600 hover:text-white dark:hover:bg-violet-600 dark:hover:text-white 
              transition-all duration-200 cursor-pointer"
          >
            <Maximize2 className="h-3.5 w-3.5" />
          </button>

          {/* Badge de Stock (Esquina inferior izquierda) */}
          <div className="absolute bottom-2 left-2 z-10">
            {isOutOfStock ? (
              <span className="inline-block bg-red-600 text-white text-[9px] font-extrabold px-2 py-0.5 rounded-full uppercase tracking-wider shadow-sm">
                Sin Stock
              </span>
            ) : variant.stock <= 3 ? (
              <span className="inline-block bg-amber-500 text-white text-[9px] font-extrabold px-2 py-0.5 rounded-full uppercase tracking-wider shadow-sm">
                Últimas u.
              </span>
            ) : (
              <span className="inline-block bg-emerald-500 text-white text-[9px] font-extrabold px-2 py-0.5 rounded-full uppercase tracking-wider shadow-sm">
                Disponible
              </span>
            )}
          </div>
        </div>
      </CardHeader>

      {/* CONTENT: Nombre y Precios (Padding reducido) */}
      <CardContent className="p-3 space-y-2 flex-1 flex flex-col justify-between">
        <h3 className="text-xs sm:text-sm font-medium text-slate-800 dark:text-slate-200 line-clamp-2 leading-tight">
          {variant.name}
        </h3>

        <div className="space-y-0.5">
          <div className="flex items-center justify-between gap-1">
            <span className="text-lg font-bold text-slate-900 dark:text-slate-100">
              {formatPrice(activePrice)}
            </span>

            {hasDiscount && (
              <span className="bg-violet-600 text-white text-[10px] font-extrabold px-1.5 py-0.5 rounded shadow-sm">
                {variant.discountPercentageArs}% OFF
              </span>
            )}
          </div>

          {originalPrice && originalPrice > activePrice && (
            <p className="text-[11px] text-slate-400 dark:text-slate-500 line-through font-medium">
              {formatPrice(originalPrice)}
            </p>
          )}

          <p className="text-[10px] text-slate-400 dark:text-slate-500">
            Precio final con todos los medios de pago
          </p>
        </div>
      </CardContent>

      {/* FOOTER: Botón Violeta Compacto */}
      <CardFooter className="p-3 pt-0 flex items-center justify-center">
        <Button
          disabled={isOutOfStock}
          className="w-full gap-1.5 bg-violet-600 hover:bg-violet-500 text-white font-bold 
            text-xs h-8 cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
        >
          <ShoppingCart className="h-3.5 w-3.5" />
          {isOutOfStock ? 'Sin Stock' : 'Agregar'}
        </Button>
      </CardFooter>
    </Card>
  );
}