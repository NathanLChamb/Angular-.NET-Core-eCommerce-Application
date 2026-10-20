import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CategoryService } from '../category-service';
import { rxResource } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-category-list',
  imports: [RouterLink],
  templateUrl: './category-list.html',
  styleUrl: './category-list.css',
})
export class CategoryList {
  private categoryService = inject(CategoryService)
  protected pageNumber = signal(1)
  protected pageSize = signal(5)
  
  protected categories = rxResource({
    params: () => ({
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize()
    }),
    stream: ({params}) => this.categoryService.getAllCategories(params)
  })

  protected totalPages = computed(() => {
    const data = this.categories.value()
    if (!data) return 0

    return Math.ceil(data.totalCount / this.pageSize())
  })

  protected previousPage() {
    if (this.pageNumber() > 1) {
      this.pageNumber.update(p => p - 1)
    }
  }

  protected nextPage() {
    if (this.pageNumber() < this.totalPages()) {
      this.pageNumber.update(p => p + 1)
    }
  }

  handleDelete(id: number) {
    this.categoryService.deleteCategory(id).subscribe({
      next: () => this.categories.reload()
    })
  }
}
