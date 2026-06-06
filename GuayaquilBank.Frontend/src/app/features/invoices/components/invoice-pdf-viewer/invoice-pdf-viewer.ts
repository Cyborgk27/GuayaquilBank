import { Component, inject, input, signal } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { SalesApiService } from '../../../../core/api/v1';

@Component({
  selector: 'app-invoice-pdf-viewer',
  imports: [CommonModule],
  templateUrl: './invoice-pdf-viewer.html',
  styleUrl: './invoice-pdf-viewer.css',
})
export class InvoicePdfViewer {
  private salesService = inject(SalesApiService);
  private sanitizer = inject(DomSanitizer);

  public invoiceId = input.required<string>();

  public pdfUrl = signal<SafeResourceUrl | null>(null);
  public isLoading = signal<boolean>(true);
  public hasError = signal<boolean>(false);

  private objectUrl: string | null = null;

  ngOnInit(): void {
    this.loadPdf();
  }

  ngOnDestroy(): void {
    if (this.objectUrl) {
      URL.revokeObjectURL(this.objectUrl);
    }
  }

  public loadPdf(): void {
    this.isLoading.set(true);
    this.hasError.set(false);

    this.salesService.apiSalesIdPdfGet(this.invoiceId()).subscribe({
      next: (blob: Blob) => {
        this.objectUrl = URL.createObjectURL(blob);

        const safeUrl = this.sanitizer.bypassSecurityTrustResourceUrl(this.objectUrl);

        this.pdfUrl.set(safeUrl);
        this.isLoading.set(false);
      },
      error: () => {
        this.hasError.set(true);
        this.isLoading.set(false);
      }
    });
  }

  /**
   * Abre el documento en una nueva pestaña usando el comportamiento nativo del navegador
   */
  public openInNewTab(): void {
    if (this.objectUrl) {
      window.open(this.objectUrl, '_blank');
    }
  }
}
