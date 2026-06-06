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
      ]
  }
];
