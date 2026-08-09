import { Component, input, output } from '@angular/core';
import { BaseControlComponent } from '../base-control';

@Component({
  selector: 'app-checkbox',
  standalone: true,
  templateUrl: './checkbox.html',
  styleUrl: './checkbox.scss'
})
export class CheckboxComponent extends BaseControlComponent<boolean> {
  readonly label = input<string | null>(null);
  readonly changed = output<boolean>();

  value = false;

  writeValue(value: unknown): void {
    this.value = !!value;
  }

  onCheckboxChange(checked: boolean): void {
    this.value = checked;
    this.onChange(checked);
    this.onTouched();
    this.changed.emit(checked);
  }
}
