import {
  Component,
  ElementRef,
  HostListener,
  computed,
  effect,
  inject,
  input,
  signal,
  OnDestroy
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { BaseControlComponent } from '../base-control';

export interface SelectOption {
  value: unknown;
  label: string;
}

@Component({
  selector: 'app-select',
  standalone: true,
  imports: [CommonModule, InlineSVGModule],
  templateUrl: './select.html',
  styleUrl: './select.scss'
})
export class SelectComponent extends BaseControlComponent<unknown> implements OnDestroy {
  private readonly elementRef = inject(ElementRef);

  readonly id = input.required<string>();
  readonly label = input<string | null>(null);
  readonly options = input.required<SelectOption[]>();
  readonly placeholder = input<string | null>(null);

  readonly isOpen = signal<boolean>(false);
  value: unknown = null;
  readonly selectedIndex = signal<number | null>(null);

  dropdownTop = 0;
  dropdownLeft = 0;
  dropdownMinWidth = 0;

  private onScrollFn = () => {
    if (this.isOpen()) {
      this.isOpen.set(false);
      this.markTouched();
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
    return this.placeholder() || 'Select an option';
  });

  readonly isPlaceholder = computed(() => {
    return this.selectedOption() === null;
  });

  constructor() {
    super();
    effect(() => {
      // Re-sync selectedIndex whenever options() changes
      this.options();
      // Use untracked to prevent circular effect loops when modifying signals
      this.syncSelectedIndex();
    });
  }

  ngOnDestroy(): void {
    window.removeEventListener('scroll', this.onScrollFn, true);
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (this.isOpen() && !this.elementRef.nativeElement.contains(event.target)) {
      this.isOpen.set(false);
      this.markTouched();
      window.removeEventListener('scroll', this.onScrollFn, true);
    }
  }

  @HostListener('keydown.escape')
  onEscape(): void {
    if (this.isOpen()) {
      this.isOpen.set(false);
      this.markTouched();
      window.removeEventListener('scroll', this.onScrollFn, true);
    }
  }

  override writeValue(value: unknown): void {
    this.value = value;
    this.syncSelectedIndex();
  }

  private syncSelectedIndex(): void {
    const opts = this.options();
    if (!opts || opts.length === 0) {
      this.selectedIndex.set(null);
      return;
    }

    const val = this.value;
    const idx = opts.findIndex((option) =>
      option.value === val ||
      (option.value != null && val != null && String(option.value) === String(val))
    );

    this.selectedIndex.set(idx >= 0 ? idx : null);
  }

  toggleDropdown(): void {
    if (this.disabled()) {
      return;
    }
    
    if (this.isOpen()) {
      this.isOpen.set(false);
      window.removeEventListener('scroll', this.onScrollFn, true);
      this.markTouched();
    } else {
      this.isOpen.set(true);
      this.updatePosition();
      window.addEventListener('scroll', this.onScrollFn, true);
    }
  }

  private updatePosition(): void {
    const trigger = this.elementRef.nativeElement.querySelector('.select-trigger');
    if (trigger) {
      const rect = trigger.getBoundingClientRect();
      this.dropdownTop = rect.bottom + 4;
      this.dropdownLeft = rect.left;
      this.dropdownMinWidth = rect.width;
    }
  }

  selectOption(index: number): void {
    if (this.disabled()) {
      return;
    }
    this.selectedIndex.set(index);
    const option = this.options()[index];
    this.value = option ? option.value : null;
    this.onChange(this.value);
    this.markTouched();
    this.isOpen.set(false);
  }
}
