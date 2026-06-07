import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'login',
    title: 'Iniciar Sesión - Guayaquil Bank ERP',
    loadComponent: () => import('./features/auth/pages/login/login')
      .then(m => m.Login)
  },
  {
    path: '',
    loadComponent: () => import('./shared/components/layout/layout/layout')
      .then(m => m.Layout),
    children: [
      {
        path: 'customers',
        title: 'Gestión de Clientes - Guayaquil Bank ERP',
        loadComponent: () => import('./features/customers/pages/customer-list/customer-list')
          .then(m => m.CustomerList)
      },
      {
        path: 'products',
        title: 'Gestión de Productos - Guayaquil Bank ERP',
        loadComponent: () => import('./features/products/pages/product-list/product-list')
          .then(m => m.ProductList)
      },
      {
        path: 'sales',
        title: 'Gestión de Facturas - Guayaquil Bank ERP',
        loadComponent: () => import('./features/invoices/pages/invoice-list/invoice-list')
          .then(m => m.InvoiceList)
      }
    ]
  }
];
