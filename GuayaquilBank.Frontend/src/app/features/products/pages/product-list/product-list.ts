import { CommonModule } from '@angular/common';
import { Component, effect, inject, OnInit, signal, ViewChild } from '@angular/core';
import { CreateProductBatchRequestDto, CreateProductRequestDto, ProductResponseDto, ProductsApiService } from '../../../../core/api/v1';
import { DataFilter } from '../../../../shared/components/data-filter/data-filter';
import { GenericModal } from '../../../../shared/components/generic-modal/generic-modal';
import { GenericTable } from '../../../../shared/components/generic-table/generic-table';
import { TableAction } from '../../../../shared/interfaces/table-action.interface';
import { TableColumn } from '../../../../shared/interfaces/table-column.interface';
import { ProductBatchForm } from '../../components/product-batch-form/product-batch-form';
import { ProductForm } from '../../components/product-form/product-form';
import { Ui } from '../../../../core/service/ui';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, GenericTable, DataFilter, GenericModal, ProductForm, ProductBatchForm],
  templateUrl: './product-list.html',
  styleUrls: ['./product-list.css']
})
export class ProductList implements OnInit {
  @ViewChild('productModal') productModal!: GenericModal;
  @ViewChild('batchModal') batchModal!: GenericModal;
  @ViewChild('productFormComponent') productForm!: ProductForm;
  @ViewChild('productBatchFormComponent') productBatchForm!: ProductBatchForm;

  private productsApiService = inject(ProductsApiService);
  private ui = inject(Ui);

  public products = signal<ProductResponseDto[]>([]);
  public selectedProduct = signal<ProductResponseDto | null>(null);
  public isLoading = signal<boolean>(false);
  public isModalLoading = signal<boolean>(false);
  public page = signal<number>(1);
  public pageSize = signal<number>(5);
  public search = signal<string>('');

  public totalItems = signal<number>(0);
  public totalPages = signal<number>(0);

  public tableColumns: TableColumn[] = [
    { key: 'sku', label: 'SKU' },
    { key: 'name', label: 'Nombre del Producto' },
    { key: 'description', label: 'Descripción' },
    { key: 'totalStock', label: 'Stock Disponible' }
  ];

  public tableActions: TableAction<ProductResponseDto>[] = [
    {
      icon: 'pi-box',
      label: 'Agregar lote',
      colorClass: 'btn-primary',
      callback: (product) => this.openAddBatchModal(product)
    }
  ];

  constructor() {
    effect(() => {
      this.loadProducts();
    }, { allowSignalWrites: true });
  }

  ngOnInit(): void { }

  public loadProducts(): void {
    this.isLoading.set(true);
    this.productsApiService.apiProductsGet(
      this.page(),
      this.pageSize(),
      this.search()
    ).subscribe({
      next: (response: any) => {
        if (response && response.success && response.data) {
          this.products.set(response.data.items || []);
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

  public openCreateModal(): void {
    this.productForm.resetForCreate();
    this.productModal.open();
  }

  public openAddBatchModal(product: ProductResponseDto): void {
    this.selectedProduct.set(product);
    this.productBatchForm.reset();
    this.batchModal.open();
  }

  public handleFormSubmit(payload: CreateProductRequestDto): void {
    this.isModalLoading.set(true);

    this.productsApiService.apiProductsPost(payload).subscribe({
      next: (response: any) => {
        if (response && response.success) {
          this.productModal.close();
          this.loadProducts();
          this.ui.showSuccess(response.message || 'Producto creado exitosamente.');
        }
      },
      error: () => this.isModalLoading.set(false),
      complete: () => this.isModalLoading.set(false)
    });
  }

  public handleBatchSubmit(payload: CreateProductBatchRequestDto): void {
    const product = this.selectedProduct();
    if (!product?.id) {
      return;
    }

    this.isModalLoading.set(true);
    this.productsApiService.apiProductsIdBatchesPost(product.id, payload).subscribe({
      next: (response: any) => {
        if (response && response.success) {
          this.batchModal.close();
          this.loadProducts();

          this.ui.showSuccess(response.message || 'Lote agregado exitosamente.');
        }
      },
      error: () => this.isModalLoading.set(false),
      complete: () => this.isModalLoading.set(false)
    });
  }
}
