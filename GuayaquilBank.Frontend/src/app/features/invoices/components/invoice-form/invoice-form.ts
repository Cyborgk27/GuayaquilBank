import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, output, signal } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CreateInvoiceRequestDto, CustomersApiService, MyCompanyResponseDto, ProductsApiService, ProfileApiService } from '../../../../core/api/v1';
import { GenericInput } from '../../../../shared/components/generic-input/generic-input';
import { debounceTime } from 'rxjs/internal/operators/debounceTime';
import { GenericAutocomplete } from '../../../../shared/components/generic-autocomplete/generic-autocomplete';
import { AutocompleteOption } from '../../../../shared/interfaces/autocomplete-option.interface';

@Component({
  selector: 'app-invoice-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, GenericInput, GenericAutocomplete, FormsModule],
  templateUrl: './invoice-form.html',
  styleUrl: './invoice-form.css'
})
export class InvoiceForm implements OnInit {
  private fb = inject(FormBuilder);

  public onSubmitForm = output<CreateInvoiceRequestDto>();
  public onCancel = output<void>();

  public customerService = inject(CustomersApiService);
  public productService = inject(ProductsApiService);
  public profileService = inject(ProfileApiService);

  // Totales de la factura
  public subtotal = signal<number>(0);
  public tax = signal<number>(0);
  public total = signal<number>(0);
  public taxRate = signal<number>(0.15);
  public currencySymbol = signal<string>('$');
  public company = signal<MyCompanyResponseDto | null>(null);

  public customerOptions = signal<AutocompleteOption[]>([]);
  public isCustomersLoading = signal<boolean>(false);

  public productOptionsArray = signal<AutocompleteOption[][]>([]);
  public isProductsLoadingArray = signal<boolean[]>([]);

