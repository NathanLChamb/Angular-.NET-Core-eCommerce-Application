import { Component, computed, inject, input } from '@angular/core';
import { OptionService } from '../option-service';
import { FormArray, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { rxResource } from '@angular/core/rxjs-interop';
import { catchError, EMPTY, finalize, tap } from 'rxjs';
import { OptionResponse } from '../models';

type OptionFormGroup = FormGroup<{
  name: FormControl<string>
  optionValues: FormArray<OptionValueForm>
}>
type OptionValueForm = FormGroup<{
  id: FormControl<number | null>;
  value: FormControl<string>;
}>
@Component({
  selector: 'app-option-form',
  imports: [ReactiveFormsModule],
  templateUrl: './option-form.html',
  styleUrl: './option-form.css',
})
export class OptionForm {
  private optionService = inject(OptionService)
  private fb = inject(FormBuilder)
  private router = inject(Router)
  protected id = input<string | undefined>()
  protected isEditMode = computed(() => !!this.id())
  get name() {
    return this.form.controls.name
  }
  get optionValues() {
    return this.form.controls.optionValues
  }

  protected addOptionValue() {
    this.optionValues.push(this.fb.group({
      id: this.fb.control<number | null>(null),
      value: this.fb.nonNullable.control('', [Validators.required, Validators.maxLength(50)])
    }))
  }
  protected removeOptionValue(index: number) {
    this.optionValues.removeAt(index)
  }
  

  protected form: OptionFormGroup = this.fb.nonNullable.group({
    name: this.fb.nonNullable.control('',[Validators.required, Validators.maxLength(50)]),
    optionValues: this.fb.array([this.fb.group({
      id: this.fb.control<number | null>(null),
      value: this.fb.nonNullable.control('', [Validators.required, Validators.maxLength(50)])
    })], [Validators.minLength(1)])
  })

  protected option = rxResource({
    params: () => {
      const id = this.id()
      return id ? { id: Number(id) } : null
    },
    stream: ({params}) => {
      if (!params) {
        return EMPTY
      }
      this.form.disable()

      return this.optionService.getOptionById(params.id).pipe(
        tap(option => {
          this.seedForm(option)
        }),
        catchError(error => {
          if (error.status === 404) {
            this.router.navigate(['/option'])
          }
          return EMPTY
        }),
        finalize(() => {
          this.form.enable()
        })
      )
    }
  })

  protected seedForm(option: OptionResponse) {
    this.form.patchValue({ name: option.name })

    this.optionValues.clear()
    option.optionValues.forEach(optionValue => {
      this.optionValues.push(this.fb.group({
        id: this.fb.control<number | null>(optionValue.id),
        value: this.fb.nonNullable.control(optionValue.value, [Validators.required, Validators.maxLength(50)])
      }))
    })
  }

  onSubmit() {
    if (this.form.invalid) return

    const raw = this.form.getRawValue()
    const dto = {
      name: raw.name.trim(),
      optionValues: raw.optionValues
        .filter(optionValue => optionValue.value.trim() !== "")
        .map(optionValue => ({
          id: optionValue.id ?? undefined,
          value: optionValue.value.trim()
        }))
    }

    if (this.isEditMode()) {
      this.optionService.updateOption(Number(this.id()), dto).subscribe({
        next: () => this.router.navigate(['/option'])
      })
    } else {
      this.optionService.createOption({
        name: dto.name,
        optionValues: dto.optionValues.map(val => val.value)
      }).subscribe({
        next: () => this.router.navigate(['/option'])
      })
    }
  }
}
