import { Component, computed, inject, signal } from '@angular/core';
import { OptionService } from '../option-service';
import { rxResource } from '@angular/core/rxjs-interop';
import { computeMsgId } from '@angular/compiler';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-option-list',
  imports: [RouterLink],
  templateUrl: './option-list.html',
  styleUrl: './option-list.css',
})
export class OptionList {
  private optionService = inject(OptionService)
  protected pageNumber = signal(1)
  protected pageSize = signal(5)

  protected options = rxResource({
    params: () => ({
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize()
    }),
    stream: ({params}) => this.optionService.getAllOptions(params)
  })

  protected totalPages = computed(() => {
    const data = this.options.value()
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

  protected handleDelete(id: number) {
    this.optionService.deleteOption(id).subscribe({
      next: () => this.options.reload()
    })
  }
}
