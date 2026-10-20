import { Component, computed, effect, inject, input } from '@angular/core';
import { ProductService } from '../product-service';
import { CategoryService } from '../../category/category-service';
import { OptionService } from '../../option/option-service';
import { Router } from '@angular/router';
import { FormArray, FormBuilder, FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { rxResource, toSignal } from '@angular/core/rxjs-interop';
import { catchError, EMPTY, finalize, startWith, tap } from 'rxjs';
import { PictureResponse, ProductResponse } from '../models';

type ProductFormGroup = FormGroup<{
  name: FormControl<string>;
  description: FormControl<string>;
  categoryIds: FormControl<number[]>;
  optionIds: FormControl<number[]>;
  optionValueIds: FormControl<number[]>
  productVariants: FormArray<ProductVariantForm>;
}>
type ProductVariantForm = FormGroup<{
  id: FormControl<number | null>;
  sku: FormControl<string>;
  price: FormControl<number>;
  stockQuantity: FormControl<number>;
  optionValueIds: FormControl<number[]>;
  pictures: FormArray<PictureForm>;
}>
type PictureForm = FormGroup<{
  role: FormControl<string>;
  location: FormControl<string>;
}>;
type VariantCombination = {
  optionValueIds: number[]
}
@Component({
  selector: 'app-product-form',
  imports: [ReactiveFormsModule],
  templateUrl: './product-form.html',
  styleUrl: './product-form.css',
})
export class ProductForm {
  private productService = inject(ProductService)
  private categoryService = inject(CategoryService)
  private optionService = inject(OptionService)
  private router = inject(Router)
  private fb = inject(FormBuilder)
  protected id = input<string | undefined>()
  protected isEditMode = computed(() => !!this.id())
  private isSeeding = false
  get name() { return this.form.controls.name }
  get description() { return this.form.controls.description }
  get optionIds() { return this.form.controls.optionIds}
  get productVariants() { return this.form.controls.productVariants }

  protected productId = computed(() => {
    const id = this.id();
    return id ? Number(id) : null;
  });
  
  protected form: ProductFormGroup = this.fb.nonNullable.group({
    name: this.fb.nonNullable.control(''),
    description: this.fb.nonNullable.control(''),
    categoryIds: this.fb.nonNullable.control<number[]>([]),
    optionIds: this.fb.nonNullable.control<number[]>([]),
    optionValueIds: this.fb.nonNullable.control<number[]>([]),
    productVariants: this.fb.array<ProductVariantForm>([])
  })

  protected product = rxResource({
    params: () => {
      const id = this.id()
      return id ? { id: Number(id) } : null
    },
    stream: ({params}) => {
      if (!params) return EMPTY
      this.form.disable()

      return this.productService.getProductById(params.id).pipe(
        tap(product => {
          this.isSeeding = true
          try { 
            this.seedForm(product) 
          } finally { 
            this.isSeeding = false 
          }
        }),
        finalize(() => this.form.enable())
      )
    }
  })

  protected categories = rxResource({
    params: () => ({
      pageNumber: 1,
      pageSize: 100
    }),
    stream: ({params}) => this.categoryService.getAllCategories(params)
  })

  protected options = rxResource({
    params: () => ({
      pageNumber: 1,
      pageSize: 100
    }),
    stream: ({params}) => this.optionService.getAllOptions(params)
  })

  protected selectedOptionValueIds = toSignal(
    this.form.controls.optionValueIds.valueChanges.pipe(
      startWith(this.form.controls.optionValueIds.value)
    ),
    { initialValue: this.form.controls.optionValueIds.value }
  );

  toggleOptionValue(valueId: number) {
    const current = this.form.controls.optionValueIds.value;

    const next = current.includes(valueId)
      ? current.filter(id => id !== valueId)
      : [...current, valueId];

    this.form.controls.optionValueIds.setValue(next);
  }

  protected variants = computed(() => {
  const selectedValueIds = this.selectedOptionValueIds();
  const selectedOptions = this.selectedOptions();

  const valueGroups = selectedOptions.map(option =>
    (option.optionValues ?? []).filter(value =>
      selectedValueIds.includes(value.id)
    )
  );

  // Don't generate variants until every selected option
  // has at least one selected value.
  if (valueGroups.some(group => group.length === 0)) {
    return [];
  }

  const combinations = this.cartesianProduct(valueGroups);

  return combinations.map(combination => ({
    optionValueIds: combination.map(value => value.id)
  }));
});

  protected seedForm(product: ProductResponse) {
    const existingValueIds = [
      ...new Set(
        product.productVariants.flatMap(variant =>
          variant.optionValues.map(value => value.id)
        )
      )
    ];

    this.form.patchValue({
      name: product.name,
      description: product.description,
      categoryIds: product.categories.map(c => c.id),
      optionIds: product.options.map(o => o.id),
      optionValueIds: existingValueIds
    });

    this.productVariants.clear();

    product.productVariants.forEach(variant => {
      this.productVariants.push(
        this.fb.group({
          id: this.fb.control<number | null>(variant.id),
          sku: this.fb.nonNullable.control(variant.sku),
          price: this.fb.nonNullable.control(variant.price ?? 0),
          stockQuantity: this.fb.nonNullable.control(variant.stockQuantity),
          optionValueIds: this.fb.nonNullable.control(variant.optionValues.map(ov => ov.id)),
          pictures: this.fb.array<PictureForm>(
            variant.pictures.map(picture =>
              this.fb.group({
                role: this.fb.nonNullable.control(picture.role),
                location: this.fb.nonNullable.control(picture.location)
              })
            )
          )
        })
      );
    });
  }

  protected selectedOptionIds = toSignal(
    this.form.controls.optionIds.valueChanges.pipe(
      startWith(this.optionIds.value)
    ), {initialValue: this.optionIds.value}
  )

  protected selectedOptions = computed(() => {
    const optionIds = this.selectedOptionIds()
    const allOptions = this.options.value()?.items ?? []

    return allOptions.filter(existing => optionIds.includes(existing.id))
  })

  cartesianProduct<T>(arrays: T[][]): T[][] {
    return arrays.reduce<T[][]>(
      (acc, curr) => acc.flatMap(a =>
        curr.map(c => [...a, c])
      ), [[]]
    )
  }

  private syncVariantForm(variants: VariantCombination[]) {
    const existingMap = new Map(
      this.productVariants.controls.map(control => [
        this.variantKey({
          optionValueIds: control.controls.optionValueIds.value
        }),
        control
      ])
    )

    const nextControls: ProductVariantForm[] = []

    for (const variant of variants) {
      const key = this.variantKey(variant)

      const existing = existingMap.get(key)

      if (existing) {
        nextControls.push(existing)
      } else {
        nextControls.push(
          this.fb.group({
            id: this.fb.control<number | null>(null),
            sku: this.fb.nonNullable.control(''),
            price: this.fb.nonNullable.control(0),
            stockQuantity: this.fb.nonNullable.control(0),
            optionValueIds: this.fb.nonNullable.control(variant.optionValueIds),
            pictures: this.fb.array<PictureForm>([])
          })
        );
      }
    }

    this.form.setControl(
      'productVariants',
      this.fb.array(nextControls)
    )
  }

  constructor() {
    effect(() => {
      if (this.isSeeding) return
      const options = this.options.value()

      if (!options) return

      const variants = this.variants()

      const currentKeys = this.productVariants.controls.map(control =>
        this.variantKey({
          optionValueIds:
            control.controls.optionValueIds.value
        })
      )

      const newKeys = variants.map(v =>
        this.variantKey(v)
      )

      const changed =
        currentKeys.length !== newKeys.length ||
        currentKeys.some((k, i) => k !== newKeys[i])

      if (changed) {
        this.syncVariantForm(variants)
      }
    })
  }

  private variantKey(v: VariantCombination) {
    return [...v.optionValueIds].sort().join('-')
  }

  toggleCategory(id: number) {
    const current = this.form.controls.categoryIds.value

    const next = current.includes(id)
      ? current.filter(x => x !== id)
      : [...current, id]

    this.form.controls.categoryIds.setValue(next)
  }

  toggleOption(id: number) {
    const current = this.form.controls.optionIds.value;

    const next = current.includes(id)
      ? current.filter(x => x !== id)
      : [...current, id];

    this.form.controls.optionIds.setValue(next);

    const selectedValueIds = this.form.controls.optionValueIds.value;
    const allOptions = this.options.value()?.items ?? [];

    const allowedValueIds = new Set(
      allOptions
        .filter(option => next.includes(option.id))
        .flatMap(option =>
          (option.optionValues ?? []).map(value => value.id)
        )
    );

    this.form.controls.optionValueIds.setValue(
      selectedValueIds.filter(valueId => allowedValueIds.has(valueId))
    );
  }

  optionValueMap = computed(() => {
    const map = new Map<number, string>()

    for (const opt of this.selectedOptions()) {
      for (const val of opt.optionValues) {
        map.set(val.id, `${opt.name} - ${val.value}`)
      }
    }

    return map
  })

  protected addPicture(variant: ProductVariantForm) {
    variant.controls.pictures.push(
      this.createPictureForm()
    );
  }

  protected removePicture(
    variant: ProductVariantForm,
    index: number
  ) {
    variant.controls.pictures.removeAt(index);
  }

  private createPictureForm(picture?: PictureResponse): PictureForm {
    return this.fb.group({
      role: this.fb.nonNullable.control(
        picture?.role ?? ''
      ),
      location: this.fb.nonNullable.control(
        picture?.location ?? ''
      )
    });
  }

  protected onSubmit() {
    if (this.form.invalid) return

    const raw = this.form.getRawValue()
    const dto = {
      name: raw.name.trim(),
      description: raw.description.trim(),
      categoryIds: raw.categoryIds,
      optionIds: raw.optionIds,
      productVariants: raw.productVariants
        .filter(v => v.sku && v.sku.trim() !== '')
        .map(v => ({
          id: v.id ?? undefined,
          sku: v.sku.trim(),
          price: Number(v.price),
          stockQuantity: Number(v.stockQuantity),
          optionValueIds: v.optionValueIds,
          pictures: v.pictures
            .filter(p => p.role.trim() !== '' && p.location.trim() !== '')
            .map(p => ({
              role: p.role.trim(),
              location: p.location.trim()
            }))
        }))
    }

    if (this.isEditMode()) {
      this.productService.updateProduct(Number(this.id()), dto)
        .subscribe({
          next: () => this.router.navigate(['/product'])
        })
    } else {
      this.productService.createProduct(dto)
        .subscribe({
          next: () => this.router.navigate(['/product'])
        })
    }
  }
}
