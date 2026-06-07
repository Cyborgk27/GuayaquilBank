import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, Input, OnInit, output, Output, signal } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { GenericInput } from '../../../../shared/components/generic-input/generic-input';
import { UserResponseDto } from '../../../../core/api/v1';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, GenericInput],
  templateUrl: './user-form.html',
  styleUrl: './user-form.css'
})
export class UserForm {
  private fb = inject(FormBuilder);

  public onSubmitForm = output<any>();
  public onCancel = output<void>();

  public isEditMode = signal<boolean>(false);
  private currentUserId: string | null = null;

  public form = this.fb.group({
    username: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    firstName: ['', [Validators.required]],
    lastName: ['', [Validators.required]],
    password: ['']
  });

  public setFormValue(user: UserResponseDto): void {
    this.isEditMode.set(true);
    this.currentUserId = user.id || null;

    this.form.patchValue({
      username: user.username,
      email: user.email,
      firstName: user.firstName,
      lastName: user.lastName
    });
  }

  submit(): void {
    if(this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const payload = {
      ...this.form.value,
      ...(this.isEditMode() && { id: this.currentUserId })
    };

    this.onSubmitForm.emit(payload);
  }

  public isFieldInvalid(field: string): boolean {
    const control = this.form.get(field);
    return !!(control && control.touched && control.invalid);
  }

  public getErrorMessage(field: string): string {
    const control = this.form.get(field);
    if (control && control.errors) {
      if (control.errors['required']) {
        return 'Este campo es obligatorio';
      }
      if (control.errors['minlength']) {
        const requiredLength = control.errors['minlength'].requiredLength;
        return `Mínimo ${requiredLength} caracteres`;
      }
      if (control.errors['email']) {
        return 'Correo electrónico no válido';
      }
    }
    return '';
  }
}
