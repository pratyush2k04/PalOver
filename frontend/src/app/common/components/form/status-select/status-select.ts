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

  constructor() {
    super();
    effect(() => {
      this.options();
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
    const trigger = this.elementRef.nativeElement.querySelector('.status-select-trigger');
    if (trigger) {
      const rect = trigger.getBoundingClientRect();
      const viewportHeight = window.innerHeight;
      const spaceBelow = viewportHeight - rect.bottom;

      const dropdownEl = this.elementRef.nativeElement.querySelector('.status-select-dropdown') as HTMLElement | null;
      const dropdownHeight = dropdownEl ? dropdownEl.offsetHeight : Math.min(this.options().length * 38 + 12, 256);

      if (spaceBelow < dropdownHeight && rect.top > spaceBelow) {
        this.dropdownTop = Math.max(8, rect.top - dropdownHeight - 6);
      } else {
        this.dropdownTop = rect.bottom + 6;
      }

      const dropdownWidth = dropdownEl ? dropdownEl.offsetWidth : 152;
      const maxLeft = Math.max(8, window.innerWidth - dropdownWidth - 8);
      this.dropdownLeft = Math.min(Math.max(8, rect.left), maxLeft);
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
