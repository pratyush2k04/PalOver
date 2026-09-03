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

  private onScrollFn = (event: Event) => {
    const dropdown = this.elementRef.nativeElement.querySelector('.select-dropdown');
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
    const trigger = this.elementRef.nativeElement.querySelector('.select-trigger');
    if (trigger) {
      const rect = trigger.getBoundingClientRect();
      const viewportHeight = window.innerHeight;
      const spaceBelow = viewportHeight - rect.bottom;

      const dropdownEl = this.elementRef.nativeElement.querySelector('.select-dropdown') as HTMLElement | null;
      const dropdownHeight = dropdownEl ? dropdownEl.offsetHeight : Math.min(this.options().length * 38 + 12, 256);

      if (spaceBelow < dropdownHeight && rect.top > dropdownHeight) {
        this.dropdownTop = Math.max(8, rect.top - dropdownHeight - 4);
      } else {
        this.dropdownTop = rect.bottom + 4;
      }
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
    this.closeDropdown();
  }
}
