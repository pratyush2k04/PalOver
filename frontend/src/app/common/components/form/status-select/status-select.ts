import {
  Component,
  ElementRef,
  HostListener,
  computed,
  forwardRef,
  inject,
  input,
  model,
  output,
  signal,
  OnDestroy
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { StatusBadgeTone } from '../../status-badge/status-badge';

export interface StatusSelectOption {
  value: unknown;
  label: string;
  tone?: StatusBadgeTone;
  disabled?: boolean;
}

@Component({
  selector: 'app-status-select',
  standalone: true,
  imports: [CommonModule, InlineSVGModule],
  templateUrl: './status-select.html',
  styleUrl: './status-select.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => StatusSelectComponent),
      multi: true
    }
  ]
})
export class StatusSelectComponent implements ControlValueAccessor, OnDestroy {
  private readonly elementRef = inject(ElementRef);

  readonly options = input.required<StatusSelectOption[]>();
  readonly placeholder = input<string>('Select status');
  readonly id = input<string>('');
  readonly disabled = model<boolean>(false);

  // Controlled mode input & output
  readonly value = input<unknown>(undefined);
  readonly statusChange = output<unknown>();

  // CVA mode internal value
  private readonly cvaValue = signal<unknown>(undefined);

  readonly isOpen = signal<boolean>(false);
  
  readonly dropdownTop = signal<number>(0);
  readonly dropdownLeft = signal<number>(0);
  readonly dropdownMinWidth = signal<number>(0);

  private onChange: (value: unknown) => void = () => {};
  private onTouched: () => void = () => {};

  readonly effectiveValue = computed(() => {
    const valInput = this.value();
    if (valInput !== undefined) {
      return valInput;
    }
    return this.cvaValue();
  });

  readonly selectedIndex = computed(() => {
    const opts = this.options();
    const val = this.effectiveValue();
    if (!opts || opts.length === 0 || val === undefined || val === null) {
      return null;
    }
    const idx = opts.findIndex((option) =>
      option.value === val ||
      (option.value != null && val != null && String(option.value) === String(val))
    );
    return idx >= 0 ? idx : null;
  });

  private onScrollFn = (event: Event) => {
    const dropdown = this.elementRef.nativeElement.querySelector('.status-select-dropdown');
    if (dropdown && dropdown.contains(event.target as Node)) {
      return;
    }
    if (this.isOpen()) {
      this.updatePosition();
    }
  };

  private onResizeFn = () => {
    if (this.isOpen()) {
      this.updatePosition();
    }
  };

  readonly selectedOption = computed(() => {
    const opts = this.options();
    const idx = this.selectedIndex();
    if (idx !== null && opts && opts[idx]) {
      return opts[idx];
    }
    return null;
  });

  readonly displayLabel = computed(() => {
    const selected = this.selectedOption();
    if (selected) {
      return selected.label;
    }
    return this.placeholder();
  });

  readonly currentTone = computed<StatusBadgeTone>(() => {
    return this.selectedOption()?.tone || 'neutral';
  });

  ngOnDestroy(): void {
    window.removeEventListener('scroll', this.onScrollFn, true);
    window.removeEventListener('resize', this.onResizeFn);
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (this.isOpen() && !this.elementRef.nativeElement.contains(event.target)) {
      this.closeDropdown();
    }
  }

  @HostListener('keydown.escape')
  onEscape(): void {
    if (this.isOpen()) {
      this.closeDropdown();
    }
  }

  // ControlValueAccessor implementation
  writeValue(value: unknown): void {
    this.cvaValue.set(value);
  }

  registerOnChange(fn: (value: unknown) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled.set(isDisabled);
  }

  private markTouched(): void {
    this.onTouched();
  }

  private closeDropdown(): void {
    this.isOpen.set(false);
    this.markTouched();
    window.removeEventListener('scroll', this.onScrollFn, true);
    window.removeEventListener('resize', this.onResizeFn);
  }

  toggleDropdown(): void {
    if (this.disabled()) {
      return;
    }
    
    if (this.isOpen()) {
      this.closeDropdown();
    } else {
      this.isOpen.set(true);
      this.updatePosition();
      setTimeout(() => this.updatePosition(), 0);
      window.addEventListener('scroll', this.onScrollFn, true);
      window.addEventListener('resize', this.onResizeFn);
    }
  }

  private updatePosition(): void {
    const trigger = this.elementRef.nativeElement.querySelector('.status-select-trigger');
    if (trigger) {
      const rect = trigger.getBoundingClientRect();
      const viewportHeight = window.innerHeight;
      const spaceBelow = viewportHeight - rect.bottom;

      const dropdownEl = this.elementRef.nativeElement.querySelector('.status-select-dropdown') as HTMLElement | null;
      const dropdownHeight = dropdownEl ? dropdownEl.offsetHeight : Math.min(this.options().length * 38 + 12, 256);

      if (rect.bottom < 0 || rect.top > viewportHeight) {
        this.closeDropdown();
        return;
      }

      if (spaceBelow < dropdownHeight && rect.top > spaceBelow) {
        this.dropdownTop.set(Math.max(8, rect.top - dropdownHeight - 6));
      } else {
        this.dropdownTop.set(rect.bottom + 6);
      }

      const dropdownWidth = dropdownEl ? dropdownEl.offsetWidth : 152;
      const maxLeft = Math.max(8, window.innerWidth - dropdownWidth - 8);
      this.dropdownLeft.set(Math.min(Math.max(8, rect.left), maxLeft));
      this.dropdownMinWidth.set(rect.width);
    }
  }

  selectOption(index: number): void {
    if (this.disabled()) {
      return;
    }
    const option = this.options()[index];
    if (!option || option.disabled) {
      return;
    }

    this.closeDropdown();

    if (this.value() === undefined) {
      this.cvaValue.set(option.value);
    }

    this.statusChange.emit(option.value);
    this.onChange(option.value);
  }
}