  public invoiceForm: FormGroup = this.fb.group({
    clientName: ['', [Validators.required, Validators.minLength(3)]],
    clientIdentification: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(13)]],
    details: this.fb.array([])
  });

  get details(): FormArray {
    return this.invoiceForm.get('details') as FormArray;
  }

  public resetForCreate(): void {
    this.invoiceForm.reset();
    while (this.details.length !== 0) {
      this.details.removeAt(0);
    }

    this.productOptionsArray.set([]);
    this.isProductsLoadingArray.set([]);

    this.addDetailRow();
    this.calculateTotals();
    this.loadCompanySettings();
  }

  public addDetailRow(): void {
    const detailRow = this.fb.group({
      productId: ['', [Validators.required]],
      productBatchId: ['', [Validators.required]],
      quantity: [1, [Validators.required, Validators.min(1)]],
      unitPrice: [0.00, [Validators.required, Validators.min(0.01)]]
    });

    this.details.push(detailRow);

    this.productOptionsArray.update(arr => [...arr, []]);
    this.isProductsLoadingArray.update(arr => [...arr, false]);

    this.calculateTotals();
  }

  public removeDetailRow(index: number): void {
    if (this.details.length > 1) {
      this.details.removeAt(index);

      this.productOptionsArray.update(arr => arr.filter((_, i) => i !== index));
      this.isProductsLoadingArray.update(arr => arr.filter((_, i) => i !== index));

      this.calculateTotals();
    }
  }

  /**
   * Busca clientes en el endpoint paginado y llena las opciones del maestro
   */
  public searchCustomers(term: string): void {
    if (term.length < 3) {
      this.customerOptions.set([]);
      return;
    }

    this.isCustomersLoading.set(true);
    this.customerService.apiCustomersGet(1, 10, term)
      .pipe(
        debounceTime(300)
      )
      .subscribe({
      next: (response: any) => {
        if (response && response.success && response.data) {
          const mapped: AutocompleteOption[] = response.data.items.map((c: any) => ({
            value: c.id,
            label: c.fullName,
            subLabel: `RUC/CÉDULA: ${c.identification} | ${c.email}`
          }));
          this.customerOptions.set(mapped);
        }
      },
      error: () => this.isCustomersLoading.set(false),
      complete: () => this.isCustomersLoading.set(false)
    });
  }

  /**
   * Gatillado cuando se pincha un cliente del Autocomplete.
   * Rompe el comportamiento común del valueAccessor para setear Nombre e Identificación directo en el Form.
   */
  public onCustomerSelected(customerId: string): void {
    if (!customerId) return;

    const rawOption = this.customerOptions().find(opt => opt.value === customerId);
    if (rawOption) {

      const identification = rawOption.subLabel?.split('|')[0].replace('RUC/CÉDULA:', '').trim() || '';

      this.invoiceForm.patchValue({
        clientName: rawOption.label,
        clientIdentification: identification
      });
    }
  }

  /**
   * Busca productos en el endpoint paginado afectando ÚNICAMENTE al índice de la fila que escribe
   */
  public searchProducts(term: string, index: number): void {
    if (term.length < 2) {
      this.updateProductOptionsSlot(index, []);
      return;
    }

    this.updateProductLoadingSlot(index, true);

    this.productService.apiProductsGet(1, 10, term).subscribe({
      next: (response: any) => {
        if (response && response.success && response.data) {
          const mapped: AutocompleteOption[] = response.data.items.map((p: any) => ({
            value: p.id,
            label: p.name,
            subLabel: `SKU: ${p.sku || 'N/A'} | Stock Disponible: ${p.totalStock ?? 0} u.`
          }));
          this.updateProductOptionsSlot(index, mapped);
        }
      },
      error: () => this.updateProductLoadingSlot(index, false),
      complete: () => this.updateProductLoadingSlot(index, false)
    });
  }

  public onProductSelected(option: AutocompleteOption, index: number): void {
    if (!option?.value) return;

    const productId = option.value as string;
    this.fetchProductBatchAndPrice(productId, index);
  }

  private fetchProductBatchAndPrice(productId: string, index: number): void {
    this.productService.apiProductsIdGet(productId).subscribe({
      next: (response: any) => {
        if (!(response && response.success && response.data)) return;

        const product = response.data;
        const batch = product.activeBatches?.find((b: any) => b.currentQuantity && b.currentQuantity > 0) ?? product.activeBatches?.[0];

        const detailRow = this.details.at(index);
        if (!detailRow) return;

        if (batch?.id) {
          detailRow.get('productBatchId')?.setValue(batch.id);
        }
        if (batch?.unitCost != null) {
          detailRow.get('unitPrice')?.setValue(batch.unitCost);
          this.calculateTotals();
        }
      }
    });
  }

  private loadCompanySettings(): void {
    this.profileService.apiProfileCompanyGet().subscribe({
      next: (response: any) => {
        if (!(response && response.success && response.data)) return;

        this.company.set(response.data);
        this.taxRate.set(response.data.iva != null ? response.data.iva / 100 : 0.15);
        this.currencySymbol.set(response.data.currencySymbol ?? '$');
        this.calculateTotals();
      }
    });
  }

  private updateProductOptionsSlot(index: number, options: AutocompleteOption[]): void {
    this.productOptionsArray.update(arr => {
      const clone = [...arr];
      clone[index] = options;
      return clone;
    });
  }

  private updateProductLoadingSlot(index: number, loading: boolean): void {
    this.isProductsLoadingArray.update(arr => {
      const clone = [...arr];
      clone[index] = loading;
      return clone;
    });
  }

  // ==========================================
  // CÁLCULOS FINANCIEROS Y MÉTODOS DE CONTROL
  // ==========================================

  public calculateTotals(): void {
    let currentSubtotal = 0;
    this.details.controls.forEach((control) => {
      const quantity = control.get('quantity')?.value || 0;
      const unitPrice = control.get('unitPrice')?.value || 0;
      currentSubtotal += quantity * unitPrice;
    });

    const currentTax = currentSubtotal * this.taxRate();
    const currentTotal = currentSubtotal + currentTax;

    this.subtotal.set(currentSubtotal);
    this.tax.set(currentTax);
    this.total.set(currentTotal);
  }

  public ngOnInit(): void {
    this.loadCompanySettings();
  }

  public submit(): void {
    if (this.invoiceForm.invalid || this.details.length === 0) {
      this.invoiceForm.markAllAsTouched();
      return;
    }
    this.onSubmitForm.emit(this.invoiceForm.value);
  }

  public isFieldInvalid(field: string): boolean {
    const control = this.invoiceForm.get(field);
    return !!(control && control.touched && control.invalid);
  }
}
