import { Component, input, effect } from '@angular/core';
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

  value: unknown = null;
  selectedIndex: number | null = null;

  constructor() {
    super();
    effect(() => {
      // Re-sync selectedIndex whenever options() changes
      this.options();
      this.syncSelectedIndex();
    });
  }

  override writeValue(value: unknown): void {
    this.value = value;
    this.syncSelectedIndex();
  }

  private syncSelectedIndex(): void {
    const opts = this.options();
    if (!opts || opts.length === 0) {
      this.selectedIndex = null;
      return;
    }

    const val = this.value;
    const idx = opts.findIndex((option) =>
      option.value === val ||
      (option.value != null && val != null && String(option.value) === String(val))
    );

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
