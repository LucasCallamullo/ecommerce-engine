import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import {
  ShoppingCart,
  ChevronRight,
  Check,
  Tag,
  ShieldCheck,
  Truck,
  PackageCheck,
  Minus,
  Plus,
  Sparkles,
} from 'lucide-react';
import { Button } from '@shared/components/ui/button';
import { useProductDetail } from '../hooks/useProducts';

export function ProductDetailPage() {
  const { productSlug } = useParams<{ productSlug: string }>();
  const { product, selectedVariant, selectVariant, isLoading, error } =
    useProductDetail(productSlug);

  const [quantity, setQuantity] = useState<number>(1);

  // Reinicia la cantidad a 1 cuando cambia la variante seleccionada
  useEffect(() => {
    setQuantity(1);
  }, [selectedVariant?.id]);

  if (isLoading) {
    return (
      <div className="px-4 md:px-0 space-y-4 animate-pulse">
        <div className="h-4 w-48 bg-surface-muted rounded" />
        <div className="grid grid-cols-1 lg:grid-cols-12 gap-6 bg-surface-raised border border-border rounded-2xl p-5">
          <div className="lg:col-span-6 h-90 bg-surface-muted rounded-xl" />
          <div className="lg:col-span-6 space-y-4 py-2">
            <div className="h-4 w-24 bg-surface-muted rounded" />
            <div className="h-8 w-3/4 bg-surface-muted rounded" />
            <div className="h-16 w-1/2 bg-surface-muted rounded" />
            <div className="h-24 w-full bg-surface-muted rounded" />
            <div className="h-11 w-full bg-surface-muted rounded-lg mt-auto" />
          </div>
        </div>
      </div>
    );
  }

  if (error || !product || !selectedVariant) {
    return (
      <div className="p-10 text-center bg-surface-raised border border-border rounded-2xl space-y-4">
        <p className="text-danger font-medium text-base">
          {error ?? 'Producto no encontrado'}
        </p>
        <Link to="/products">
          <Button variant="outline">Volver al catálogo</Button>
        </Link>
      </div>
    );
  }

  const hasDiscount = selectedVariant.discountPercentageArs > 0;
  const originalPrice =
    selectedVariant.comparisonPriceArs &&
    selectedVariant.comparisonPriceArs > selectedVariant.finalPriceArs
      ? selectedVariant.comparisonPriceArs
      : selectedVariant.priceArs;

  const savingsAmount = hasDiscount
    ? Math.max(0, originalPrice - selectedVariant.finalPriceArs)
    : 0;

  const isOutOfStock = selectedVariant.stock <= 0;
  const isLowStock = selectedVariant.stock > 0 && selectedVariant.stock <= 5;

  const handleDecreaseQty = () => setQuantity((prev) => Math.max(1, prev - 1));
  const handleIncreaseQty = () =>
    setQuantity((prev) => Math.min(selectedVariant.stock, prev + 1));

  return (
    <div className="px-4 md:px-0 space-y-3">
      {/* Breadcrumb compacto */}
      <nav className="flex items-center flex-wrap gap-1.5 text-content-muted">
        <Link to="/products" className="hover:text-primary transition-colors">
          Productos
        </Link>
        {product.category && (
          <>
            <ChevronRight className="h-3.5 w-3.5 shrink-0" />
            <Link
              to={`/products/${product.category.slug}`}
              className="hover:text-primary transition-colors"
            >
              {product.category.name}
            </Link>
          </>
        )}
        {product.subcategory && product.category && (
          <>
            <ChevronRight className="h-3.5 w-3.5 shrink-0" />
            <Link
              to={`/products/${product.category.slug}/${product.subcategory.slug}`}
              className="hover:text-primary transition-colors font-medium text-content-main"
            >
              {product.subcategory.name}
            </Link>
          </>
        )}
      </nav>

      {/* Tarjeta Principal optimizada para viewport */}
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-6 lg:gap-8 bg-surface-raised border border-border rounded-2xl p-4 sm:p-6 shadow-sm">
        {/* COLUMNA IZQUIERDA: Galería / Imagen Principal (6 cols) */}
        <div className="lg:col-span-6 flex flex-col gap-3">
          <div className="relative h-75 sm:h-95 lg:h-105 w-full bg-surface-muted/60 rounded-xl overflow-hidden flex items-center justify-center border border-border group">
            {/* Badge flotante de Descuento */}
            {hasDiscount && (
              <div className="absolute top-3 left-3 z-10 flex items-center gap-1.5 px-2.5 py-1 rounded-full bg-success text-white text-xs font-bold shadow-md">
                <Sparkles className="h-3.5 w-3.5" />
                <span>{selectedVariant.discountPercentageArs}% OFF</span>
              </div>
            )}

            {/* Badge de Sin Stock sobre imagen */}
            {isOutOfStock && (
              <div className="absolute top-3 right-3 z-10 px-2.5 py-1 rounded-full bg-danger/90 text-white text-xs font-semibold">
                Sin stock
              </div>
            )}

            {selectedVariant.mainImageUrl ? (
              <img
                src={selectedVariant.mainImageUrl}
                alt={`${product.name} - ${selectedVariant.name}`}
                className="w-full h-full object-contain p-4 transition-transform duration-300 group-hover:scale-105"
              />
            ) : (
              <span className="text-sm text-content-muted">
                Sin imagen disponible
              </span>
            )}
          </div>

          {/* Miniaturas rápidas si las variantes tienen imágenes distintas */}
          {product.variants.length > 1 && (
            <div className="flex items-center gap-2 overflow-x-auto pb-1">
              {product.variants.map((v) => {
                const isSelected = v.id === selectedVariant.id;
                return (
                  <button
                    key={v.id}
                    type="button"
                    onClick={() => selectVariant(v.id)}
                    title={v.name}
                    className={`relative h-14 w-14 rounded-lg border overflow-hidden shrink-0 bg-surface-muted flex items-center justify-center transition-all cursor-pointer ${
                      isSelected
                        ? 'border-primary ring-2 ring-primary/30'
                        : 'border-border opacity-70 hover:opacity-100'
                    }`}
                  >
                    {v.mainImageUrl ? (
                      <img
                        src={v.mainImageUrl}
                        alt={v.name}
                        className="w-full h-full object-contain p-1"
                      />
                    ) : v.hexColor ? (
                      <span
                        className="h-6 w-6 rounded-full border border-border"
                        style={{ backgroundColor: v.hexColor }}
                      />
                    ) : (
                      <span className="text-[10px] font-medium px-1 truncate">
                        {v.name}
                      </span>
                    )}
                  </button>
                );
              })}
            </div>
          )}
        </div>

        {/* COLUMNA DERECHA: Info, Precio, Selectores y Compra (6 cols) */}
        <div className="lg:col-span-6 flex flex-col justify-between space-y-5">
          <div className="space-y-4">
            {/* Marca + Etiqueta de Color/Talle activo */}
            <div className="flex items-center justify-between gap-2 flex-wrap">
              {product.brand ? (
                <span className="inline-flex items-center px-2.5 py-0.5 rounded-md text-xs font-bold uppercase tracking-wider bg-primary/10 text-primary">
                  {product.brand.name}
                </span>
              ) : (
                <span />
              )}

              {/* Estado de Stock compacto */}
              <div className="flex items-center gap-1.5 text-xs font-medium">
                {isOutOfStock ? (
                  <span className="text-danger">Agotado</span>
                ) : isLowStock ? (
                  <span className="text-warning font-semibold">
                    ¡Últimas {selectedVariant.stock} unidades!
                  </span>
                ) : (
                  <span className="inline-flex items-center gap-1 text-success">
                    <PackageCheck className="h-3.5 w-3.5" />
                    En stock ({selectedVariant.stock} disp.)
                  </span>
                )}
              </div>
            </div>

            {/* Título del producto */}
            <div>
              <h1 className="text-xl sm:text-2xl font-bold text-content-main leading-tight">
                {product.name}
              </h1>
              <p className="text-xs text-content-muted mt-1">
                Opción seleccionada:{' '}
                <span className="font-semibold text-content-main">
                  {selectedVariant.name}
                </span>
                {selectedVariant.color && ` · Color: ${selectedVariant.color}`}
                {selectedVariant.size && ` · Talle/Medida: ${selectedVariant.size}`}
              </p>
            </div>

            {/* BLOQUE DE PRECIO Y DESCUENTO DESTACADO */}
            <div className="p-3.5 rounded-xl bg-surface-muted/70 border border-border/80 space-y-1">
              {hasDiscount && (
                <div className="flex items-center gap-2 flex-wrap">
                  <span className="text-sm line-through text-content-muted">
                    ${originalPrice.toLocaleString('es-AR')}
                  </span>
                  <span className="inline-flex items-center gap-1 text-xs font-bold px-2 py-0.5 rounded-md bg-success/15 text-success">
                    <Tag className="h-3 w-3" />
                    -{selectedVariant.discountPercentageArs}% OFF
                  </span>
                  {savingsAmount > 0 && (
                    <span className="text-xs font-medium text-success">
                      (Ahorrás ${savingsAmount.toLocaleString('es-AR')})
                    </span>
                  )}
                </div>
              )}

              <div className="flex items-baseline gap-2">
                <span className="text-3xl font-extrabold tracking-tight text-content-main">
                  ${selectedVariant.finalPriceArs.toLocaleString('es-AR')}
                </span>
                <span className="text-xs text-content-muted">ARS</span>
              </div>
            </div>

            {/* SELECTOR DE VARIANTES (Con muestra de color hexColor y talle) */}
            {product.variants.length > 0 && (
              <div className="space-y-2">
                <div className="flex items-center justify-between">
                  <span className="text-xs font-semibold uppercase tracking-wide text-content-muted">
                    Variantes ({product.variants.length})
                  </span>
                </div>

                <div className="flex flex-wrap gap-2">
                  {product.variants.map((variant) => {
                    const isSelected = variant.id === selectedVariant.id;
                    const noStock = variant.stock <= 0;

                    return (
                      <button
                        key={variant.id}
                        type="button"
                        onClick={() => selectVariant(variant.id)}
                        className={`relative flex items-center gap-2 px-3 py-1.5 rounded-lg border text-xs font-medium transition-all cursor-pointer ${
                          isSelected
                            ? 'border-primary bg-primary/10 text-primary ring-1 ring-primary shadow-xs'
                            : 'border-border bg-surface hover:border-primary/50 text-content-main'
                        } ${noStock ? 'opacity-50' : ''}`}
                      >
                        {variant.hexColor && (
                          <span
                            className="h-4 w-4 rounded-full border border-black/20 dark:border-white/20 shrink-0 shadow-inner"
                            style={{ backgroundColor: variant.hexColor }}
                          />
                        )}
                        <span className="truncate max-w-35">
                          {variant.name}
                        </span>
                        {variant.size && (
                          <span className="px-1.5 py-0.2 rounded bg-surface-muted text-[11px] font-semibold text-content-muted">
                            {variant.size}
                          </span>
                        )}
                        {isSelected && (
                          <Check className="h-3.5 w-3.5 text-primary shrink-0" />
                        )}
                      </button>
                    );
                  })}
                </div>
              </div>
            )}

            {/* Descripción compacta con scroll interno si es muy larga */}
            {product.description && (
              <div className="pt-2 border-t border-border/70">
                <h2 className="text-xs font-semibold uppercase tracking-wide text-content-muted mb-1">
                  Descripción
                </h2>
                <p className="text-xs sm:text-sm text-content-muted leading-relaxed whitespace-pre-line max-h-24 overflow-y-auto pr-1">
                  {product.description}
                </p>
              </div>
            )}
          </div>

          {/* ZONA DE COMPRA: Cantidad + Botón Agregar al carrito */}
          <div className="pt-3 border-t border-border space-y-3">
            <div className="flex items-center gap-3">
              {/* Selector de Cantidad */}
              {!isOutOfStock && (
                <div className="flex items-center border border-border rounded-lg bg-surface h-11 px-1">
                  <button
                    type="button"
                    onClick={handleDecreaseQty}
                    disabled={quantity <= 1}
                    className="p-2 text-content-muted hover:text-content-main disabled:opacity-40 cursor-pointer"
                    aria-label="Disminuir cantidad"
                  >
                    <Minus className="h-3.5 w-3.5" />
                  </button>
                  <span className="w-8 text-center text-sm font-semibold text-content-main">
                    {quantity}
                  </span>
                  <button
                    type="button"
                    onClick={handleIncreaseQty}
                    disabled={quantity >= selectedVariant.stock}
                    className="p-2 text-content-muted hover:text-content-main disabled:opacity-40 cursor-pointer"
                    aria-label="Aumentar cantidad"
                  >
                    <Plus className="h-3.5 w-3.5" />
                  </button>
                </div>
              )}

              {/* CTA Principal */}
              <Button
                disabled={isOutOfStock}
                className="flex-1 h-11 gap-2 text-sm font-semibold cursor-pointer shadow-sm"
              >
                <ShoppingCart className="h-4 w-4" />
                {isOutOfStock ? 'Sin stock disponible' : 'Agregar al carrito'}
              </Button>
            </div>

            {/* Micro-beneficios (confianza visual sin ocupar altura) */}
            <div className="grid grid-cols-2 gap-2 pt-1 text-[11px] text-content-muted">
              <div className="flex items-center gap-1.5">
                <ShieldCheck className="h-3.5 w-3.5 text-primary shrink-0" />
                <span>Compra protegida y garantía</span>
              </div>
              <div className="flex items-center gap-1.5">
                <Truck className="h-3.5 w-3.5 text-primary shrink-0" />
                <span>Envíos a todo el país</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}