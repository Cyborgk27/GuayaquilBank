import { CommonModule } from '@angular/common';
import { Component, input, output, signal } from '@angular/core';

@Component({
  selector: 'app-generic-modal',
  imports: [CommonModule],
  templateUrl: './generic-modal.html',
  styleUrl: './generic-modal.css',
})
export class GenericModal {
  public title = input<string>('Formulario de Registro');
  public sizeClass = input<string>('max-w-lg');
  public onClose = output<void>();
  public isOpen = signal<boolean>(false);

  /**
   * Abre el modal de forma pública
   */
  public open(): void {
    this.isOpen.set(true);
  }

  /**
   * Cierra el modal y notifica al componente padre
   */
  public close(): void {
    this.isOpen.set(false);
    this.onClose.emit();
  }
}
