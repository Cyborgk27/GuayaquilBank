import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import {
  MyCompanyResponseDto,
  MyUserResponseDto,
  ProfileApiService,
  UpdateMyCompanyRequestDto,
  UpdateMyProfileRequestDto
} from '../../core/api/v1';
import { GenericInput } from '../../shared/components/generic-input/generic-input';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, GenericInput],
  templateUrl: './profile.html',
})
export class Profile implements OnInit {
  public title = signal<string>('Perfil de Usuario');
  public myUserResponseDto = signal<MyUserResponseDto | null>(null);
  public myCompanyResponseDto = signal<MyCompanyResponseDto | null>(null);
  public isLoading = signal<boolean>(false);
  public isCompanyLoading = signal<boolean>(false);
  public successMessage = signal<string>('');
  public errorMessage = signal<string>('');
  public companySuccessMessage = signal<string>('');
  public companyErrorMessage = signal<string>('');

  private fb = inject(FormBuilder);
  private profileService = inject(ProfileApiService);

  public profileForm = this.fb.group({
    firstName: ['', [Validators.required]],
    lastName: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    profilePictureUrl: ['']
  });

  public companyForm = this.fb.group({
    name: ['', [Validators.required]],
    taxId: ['', [Validators.required]],
    domain: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    phoneNumber: ['', [Validators.required]],
    address: ['', [Validators.required]],
    city: ['', [Validators.required]],
    region: ['', [Validators.required]],
    postalCode: ['', [Validators.required]],
    country: ['', [Validators.required]],
    logoUrl: [''],
    currencySymbol: ['', [Validators.required]],
    iva: [0, [Validators.required, Validators.min(0)]]
  });

  ngOnInit(): void {
    this.loadProfile();
    this.loadCompany();
  }

  private loadProfile(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');
    this.profileService.apiProfileGet().subscribe({
      next: result => {
        if (result?.success && result.data) {
          this.myUserResponseDto.set(result.data);
          this.profileForm.patchValue({
            firstName: result.data.firstName ?? '',
            lastName: result.data.lastName ?? '',
            email: result.data.email ?? '',
            profilePictureUrl: result.data.profilePictureUrl ?? ''
          });
        } else {
          this.errorMessage.set(result?.message ?? 'No se pudo cargar los datos de perfil.');
        }
      },
      error: () => {
        this.errorMessage.set('Error cargando perfil. Intenta de nuevo más tarde.');
      },
      complete: () => this.isLoading.set(false)
    });
  }

  private loadCompany(): void {
    this.isCompanyLoading.set(true);
    this.companyErrorMessage.set('');
    this.profileService.apiProfileCompanyGet().subscribe({
      next: result => {
        if (result?.success && result.data) {
          this.myCompanyResponseDto.set(result.data);
          this.companyForm.patchValue({
            name: result.data.name ?? '',
            taxId: result.data.taxId ?? '',
            domain: result.data.domain ?? '',
            email: result.data.email ?? '',
            phoneNumber: result.data.phoneNumber ?? '',
            address: result.data.address ?? '',
            city: result.data.city ?? '',
            region: result.data.region ?? '',
            postalCode: result.data.postalCode ?? '',
            country: result.data.country ?? '',
            logoUrl: result.data.logoUrl ?? '',
            currencySymbol: result.data.currencySymbol ?? '',
            iva: result.data.iva ?? 0
          });
        } else {
          this.companyErrorMessage.set(result?.message ?? 'No se pudo cargar los datos de la compañía.');
        }
      },
      error: () => {
        this.companyErrorMessage.set('Error cargando la información de la compañía. Intenta de nuevo más tarde.');
      },
      complete: () => this.isCompanyLoading.set(false)
    });
  }

  submit(): void {
    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();
      return;
    }

    this.successMessage.set('');
    this.errorMessage.set('');
    this.isLoading.set(true);

    const payload: UpdateMyProfileRequestDto = {
      email: this.profileForm.get('email')?.value?.trim() ?? '',
      firstName: this.profileForm.get('firstName')?.value?.trim() ?? '',
      lastName: this.profileForm.get('lastName')?.value?.trim() ?? '',
      profilePictureUrl: this.profileForm.get('profilePictureUrl')?.value?.trim() || undefined
    };

