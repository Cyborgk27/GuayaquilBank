import { CommonModule } from '@angular/common';
import { Component, inject, output, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CreateProductRequestDto } from '../../../../core/api/v1';
import { GenericInput } from '../../../../shared/components/generic-input/generic-input';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, GenericInput],
  templateUrl: './product-form.html',
  styleUrls: ['./product-form.css']
})
export class ProductForm {
  private fb = inject(FormBuilder);

  public onSubmitForm = output<CreateProductRequestDto>();
  public onCancel = output<void>();

  public isEditMode = signal<boolean>(false);

  public productForm: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3)]],
    sku: ['', [Validators.required, Validators.minLength(3)]],
    description: [''],
    useInitialBatch: [false],
    initialBatch: this.fb.group({
      batchNumber: ['', [Validators.required]],
      quantity: [1, [Validators.required, Validators.min(1)]],
      unitCost: [0.01, [Validators.required, Validators.min(0.01)]],
      manufacturedAt: ['', [Validators.required]],
      expirationDate: ['']
    })
  });

  constructor() {
    const useInitialBatchControl = this.productForm.get('useInitialBatch');
    const initialBatchGroup = this.productForm.get('initialBatch');

    if (useInitialBatchControl && initialBatchGroup) {
      useInitialBatchControl.valueChanges.subscribe((useBatch: boolean) => {
        if (useBatch) {
          initialBatchGroup.enable();
        } else {
          initialBatchGroup.disable();
        }
      });
    }
  }

  public resetForCreate(): void {
    this.isEditMode.set(false);
    this.productForm.reset({
      name: '',
      sku: '',
      description: '',
      useInitialBatch: false,
      initialBatch: {
        batchNumber: '',
        quantity: 1,
        unitCost: 0.01,
        manufacturedAt: '',
        expirationDate: ''
      }
    });

    this.productForm.get('initialBatch')?.disable();
  }

  public submit(): void {
    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      return;
    }

    const formValue = this.productForm.value;
    const payload: CreateProductRequestDto = {
      name: formValue.name,
      sku: formValue.sku,
      description: formValue.description || undefined
    };

    if (formValue.useInitialBatch) {
      payload.initialBatch = {
        batchNumber: formValue.initialBatch.batchNumber,
        quantity: formValue.initialBatch.quantity,
        unitCost: formValue.initialBatch.unitCost,
        manufacturedAt: formValue.initialBatch.manufacturedAt,
        expirationDate: formValue.initialBatch.expirationDate || undefined
      };
    }

    this.onSubmitForm.emit(payload);
  }

  public isFieldInvalid(field: string): boolean {
    const control = this.productForm.get(field);
    return !!(control && control.touched && control.invalid);
  }

  public getErrorMessage(field: string): string {
    const control = this.productForm.get(field);
    if (!control || !control.errors || !control.touched) return '';

    if (control.errors['required']) return 'Este campo es obligatorio.';
    if (control.errors['minlength']) return `Mínimo ${control.errors['minlength'].requiredLength} caracteres.`;
    if (control.errors['min']) return `Valor mínimo ${control.errors['min'].min}.`;

    return 'Campo inválido.';
  }
}
