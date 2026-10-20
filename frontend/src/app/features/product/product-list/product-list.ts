import { Component, computed, inject, signal } from '@angular/core';
import { ProductService } from '../product-service';
import { rxResource } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { ProductSearchFilter, ProductSort } from '../models';


@Component({
  selector: 'app-product-list',
  imports: [RouterLink],
  templateUrl: './product-list.html',
  styleUrl: './product-list.css',
})
export class ProductList {
  private productService = inject(ProductService)
  protected ProductSort = ProductSort;
  protected filter = signal<ProductSearchFilter>({
  search: '',
  categoryIds: [],
  optionIds: [],
  sort: ProductSort.Default,
  pageNumber: 1,
  pageSize: 5
})
  expandedProductIds = new Set<number>()

  protected products = rxResource({
    params: () => this.filter(),
    stream: ({params}) => this.productService.getAllProducts(params)
  })

  protected totalPages = computed(() => {
    const data = this.products.value()
    if (!data) return 0

    return Math.ceil(data.totalCount / this.filter().pageSize)
  })

  protected toggle(productId: number) {
    if (this.expandedProductIds.has(productId)) {
      this.expandedProductIds.delete(productId)
    } else {
      this.expandedProductIds.add(productId)
    }
  }

  protected isExpanded(productId: number) {
    return this.expandedProductIds.has(productId)
  }

  protected updateSearch(search: string) {
    this.filter.update(f => ({
      ...f,
      search,
      pageNumber: 1
    }));
  }

  protected updateSort(sort: ProductSort) {
    this.filter.update(f => ({
      ...f,
      sort,
      pageNumber: 1
    }));
  }

  protected previousPage() {
    this.filter.update(f => ({
      ...f,
      pageNumber: Math.max(1, f.pageNumber - 1)
    })) 
  }

  protected nextPage() {
    this.filter.update(f => ({
      ...f,
      pageNumber: f.pageNumber + 1
    }))
  }

  handleDelete(id: number) {
    this.productService.deleteProduct(id).subscribe({
      next: () => this.products.reload()
    })
  }
}
