import { Routes } from '@angular/router';
import { authGuard, noAuthGuard } from './core/guards';

export const routes: Routes = [
  {
    path: 'login',
    title: 'Iniciar Sesión - Guayaquil Bank ERP',
    canActivate: [noAuthGuard],
    loadComponent: () => import('./features/auth/pages/login/login')
      .then(m => m.Login)
  },
  {
    path: '',
    canActivate: [authGuard],
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
      },
      {
        path: 'users',
        title: 'Gestión de Usuarios - Guayaquil Bank ERP',
        loadComponent: () => import('./features/users/components/users-list/users-list')
          .then(m => m.UsersList)
      },
      {
        path: 'profile',
        title: 'Perfil de Usuario - Guayaquil Bank ERP',
        loadComponent: () => import('./features/profile/profile').then(m => m.Profile)
      }
    ]
  }
];
