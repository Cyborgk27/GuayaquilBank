import { CommonModule } from '@angular/common';
import { Component, inject, output } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CreateProductBatchRequestDto } from '../../../../core/api/v1';
import { GenericInput } from '../../../../shared/components/generic-input/generic-input';

@Component({
  selector: 'app-product-batch-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, GenericInput],
  templateUrl: './product-batch-form.html',
  styleUrls: ['./product-batch-form.css']
})
export class ProductBatchForm {
  private fb = inject(FormBuilder);

  public onSubmitBatch = output<CreateProductBatchRequestDto>();
  public onCancel = output<void>();

  public batchForm: FormGroup = this.fb.group({
    batchNumber: ['', [Validators.required]],
    quantity: [1, [Validators.required, Validators.min(1)]],
    unitCost: [0.01, [Validators.required, Validators.min(0.01)]],
    manufacturedAt: ['', [Validators.required]],
    expirationDate: ['']
  });

  public reset(): void {
    this.batchForm.reset({
      batchNumber: '',
      quantity: 1,
      unitCost: 0.01,
      manufacturedAt: '',
      expirationDate: ''
    });
  }

  public submit(): void {
    if (this.batchForm.invalid) {
      this.batchForm.markAllAsTouched();
      return;
    }

    const value = this.batchForm.value;
    this.onSubmitBatch.emit({
      batchNumber: value.batchNumber,
      quantity: value.quantity,
      unitCost: value.unitCost,
      manufacturedAt: value.manufacturedAt,
      expirationDate: value.expirationDate || undefined
    });
  }

  public isFieldInvalid(field: string): boolean {
    const control = this.batchForm.get(field);
    return !!(control && control.touched && control.invalid);
  }

  public getErrorMessage(field: string): string {
    const control = this.batchForm.get(field);
    if (!control || !control.errors || !control.touched) return '';

    if (control.errors['required']) return 'Este campo es obligatorio.';
    if (control.errors['min']) return `Valor mínimo ${control.errors['min'].min}.`;
    return 'Campo inválido.';
  }
}