    this.profileService.apiProfilePut(payload).subscribe({
      next: result => {
        if (result?.success && result.data) {
          this.myUserResponseDto.set(result.data);
          this.profileForm.patchValue({
            firstName: result.data.firstName ?? '',
            lastName: result.data.lastName ?? '',
            email: result.data.email ?? '',
            profilePictureUrl: result.data.profilePictureUrl ?? ''
          });
          this.successMessage.set('Perfil actualizado correctamente.');
        } else {
          this.errorMessage.set(result?.message ?? 'No se pudo actualizar el perfil.');
        }
      },
      error: () => {
        this.errorMessage.set('Error guardando los cambios. Intenta de nuevo.');
      },
      complete: () => this.isLoading.set(false)
    });
  }

  submitCompany(): void {
    if (this.companyForm.invalid) {
      this.companyForm.markAllAsTouched();
      return;
    }

    this.companySuccessMessage.set('');
    this.companyErrorMessage.set('');
    this.isCompanyLoading.set(true);

    const payload: UpdateMyCompanyRequestDto = {
      name: this.companyForm.get('name')?.value?.trim() ?? '',
      taxId: this.companyForm.get('taxId')?.value?.trim() ?? '',
      domain: this.companyForm.get('domain')?.value?.trim() ?? '',
      email: this.companyForm.get('email')?.value?.trim() ?? '',
      phoneNumber: this.companyForm.get('phoneNumber')?.value?.trim() ?? '',
      address: this.companyForm.get('address')?.value?.trim() ?? '',
      city: this.companyForm.get('city')?.value?.trim() ?? '',
      region: this.companyForm.get('region')?.value?.trim() ?? '',
      postalCode: this.companyForm.get('postalCode')?.value?.trim() ?? '',
      country: this.companyForm.get('country')?.value?.trim() ?? '',
      logoUrl: this.companyForm.get('logoUrl')?.value?.trim() || undefined,
      currencySymbol: this.companyForm.get('currencySymbol')?.value?.trim() ?? '',
      iva: Number(this.companyForm.get('iva')?.value) || 0
    };

    this.profileService.apiProfileCompanyPut(payload).subscribe({
      next: result => {
        if (result?.success && result.data) {
          this.myCompanyResponseDto.set(result.data);
          this.companyForm.patchValue({
            name: result.data.name ?? '',
            taxId: result.data.taxId ?? '',
            domain: result.data.domain ?? '',
            email: result.data.email ?? '',
            phoneNumber: result.data.phoneNumber ?? '',
            address: result.data.address ?? '',
            city: result.data.city ?? '',
            region: result.data.region ?? '',
            postalCode: result.data.postalCode ?? '',
            country: result.data.country ?? '',
            logoUrl: result.data.logoUrl ?? '',
            currencySymbol: result.data.currencySymbol ?? '',
            iva: result.data.iva ?? 0
          });
          this.companySuccessMessage.set('Información de la compañía actualizada correctamente.');
        } else {
          this.companyErrorMessage.set(result?.message ?? 'No se pudo actualizar la compañía.');
        }
      },
      error: () => {
        this.companyErrorMessage.set('Error guardando los cambios de la compañía. Intenta de nuevo.');
      },
      complete: () => this.isCompanyLoading.set(false)
    });
  }

  public isFieldInvalid(field: string): boolean {
    const control = this.profileForm.get(field);
    return !!(control && control.touched && control.invalid);
  }

  public isCompanyFieldInvalid(field: string): boolean {
    const control = this.companyForm.get(field);
    return !!(control && control.touched && control.invalid);
  }

  public getErrorMessage(field: string): string {
    const control = this.profileForm.get(field);
    if (control && control.errors) {
      if (control.errors['required']) {
        return 'Este campo es obligatorio';
      }
      if (control.errors['email']) {
        return 'Correo electrónico no válido';
      }
    }
    return '';
  }

  public getCompanyErrorMessage(field: string): string {
    const control = this.companyForm.get(field);
    if (control && control.errors) {
      if (control.errors['required']) {
        return 'Este campo es obligatorio';
      }
      if (control.errors['email']) {
        return 'Correo electrónico no válido';
      }
      if (control.errors['min']) {
        return 'Debe ser un número mayor o igual a 0';
      }
    }
    return '';
  }
}
