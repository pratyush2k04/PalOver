import { Component, input, output, signal } from '@angular/core';

export interface AutocompleteOption {
  id: number | string;
  label: string;
}

@Component({
  selector: 'app-autocomplete',
  standalone: true,
  templateUrl: './autocomplete.html',
  styleUrl: './autocomplete.scss'
})
export class AutocompleteComponent {
  readonly placeholder = input<string>('');
  readonly searchTerm = input<string>('');
  readonly options = input.required<AutocompleteOption[]>();
  readonly emptyMessage = input<string>('No matching results');

  readonly searchTermChange = output<string>();
  readonly optionSelected = output<AutocompleteOption>();

  readonly isOpen = signal(false);

  onFocus(): void {
    this.isOpen.set(true);
  }

  onBlur(): void {
    // Delay closing so a mousedown on a dropdown option registers before the panel disappears.
    setTimeout(() => this.isOpen.set(false), 200);
  }

  onInput(value: string): void {
    this.searchTermChange.emit(value);
  }

  selectOption(option: AutocompleteOption): void {
    this.optionSelected.emit(option);
  }
}
