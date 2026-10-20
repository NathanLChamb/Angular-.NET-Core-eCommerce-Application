import { Component, computed, effect, inject, input, signal } from '@angular/core';
import { ProductService } from '../product-service';
import { rxResource } from '@angular/core/rxjs-interop';
import { ProductSearchFilter, ProductSort } from '../models';
import { EMPTY } from 'rxjs';
import { CartService } from '../../cart/cart-service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-product-details',
  imports: [],
  templateUrl: './product-details.html',
  styleUrl: './product-details.css',
})
export class ProductDetails {
  private productService = inject(ProductService);
  private cartService = inject(CartService);
  private snackBar = inject(MatSnackBar);

  protected id = input<string | undefined>();

  protected ProductSort = ProductSort;

  protected selectedOptions = signal<Map<number, number>>(new Map());

  protected filter = signal<ProductSearchFilter>({
    search: '',
    categoryIds: [],
    optionIds: [],
    sort: ProductSort.Default,
    pageNumber: 1,
    pageSize: 5
  });

  protected product = rxResource({
    params: () => {
      const id = this.id();

      return id
        ? { id: Number(id) }
        : null;
    },

    stream: ({ params }) => {

      if (!params) {
        return EMPTY;
      }

      return this.productService
        .getProductById(params.id);
    }
  });

  protected isSelected(
    optionId: number,
    valueId: number
  ): boolean {
    return this.selectedOptions().get(optionId) === valueId;
  }

  protected selectOption(
    optionId: number,
    valueId: number
  ) {
    this.selectedOptions.update(current => {
      const updated = new Map(current);
      updated.set(optionId, valueId);
      return updated;
    });
  }

  protected selectedVariant = computed(() => {
    const product = this.product.value();

    if (!product) {
      return null;
    }

    const selected = this.selectedOptions();

    return product.productVariants.find(variant => {
      return product.options.every(option => {
        const selectedValueId = selected.get(option.id);

        if (selectedValueId === undefined) {
          return false;
        }

        return variant.optionValues.some(
          optionValue => optionValue.id === selectedValueId
        );
      });
    }) ?? null;
  });

  protected selectedVariantPictures = computed(() => {
    return this.selectedVariant()?.pictures ?? [];
  });
  
  constructor() {
    effect(() => {
        const product = this.product.value();

        if (!product || this.selectedOptions().size > 0) {
          return;
        }

        const firstVariant = product.productVariants[0];

        if (!firstVariant) {
          return;
        }

        const selections = new Map<number, number>();

        for (const option of product.options) {
          const value = option.optionValues.find(optionValue =>
            firstVariant.optionValues.some(
              variantValue => variantValue.id === optionValue.id
            )
          );

          if (value) {
            selections.set(option.id, value.id);
          }
        }

        this.selectedOptions.set(selections);
      });
    }

  protected addToCart(
    productVariantId: number
  ) {

    this.cartService.AddItem({
      productVariantId,
      quantity: 1,
    }).subscribe(() => {

      this.snackBar.open(
        'Added to cart',
        'Close',
        {
          duration: 3000,
          horizontalPosition: 'right',
          verticalPosition: 'bottom',
        }
      );

    });
  }
}
