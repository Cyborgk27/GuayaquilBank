import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'login',
    title: 'Iniciar Sesión - Guayaquil Bank ERP',
    loadComponent: () => import('./features/auth/pages/login/login')
      .then(m => m.Login)
  },
];
