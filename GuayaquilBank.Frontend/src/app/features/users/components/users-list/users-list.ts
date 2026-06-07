import { CommonModule } from '@angular/common';
import { Component, effect, inject, OnInit, signal, ViewChild } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UserResponseDto, UsersApiService } from '../../../../core/api/v1';
import { Ui } from '../../../../core/service/ui';
import { DataFilter } from '../../../../shared/components/data-filter/data-filter';
import { GenericInput } from '../../../../shared/components/generic-input/generic-input';
import { GenericModal } from '../../../../shared/components/generic-modal/generic-modal';
import { GenericTable } from '../../../../shared/components/generic-table/generic-table';
import { TableAction } from '../../../../shared/interfaces/table-action.interface';
import { TableColumn } from '../../../../shared/interfaces/table-column.interface';
import { UserForm } from '../user-form/user-form';

@Component({
  selector: 'app-users-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, GenericInput, UserForm, GenericTable, GenericModal, DataFilter],
  templateUrl: './users-list.html',
  styleUrl: './users-list.css'
})
export class UsersList implements OnInit {
  @ViewChild('userModal') userModal!: GenericModal;
  @ViewChild('userFormComponent') userForm!: UserForm;

  private userService = inject(UsersApiService);
  private ui = inject(Ui);

  public users = signal<UserResponseDto[]>([]);
  public selectedUser = signal<UserResponseDto | null>(null);
  public isLoading = signal<boolean>(false);
  public isModalLoading = signal<boolean>(false);
  public page = signal<number>(1);
  public pageSize = signal<number>(10);
  public search = signal<string>('');

  public totalItems = signal<number>(0);
  public totalPages = signal<number>(0);

  public tableColumns: TableColumn[] = [
    { key: 'username', label: 'Usuario' },
    { key: 'email', label: 'Correo' },
    { key: 'fullName', label: 'Nombre' }
  ];

  public tableActions: TableAction<UserResponseDto>[] = [
    { icon: 'pi pi-pencil', label: 'Editar', colorClass: 'btn-ghost', callback: (u) => this.openEdit(u) },
    { icon: 'pi pi-trash', label: 'Eliminar', colorClass: 'btn-ghost text-error', callback: (u) => this.deleteUser(u) }
  ];

  constructor() {
    effect(() => {
      this.load();
    }, { allowSignalWrites: true });
  }

  ngOnInit(): void { }

  public handleSearch(term: string): void {
    this.search.set(term);
    this.page.set(1);
  }

  load(): void {
    this.isLoading.set(true);
    this.userService.apiUsersGet(
      this.page(),
      this.pageSize(),
      this.search())
      .subscribe({
      next: (response: any) => {
        if (response && response.success) {
          this.users.set(response.data.items);
          this.totalItems.set(response.data.totalItems);
          this.totalPages.set(response.data.totalPages);
        }
      },
      error: () => this.isLoading.set(false),
      complete: () => this.isLoading.set(false)
    });
  }

  openCreate(): void {
    this.selectedUser.set(null);
    this.userModal.open();
  }

  openEdit(user: UserResponseDto): void {
    if(!user.id) return;
    this.userService.apiUsersIdGet(user.id).subscribe({
      next: (response: any) => {
        if (response && response.success) {
          this.userForm.setFormValue(response.data);
          this.userModal.open();
        }
      }
    });
  }

  onSaved(payload: any): void {
    this.isModalLoading.set(true);

    const request$ = payload.id
      ? this.userService.apiUsersIdPut(payload.id, payload)
      : this.userService.apiUsersPost(payload);

    request$.subscribe({
      next: (response: any) => {
        if (response && response.success) {
          this.userModal.close();
          this.load();
          this.ui.showSuccess(response.message || `Usuario ${payload.id ? 'actualizado' : 'creado'} exitosamente.`);
        }
      },
      error: () => this.isModalLoading.set(false),
      complete: () => this.isModalLoading.set(false)
    })
  }

  onCancel(): void {
    this.userModal.close();
  }

  deleteUser(user: any): void {
    this.ui.showConfirm('Eliminar usuario', '¿Deseas eliminar este usuario?').then((confirmed) => {
      if (!confirmed) return;
      this.userService.apiUsersIdDelete(user.id).subscribe({
        next: (resp: any) => {
          this.load();
          this.ui.showSuccess(resp?.message || 'Usuario eliminado.');
        }
      });
    });
  }
}
