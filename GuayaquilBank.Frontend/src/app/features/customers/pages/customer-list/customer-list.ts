import { Component, inject, signal, OnInit, effect, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CustomersApiService, CustomerResponseDto } from '../../../../core/api/v1';
import { GenericTable } from '../../../../shared/components/generic-table/generic-table';
import { TableColumn } from '../../../../shared/interfaces/table-column.interface';
import { TableAction } from '../../../../shared/interfaces/table-action.interface';
import { DataFilter } from '../../../../shared/components/data-filter/data-filter';
import { GenericModal } from "../../../../shared/components/generic-modal/generic-modal";
import { CustomerForm } from '../../components/customer-form/customer-form';
import { Ui } from '../../../../core/service/ui';

@Component({
  selector: 'app-customer-list',
  standalone: true,
  imports: [CommonModule, GenericTable, DataFilter, GenericModal, CustomerForm],
  templateUrl: './customer-list.html'
})
export class CustomerList implements OnInit {
  @ViewChild('customerModal') customerModal!: GenericModal;
  @ViewChild('customerFormComponent') customerForm!: CustomerForm;

  private customersApiService = inject(CustomersApiService);
  private ui = inject(Ui);

  public customers = signal<CustomerResponseDto[]>([]);
  public isLoading = signal<boolean>(false);
  public isModalLoading = signal<boolean>(false);
  public page = signal<number>(1);
  public pageSize = signal<number>(5);
  public search = signal<string>('');

  public totalItems = signal<number>(0);
  public totalPages = signal<number>(0);

  public tableColumns: TableColumn[] = [
    { key: 'identification', label: 'RUC / Identificación' },
    { key: 'fullName', label: 'Razón Social / Nombre' },
    { key: 'email', label: 'Correo Electrónico' },
    { key: 'phoneNumber', label: 'Teléfono' },
    { key: 'address', label: 'Dirección' },
    {
      key: 'isActive',
      label: 'Estado',
      type: 'badge',
      badgeClass: (value: boolean) => value ? 'badge-success text-white' : 'badge-ghost text-base-content/40'
    }
  ];

  public tableActions: TableAction<CustomerResponseDto>[] = [
    {
      icon: 'pi-pencil',
      label: 'Editar Cliente',
      colorClass: 'text-primary hover:bg-primary/10',
      callback: (customer) => this.openEditModal(customer)
    },
    {
      icon: 'pi-trash',
      label: 'Eliminar Cliente',
      colorClass: 'text-error hover:bg-error/10',
      callback: (customer) => this.deleteCustomer(customer)
    },
    {
      icon: 'pi-power-off',
      label: (customer) => customer.isActive ? 'Desactivar Cliente' : 'Activar Cliente',
      colorClass: (customer) => customer.isActive ? 'text-error hover:bg-error/10' : 'text-success hover:bg-success/10',
      callback: (customer) => this.deactivateCustomer(customer)
    }

  ];

  constructor() {
    effect(() => {
      this.loadCustomers();
    }, { allowSignalWrites: true });
  }

  ngOnInit(): void { }

  public loadCustomers(): void {
    this.isLoading.set(true);
    this.customersApiService.apiCustomersGet(
      this.page(),
      this.pageSize(),
      this.search()
    ).subscribe({
      next: (response: any) => {
        if (response && response.success && response.data) {
          this.customers.set(response.data.items || []);
          this.totalItems.set(response.data.totalItems || 0);
          this.totalPages.set(response.data.totalPages || 0);
        }
      },
      error: () => this.isLoading.set(false),
      complete: () => this.isLoading.set(false)
    });
  }

  public handleSearch(term: string): void {
    this.search.set(term);
    this.page.set(1);
  }

  /**
   * FLUJO: CREACIÓN
   */
  public openCreateModal(): void {
    this.customerForm.resetForCreate();
    this.customerModal.open();
  }

  /**
   * FLUJO: EDICIÓN CON GET_BY_ID
   */
  public openEditModal(customer: CustomerResponseDto): void {
    if (!customer.id) return;

    this.isModalLoading.set(true);
    this.customerModal.open();

    this.customersApiService.apiCustomersIdGet(customer.id).subscribe({
      next: (response: any) => {
        if (response && response.success && response.data) {
          this.customerForm.setFormValue(response.data);
        }
      },
      error: () => this.customerModal.close(),
      complete: () => this.isModalLoading.set(false)
    });
  }

  /**
   * PROCESAMIENTO UNIFICADO DE DATOS (POST / PUT)
   */
  public handleFormSubmit(formData: any): void {
    this.isModalLoading.set(true);

    const request$ = this.customerForm.isEditMode()
      ? this.customersApiService.apiCustomersIdPut(formData.id, formData) // Modo Edición (PUT)
      : this.customersApiService.apiCustomersPost(formData);              // Modo Creación (POST)

    request$.subscribe({
      next: (response: any) => {
        if (response && response.success) {
          this.customerModal.close();
          this.loadCustomers();
        }
      },
      error: () => this.isModalLoading.set(false),
      complete: () => this.isModalLoading.set(false)
    });
  }

  public async deleteCustomer(customer: CustomerResponseDto): Promise<void> {
    const isConfirmed = await this.ui.showConfirm(
      'Eliminar Cliente',
      `¿Estás seguro de que deseas eliminar al cliente "${customer.fullName}"?`
    );

    if (isConfirmed && customer.id) {
      this.customersApiService.apiCustomersIdDelete(customer.id).subscribe({
        next: () => {
          this.loadCustomers();
        },
        error: () => {
          this.ui.showError('Error', 'No se pudo eliminar el cliente.');
        }
      });
    }
  }

  public async deactivateCustomer(customer: CustomerResponseDto): Promise<void> {
    const action = customer.isActive ? 'Desactivar' : 'Activar';
    const isConfirmed = await this.ui.showConfirm(
      `${action} Cliente`,
      `¿Estás seguro de que deseas ${action.toLowerCase()} al cliente "${customer.fullName}"?`
    );

    if (isConfirmed && customer.id) {
      this.customersApiService.apiCustomersIdToggleStatusPatch(customer.id).subscribe({
        next: (res) => {
          this.loadCustomers();
          this.ui.showSuccess('Éxito', res.message || `Cliente ${action.toLowerCase()}ado exitosamente.`);

        },
        error: () => {
          this.ui.showError('Error', `No se pudo ${action.toLowerCase()} el cliente.`);
        }
      });
    }
  }
}
