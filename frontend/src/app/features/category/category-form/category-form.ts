import { Component, computed, inject, input } from '@angular/core';
import { CategoryService } from '../category-service';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { rxResource } from '@angular/core/rxjs-interop';
import { catchError, EMPTY, tap } from 'rxjs';

type CategoryFormGroup = FormGroup<{
  name: FormControl<string>
}>
@Component({
  selector: 'app-category-form',
  imports: [ReactiveFormsModule],
  templateUrl: './category-form.html',
  styleUrl: './category-form.css',
})
export class CategoryForm {
  private categoryService = inject(CategoryService)
  private fb = inject(FormBuilder)
  private router = inject(Router)
  protected id = input<string | undefined>()
  protected isEditMode = computed(() => !!this.id())
  get name() {
    return this.form.controls.name
  }

  protected form: CategoryFormGroup = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(50)]],
  })

  protected category = rxResource({
    params: () => {
      const id = this.id()
      return id ? { id: Number(id) } : null
    },
    stream: ({params}) => {
      if (!params) return EMPTY
      return this.categoryService.getCategoryById(params.id).pipe(
        tap(category => {
          this.form.patchValue({
            name: category.name
          })
        }),
        catchError(error => {
          if (error.status === 404) {
            this.router.navigate(['/category'])
          }
          return EMPTY
        })
      )
    }
  })

  onSubmit() {
    if (this.form.invalid) return

    const raw = this.form.getRawValue()
    const dto = {
      name: raw.name.trim()
    }

    if (this.isEditMode()) {
      this.categoryService.updateCategory(Number(this.id()), dto).subscribe({
        next: () => this.router.navigate(['/category'])
      })
    } else {
      this.categoryService.createCategory(dto).subscribe({
        next: () => this.router.navigate(['/category'])
      })
    }
  }
}
