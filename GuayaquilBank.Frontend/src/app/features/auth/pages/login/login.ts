import { Component, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthFacade } from '../../../../core/service/auth.facade';
import { LoginRequestDto } from '../../../../core/api/v1';
import { GenericInput } from '../../../../shared/components/generic-input/generic-input';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, GenericInput],
  templateUrl: './login.html',
})
export class Login {
  private fb = inject(FormBuilder);
  private authService = inject(AuthFacade);
  private router = inject(Router);

  public isLoading = signal<boolean>(false);

  public loginForm: FormGroup = this.fb.nonNullable.group({
    domain: ['', [Validators.required, Validators.minLength(3)]],
    username: ['', [Validators.required, Validators.minLength(3)]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  public onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    const credentials = this.loginForm.getRawValue() as LoginRequestDto;

    this.authService.login(credentials).subscribe({
      next: () => {
        this.router.navigate(['']);
      },
      error: () => {
        this.isLoading.set(false);
      },
      complete: () => {
        this.isLoading.set(false);
      }
    });
  }

  public isFieldInvalid(field: string): boolean {
    const control = this.loginForm.get(field);
    return !!(control && control.errors && (control.dirty || control.touched));
  }
}
