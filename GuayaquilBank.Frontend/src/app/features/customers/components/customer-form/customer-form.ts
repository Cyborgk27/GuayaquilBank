import { Component, inject, output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CustomerResponseDto } from '../../../../core/api/v1';
import { GenericInput } from '../../../../shared/components/generic-input/generic-input';

@Component({
  selector: 'app-customer-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, GenericInput],
  templateUrl: './customer-form.html',
  styleUrl: './customer-form.css'
})
export class CustomerForm {
  private fb = inject(FormBuilder);

  public onSubmitForm = output<any>();
  public onCancel = output<void>();

  public isEditMode = signal<boolean>(false);
  private currentCustomerId: string | null = null;

  public customerForm: FormGroup = this.fb.group({
    identification: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(13)]],
    fullName: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    phoneNumber: ['', [Validators.required, Validators.pattern('^[0-9]{9,10}$')]],
    address: ['', [Validators.required]],
    isActive: [true]
  });

  public resetForCreate(): void {
    this.isEditMode.set(false);
    this.currentCustomerId = null;
    this.customerForm.reset({ isActive: true });
  }

  public setFormValue(customer: CustomerResponseDto): void {
    this.isEditMode.set(true);
    this.currentCustomerId = customer.id || null;

    this.customerForm.patchValue({
      identification: customer.identification,
      fullName: customer.fullName,
      email: customer.email,
      phoneNumber: customer.phoneNumber,
      address: customer.address,
      isActive: customer.isActive
    });
  }

  public submit(): void {
    if (this.customerForm.invalid) {
      this.customerForm.markAllAsTouched();
      return;
    }

    const payload = {
      ...this.customerForm.value,
      ...(this.isEditMode() && { id: this.currentCustomerId })
    };

    this.onSubmitForm.emit(payload);
  }

  public isFieldInvalid(field: string): boolean {
    const control = this.customerForm.get(field);
    return !!(control && control.touched && control.invalid);
  }

  public getErrorMessage(field: string): string {
    const control = this.customerForm.get(field);
    if (!control || !control.errors || !control.touched) return '';

    if (control.errors['required']) return 'Este campo es obligatorio.';
    if (control.errors['email']) return 'Ingresa un formato de correo corporativo válido.';
    if (control.errors['minlength']) return `Mínimo ${control.errors['minlength'].requiredLength} caracteres.`;
    if (control.errors['maxlength']) return `Máximo ${control.errors['maxlength'].requiredLength} caracteres.`;
    if (control.errors['pattern']) return 'El número celular o formato ingresado es inválido.';

    return 'Campo inválido.';
  }
}
