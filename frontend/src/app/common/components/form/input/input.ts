import { Component, computed, input, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { BaseControlComponent } from '../base-control';

export type InputType = 'text' | 'email' | 'password' | 'date' | 'number';

@Component({
  selector: 'app-input',
  standalone: true,
  imports: [CommonModule, InlineSVGModule],
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
  readonly min = input<number | string | null>(null);
  readonly step = input<number | string | null>(null);

  readonly isPasswordVisible = signal<boolean>(false);

  value = '';

  readonly currentType = computed(() => {
    if (this.type() === 'password') {
      return this.isPasswordVisible() ? 'text' : 'password';
    }
    return this.type();
  });

  togglePasswordVisibility(): void {
    this.isPasswordVisible.update((v) => !v);
  }

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
