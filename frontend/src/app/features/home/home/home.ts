import { Component, computed, inject } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { ProductService } from '../../product/product-service';
import { CartService } from '../../cart/cart-service';
import { RouterLink } from '@angular/router';
import { ProductFilterService } from '../../product/product-filter-service';
import { PictureResponse, ProductResponse } from '../../product/models';

@Component({
  selector: 'app-home',
  imports: [RouterLink],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {
  private cartService = inject(CartService);
  private productService =inject(ProductService);
  private filterService = inject(ProductFilterService);

  protected products = rxResource({
    params: () => this.filterService.filter(),
    stream: ({ params }) => this.productService.getAllProducts(params)
  });

  protected getProductImage(product: ProductResponse): PictureResponse | null {
    return product.productVariants[0].pictures[0] ?? null;
  }

  addToCart(productVariantId: number) {
    this.cartService.AddItem({
        productVariantId,
        quantity: 1
      })
      .subscribe();
  }

  protected pageNumber = computed(
    () => this.filterService.filter().pageNumber
  )

  protected totalPages = computed(() => {
    const data = this.products.value()
    if (!data) return 0

    return Math.ceil(data.totalCount / this.filterService.filter().pageSize)
  })

  protected previousPage() {
    this.filterService.filter.update(f => ({
      ...f,
      pageNumber: Math.max(1, f.pageNumber - 1)
    })) 
  }

  protected nextPage() {
    this.filterService.filter.update(f => ({
      ...f,
      pageNumber: f.pageNumber + 1
    }))
  }
}

