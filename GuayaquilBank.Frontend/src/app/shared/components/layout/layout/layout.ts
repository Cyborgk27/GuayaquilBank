import { Component, inject } from '@angular/core';
import { AuthFacade } from '../../../../core/service/auth.facade';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-layout',
  imports: [
    CommonModule,
    RouterOutlet,
    RouterLink,
    RouterLinkActive
  ],
  templateUrl: './layout.html',
  styleUrl: './layout.css',
})
export class Layout {
  public authService = inject(AuthFacade);
  private router = inject(Router);

  public menuItems = [
    { label: 'Panel de Control', icon: 'pi-home', route: '/dashboard' },
    { label: 'Clientes', icon: 'pi-users', route: '/customers' },
    { label: 'Productos', icon: 'pi-box', route: '/products' },
    { label: 'Facturación / Ventas', icon: 'pi-wallet', route: '/sales' },
    { label: 'Usuarios', icon: 'pi-user', route: '/users' },
    { label: 'Configuración', icon: 'pi-cog', route: '/profile' },
  ];

  public onLogout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
