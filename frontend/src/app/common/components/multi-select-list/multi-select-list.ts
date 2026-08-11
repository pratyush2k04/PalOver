import { Component, input, output } from '@angular/core';
import { SelectOption } from '../form/select/select';

@Component({
  selector: 'app-multi-select-list',
  standalone: true,
  templateUrl: './multi-select-list.html',
  styleUrl: './multi-select-list.scss'
})
export class MultiSelectListComponent {
  readonly options = input.required<SelectOption[]>();
  readonly selectedValues = input<unknown[]>([]);
  readonly variant = input<'list' | 'cards'>('list');
  readonly emptyMessage = input<string | null>(null);

  readonly selectedValuesChange = output<unknown[]>();

  isSelected(value: unknown): boolean {
    return this.selectedValues().includes(value);
  }

  toggle(value: unknown, checked: boolean): void {
    const current = this.selectedValues();
    const next = checked ? [...current, value] : current.filter((v) => v !== value);
    this.selectedValuesChange.emit(next);
  }
}
