import { Component, computed, input } from '@angular/core';
import { BaseControlComponent } from '../base-control';

export type InputType = 'text' | 'email' | 'password' | 'date';

@Component({
  selector: 'app-input',
  standalone: true,
  templateUrl: './input.html',
  styleUrl: './input.scss'
})
export class InputComponent extends BaseControlComponent<string> {
  readonly id = input.required<string>();
  readonly label = input<string | null>(null);
  readonly type = input<InputType>('text');
  readonly placeholder = input<string>('');
  readonly maxLength = input<number | null>(null);
  readonly dangerThreshold = input<number>(50);

  value = '';

  writeValue(value: unknown): void {
    this.value = (value as string) ?? '';
  }

  onInput(value: string): void {
    this.value = value;
    this.onChange(value);
  }

  readonly showCounter = computed(() => this.maxLength() !== null);

  readonly counterDanger = computed(() => {
    const max = this.maxLength();
    return max !== null && this.value.length > max - this.dangerThreshold();
  });
}
