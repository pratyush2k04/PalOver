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
import { StatusBadgeTone } from '../../status-badge/status-badge';

export interface StatusSelectOption {
  value: unknown;
  label: string;
  tone?: StatusBadgeTone;
}

@Component({
  selector: 'app-status-select',
  standalone: true,
  imports: [CommonModule, InlineSVGModule],
  templateUrl: './status-select.html',
  styleUrl: './status-select.scss'
})
export class StatusSelectComponent extends BaseControlComponent<unknown> implements OnDestroy {
  private readonly elementRef = inject(ElementRef);

  readonly options = input.required<StatusSelectOption[]>();
  readonly placeholder = input<string>('Select status');
  readonly id = input<string>('');

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
    return this.placeholder();
  });

  readonly currentTone = computed<StatusBadgeTone>(() => {
    return this.selectedOption()?.tone || 'neutral';
  });

  constructor() {
    super();
    effect(() => {
      this.options();
      this.syncSelectedIndex();
    }, { allowSignalWrites: true });
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
    const trigger = this.elementRef.nativeElement.querySelector('.status-select-trigger');
    if (trigger) {
      const rect = trigger.getBoundingClientRect();
      this.dropdownTop = rect.bottom + 6;
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
