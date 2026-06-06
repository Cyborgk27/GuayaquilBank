import { CommonModule } from '@angular/common';
import { Component, ElementRef, forwardRef, HostListener, inject, input, output, signal } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms';
import { AutocompleteOption } from '../../interfaces/autocomplete-option.interface';

@Component({
  selector: 'app-generic-autocomplete',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './generic-autocomplete.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => GenericAutocomplete),
      multi: true
    }
  ]
})
export class GenericAutocomplete implements ControlValueAccessor {
  private elementRef = inject(ElementRef);

  public label = input<string>('');
  public placeholder = input<string>('Buscar opción...');
  public options = input<AutocompleteOption[]>([]);
  public isLoading = input<boolean>(false);
  public isInvalid = input<boolean>(false);
  public errorMessage = input<string>('');

  public onQueryChange = output<string>();
  public onSelect = output<AutocompleteOption>();

  public query = signal<string>('');
  public isOpen = signal<boolean>(false);
  public isDisabled = signal<boolean>(false);

  private selectedOption: AutocompleteOption | null = null;

  private onChange: any = () => {};
  private onTouch: any = () => {};

  /**
   * Cierra el menú desplegable si el usuario hace clic fuera del componente
   */
  @HostListener('document:click', ['$event'])
  public clickOutside(event: Event): void {
    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.closeDropdown();
    }
  }

  public onInputChange(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.query.set(target.value);
    this.isOpen.set(true);
    this.onQueryChange.emit(target.value);

    if (!target.value) {
      this.clearSelection();
    }
  }

  public selectOption(option: AutocompleteOption): void {
    this.selectedOption = option;
    this.query.set(option.label);
    this.isOpen.set(false);

    this.onChange(option.value);
    this.onTouch();
    this.onSelect.emit(option);
  }

  public clearSelection(): void {
    this.selectedOption = null;
    this.query.set('');
    this.onChange('');
    this.onTouch();
  }

  public toggleDropdown(): void {
    if (this.isDisabled()) return;
    this.isOpen.update(v => !v);
  }

  private closeDropdown(): void {
    this.isOpen.set(false);
    if (this.selectedOption) {
      this.query.set(this.selectedOption.label);
    } else if (!this.onChange.value) {
      this.query.set('');
    }
  }

  // ==========================================
  // IMPLEMENTACIÓN DE CONTROL VALUE ACCESSOR
  // ==========================================

  public writeValue(val: any): void {
    if (val) {
      const matched = this.options().find(opt => opt.value === val);
      if (matched) {
        this.selectedOption = matched;
        this.query.set(matched.label);
        return;
      }
    }
    
    if (!val) {
      this.selectedOption = null;
      this.query.set('');
    }
  }

  public registerOnChange(fn: any): void { this.onChange = fn; }
  public registerOnTouched(fn: any): void { this.onTouch = fn; }

  public setDisabledState(disabled: boolean): void {
    this.isDisabled.set(disabled);
  }
}
