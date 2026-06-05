import { Component, Input } from '@angular/core';
import { TableColumn } from '../../interfaces/table-column.interface';
import { TableAction } from '../../interfaces/table-action.interface';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-generic-table',
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

  /**
   * Helper para obtener de forma segura el valor de una celda tipada
   */
  public getCellValue(row: T, key: string): any {
    return row[key];
  }
}
