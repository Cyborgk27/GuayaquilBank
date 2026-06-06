import { Component, Input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableColumn } from '../../interfaces/table-column.interface';
import { TableAction } from '../../interfaces/table-action.interface';

@Component({
  selector: 'app-generic-table',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './generic-table.html',
  styleUrl: './generic-table.css',
})
export class GenericTable<T extends Record<string, any>> {
  @Input() columns: TableColumn[] = [];
  @Input() data: T[] = [];
  @Input() isLoading: boolean = false;
  @Input() actions: TableAction<T>[] = [];
  @Input() emptyMessage: string = 'No se encontraron registros en este dominio.';
  @Input() emptyIcon: string = 'pi-box';
  @Input() currentPage: number = 1;
  @Input() totalPages: number = 0;
  @Input() totalItems: number = 0;

  public pageChange = output<number>();

  /**
   * Helper para obtener de forma segura el valor de una celda tipada
   */
  public getCellValue(row: T, key: string): any {
    return row[key];
  }

  /**
   * Gestiona el cambio de página interno y emite el evento
   */
  public onPageChange(newPage: number): void {
    if (newPage >= 1 && newPage <= this.totalPages && newPage !== this.currentPage) {
      this.pageChange.emit(newPage);
    }
  }

  /**
 * Verifica si el label provisto es una función dinámica
 */
  public isFunction(label: any): boolean {
    return typeof label === 'function';
  }

  /**
   * Helper de casteo para que el template ejecute la función sin problemas de tipos
   */
  public asFunction(label: any): (row: T) => string {
    return label as (row: T) => string;
  }
}
