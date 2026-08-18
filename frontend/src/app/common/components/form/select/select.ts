import { Component, input } from '@angular/core';
import { BaseControlComponent } from '../base-control';

export interface SelectOption {
  value: unknown;
  label: string;
}

@Component({
  selector: 'app-select',
  standalone: true,
  templateUrl: './select.html',
  styleUrl: './select.scss'
})
export class SelectComponent extends BaseControlComponent<unknown> {
  readonly id = input.required<string>();
  readonly label = input<string | null>(null);
  readonly options = input.required<SelectOption[]>();
  readonly placeholder = input<string | null>(null);

  value: unknown = null;
  selectedIndex: number | null = null;

  writeValue(value: unknown): void {
    this.value = value;
    const idx = this.options().findIndex((option) => option.value === value);
    this.selectedIndex = idx >= 0 ? idx : null;
  }

  onSelectChange(indexStr: string): void {
    const idx = indexStr === '' ? null : Number(indexStr);
    this.selectedIndex = idx;
    const option = idx !== null ? this.options()[idx] : undefined;
    this.value = option ? option.value : null;
    this.onChange(this.value);
  }
}
