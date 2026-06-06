import { Component, inject, signal, OnInit, effect, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SalesApiService, InvoiceResponseDto, CreateInvoiceRequestDto } from '../../../../core/api/v1'; // Ajusta la ruta según tu SDK
import { GenericTable } from '../../../../shared/components/generic-table/generic-table';
import { TableColumn } from '../../../../shared/interfaces/table-column.interface';
import { TableAction } from '../../../../shared/interfaces/table-action.interface';
import { DataFilter } from '../../../../shared/components/data-filter/data-filter';
import { GenericModal } from "../../../../shared/components/generic-modal/generic-modal";
import { InvoicePdfViewer } from '../../components/invoice-pdf-viewer/invoice-pdf-viewer';
import { InvoiceForm } from "../../components/invoice-form/invoice-form";

@Component({
  selector: 'app-invoice-list',
  standalone: true,
  imports: [CommonModule, GenericTable, DataFilter, GenericModal, InvoicePdfViewer, InvoiceForm],
  templateUrl: './invoice-list.html'
})
export class InvoiceList implements OnInit {
  @ViewChild('pdfModal') pdfModal!: GenericModal;
  @ViewChild('createInvoiceModal') createInvoiceModal!: GenericModal;
  @ViewChild('invoiceFormComponent') invoiceForm!: InvoiceForm;

  private salesService = inject(SalesApiService);

  public invoices = signal<InvoiceResponseDto[]>([]);
  public isLoading = signal<boolean>(false);

  // Parámetros de paginación y búsqueda reactiva
  public page = signal<number>(1);
  public pageSize = signal<number>(5);
  public search = signal<string>('');

  public totalItems = signal<number>(0);
  public totalPages = signal<number>(0);

  public selectedInvoiceId = signal<string | null>(null);

  public tableColumns: TableColumn[] = [
    { key: 'invoiceNumber', label: 'Nº Comprobante' },
    { key: 'clientName', label: 'Cliente / Razón Social' },
    {
      key: 'issuedAt',
      label: 'Fecha Emisión',
    },
    {
      key: 'total',
      label: 'Total ($)',
      badgeClass: () => 'font-bold text-base-content'
    },
  ];

  public tableActions: TableAction<InvoiceResponseDto>[] = [
    {
      icon: 'pi-file-pdf',
      label: 'Visualizar PDF',
      colorClass: 'text-error hover:bg-error/10',
      callback: (invoice) => this.viewInvoicePdf(invoice)
    }
  ];

  constructor() {
    effect(() => {
      this.loadInvoices();
    }, { allowSignalWrites: true });
  }

  ngOnInit(): void { }

  /**
   * Carga el listado de facturas
   */
  public loadInvoices(): void {
    this.isLoading.set(true);

    this.salesService.apiSalesGet(
      this.page(),
      this.pageSize(),
      this.search()
    ).subscribe({
      next: (response: any) => {
        if (response && response.success && response.data) {
          this.invoices.set(response.data.items || []);
          this.totalItems.set(response.data.totalItems || 0);
          this.totalPages.set(response.data.totalPages || 0);
        }
      },
      error: () => this.isLoading.set(false),
      complete: () => this.isLoading.set(false)
    });
  }

  /**
   * Manejador del filtro de búsqueda con debounce
   */
  public handleSearch(term: string): void {
    this.search.set(term);
    this.page.set(1);
  }

  /**
   * Abre el modal e inyecta el ID para que el visor descargue el flujo binario
   */
  public viewInvoicePdf(invoice: InvoiceResponseDto): void {
    if (!invoice.id) return;
    this.selectedInvoiceId.set(invoice.id);
    this.pdfModal.open();
  }

  /**
   * Limpia el estado al cerrar el modal para desmontar el iframe de memoria
   */
  public closePdfModal(): void {
    this.pdfModal.close();
    this.selectedInvoiceId.set(null);
  }

  public openCreateInvoiceModal(): void {
    this.invoiceForm.resetForCreate();
    this.createInvoiceModal.open();
  }

  public handleInvoiceSubmit(payload: CreateInvoiceRequestDto): void {
    this.isLoading.set(true);

    this.salesService.apiSalesPost(payload).subscribe({
      next: (response: any) => {
        if (response && response.success) {
          this.createInvoiceModal.close();
          this.loadInvoices(); // Refresca tu grid de facturas automáticamente
        }
      },
      error: () => this.isLoading.set(false)
    });
  }
}
