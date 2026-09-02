import {
  Component,
  ElementRef,
  HostListener,
  OnDestroy,
  computed,
  inject,
  input,
  signal
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { BaseControlComponent } from '../base-control';

export interface CalendarDay {
  date: Date;
  isoString: string;
  dayNumber: number;
  isCurrentMonth: boolean;
  isToday: boolean;
  isSelected: boolean;
  isDisabled: boolean;
}

const MONTH_NAMES = [
  'January',
  'February',
  'March',
  'April',
  'May',
  'June',
  'July',
  'August',
  'September',
  'October',
  'November',
  'December'
];

const SHORT_MONTH_NAMES = [
  'Jan',
  'Feb',
  'Mar',
  'Apr',
  'May',
  'Jun',
  'Jul',
  'Aug',
  'Sep',
  'Oct',
  'Nov',
  'Dec'
];

const WEEKDAY_NAMES = ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'];

function formatDateToIso(date: Date): string {
  const y = date.getFullYear();
  const m = String(date.getMonth() + 1).padStart(2, '0');
  const d = String(date.getDate()).padStart(2, '0');
  return `${y}-${m}-${d}`;
}

function parseIsoToDate(str: string | null | undefined): Date | null {
  if (!str) return null;
  const parts = str.split('T')[0].split('-');
  if (parts.length !== 3) return null;
  const y = parseInt(parts[0], 10);
  const m = parseInt(parts[1], 10) - 1;
  const d = parseInt(parts[2], 10);
  const date = new Date(y, m, d);
  return isNaN(date.getTime()) ? null : date;
}

@Component({
  selector: 'app-datepicker',
  standalone: true,
  imports: [CommonModule, InlineSVGModule],
  templateUrl: './datepicker.html',
  styleUrl: './datepicker.scss'
})
export class DatepickerComponent extends BaseControlComponent<string> implements OnDestroy {
  private readonly elementRef = inject(ElementRef);

  readonly id = input.required<string>();
  readonly label = input<string | null>(null);
  readonly placeholder = input<string>('Select date');
  readonly min = input<string | null>(null);
  readonly max = input<string | null>(null);

  readonly isOpen = signal<boolean>(false);
  value = '';

  // Track the month and year currently in view
  readonly viewYear = signal<number>(new Date().getFullYear());
  readonly viewMonth = signal<number>(new Date().getMonth()); // 0 - 11

  readonly weekdays = WEEKDAY_NAMES;

  dropdownTop = 0;
  dropdownLeft = 0;
  dropdownMinWidth = 0;

  private onScrollFn = (event: Event) => {
    const dropdown = this.elementRef.nativeElement.querySelector('.datepicker-dropdown');
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

  readonly currentMonthLabel = computed(() => {
    return `${MONTH_NAMES[this.viewMonth()]} ${this.viewYear()}`;
  });

  readonly displayValue = computed(() => {
    if (!this.value) {
      return '';
    }
    const parsed = parseIsoToDate(this.value);
    if (!parsed) {
      return this.value;
    }
    const day = String(parsed.getDate()).padStart(2, '0');
    const month = SHORT_MONTH_NAMES[parsed.getMonth()];
    const year = parsed.getFullYear();
    return `${day} ${month} ${year}`;
  });

  readonly isPlaceholder = computed(() => !this.value);

  readonly calendarDays = computed<CalendarDay[]>(() => {
    const year = this.viewYear();
    const month = this.viewMonth();
    const minStr = this.min();
    const maxStr = this.max();
    const selectedIso = this.value;

    const todayIso = formatDateToIso(new Date());

    const firstDayOfMonth = new Date(year, month, 1);
    const startDayOfWeek = firstDayOfMonth.getDay(); // 0 = Sun, 1 = Mon ...
    const daysInCurrentMonth = new Date(year, month + 1, 0).getDate();
    const daysInPrevMonth = new Date(year, month, 0).getDate();

    const days: CalendarDay[] = [];

    // 1. Trailing days from previous month
    for (let i = startDayOfWeek - 1; i >= 0; i--) {
      const dayNumber = daysInPrevMonth - i;
      const prevDate = new Date(year, month - 1, dayNumber);
      const iso = formatDateToIso(prevDate);
      days.push({
        date: prevDate,
        isoString: iso,
        dayNumber,
        isCurrentMonth: false,
        isToday: iso === todayIso,
        isSelected: iso === selectedIso,
        isDisabled: this.isDateDisabled(iso, minStr, maxStr)
      });
    }

    // 2. Current month days
    for (let i = 1; i <= daysInCurrentMonth; i++) {
      const currDate = new Date(year, month, i);
      const iso = formatDateToIso(currDate);
      days.push({
        date: currDate,
        isoString: iso,
        dayNumber: i,
        isCurrentMonth: true,
        isToday: iso === todayIso,
        isSelected: iso === selectedIso,
        isDisabled: this.isDateDisabled(iso, minStr, maxStr)
      });
    }

    // 3. Leading days for next month to complete 42 cells (6 rows * 7 columns)
    const remainingDays = 42 - days.length;
    for (let i = 1; i <= remainingDays; i++) {
      const nextDate = new Date(year, month + 1, i);
      const iso = formatDateToIso(nextDate);
      days.push({
        date: nextDate,
        isoString: iso,
        dayNumber: i,
        isCurrentMonth: false,
        isToday: iso === todayIso,
        isSelected: iso === selectedIso,
        isDisabled: this.isDateDisabled(iso, minStr, maxStr)
      });
    }

    return days;
  });

  private isDateDisabled(
    iso: string,
    minStr: string | null | undefined,
    maxStr: string | null | undefined
  ): boolean {
    if (minStr && iso < minStr) {
      return true;
    }
    if (maxStr && iso > maxStr) {
      return true;
    }
    return false;
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
    if (typeof value === 'string' && value) {
      const clean = value.split('T')[0];
      this.value = clean;
      const parsed = parseIsoToDate(clean);
      if (parsed) {
        this.viewYear.set(parsed.getFullYear());
        this.viewMonth.set(parsed.getMonth());
      }
    } else {
      this.value = '';
    }
  }

  toggleDropdown(): void {
    if (this.disabled()) {
      return;
    }

    if (this.isOpen()) {
      this.closeDropdown();
    } else {
      // If there's an existing valid date, sync view to it; otherwise sync to today
      const current = parseIsoToDate(this.value) || new Date();
      this.viewYear.set(current.getFullYear());
      this.viewMonth.set(current.getMonth());

      this.isOpen.set(true);
      this.updatePosition();
      setTimeout(() => this.updatePosition(), 0);
      window.addEventListener('scroll', this.onScrollFn, true);
      window.addEventListener('resize', this.onResizeFn);
    }
  }

  closeDropdown(): void {
    this.isOpen.set(false);
    this.markTouched();
    window.removeEventListener('scroll', this.onScrollFn, true);
    window.removeEventListener('resize', this.onResizeFn);
  }

  prevMonth(): void {
    const currentMonth = this.viewMonth();
    if (currentMonth === 0) {
      this.viewMonth.set(11);
      this.viewYear.update((y) => y - 1);
    } else {
      this.viewMonth.set(currentMonth - 1);
    }
  }

  nextMonth(): void {
    const currentMonth = this.viewMonth();
    if (currentMonth === 11) {
      this.viewMonth.set(0);
      this.viewYear.update((y) => y + 1);
    } else {
      this.viewMonth.set(currentMonth + 1);
    }
  }

  prevYear(): void {
    this.viewYear.update((y) => y - 1);
  }

  nextYear(): void {
    this.viewYear.update((y) => y + 1);
  }

  selectDay(day: CalendarDay): void {
    if (day.isDisabled || this.disabled()) {
      return;
    }

    this.value = day.isoString;
    this.onChange(this.value);

    // If day is from prev or next month, also transition view
    if (!day.isCurrentMonth) {
      this.viewYear.set(day.date.getFullYear());
      this.viewMonth.set(day.date.getMonth());
    }

    this.closeDropdown();
  }

  selectToday(): void {
    const today = new Date();
    const iso = formatDateToIso(today);
    if (this.isDateDisabled(iso, this.min(), this.max())) {
      // Jump view without selecting if disabled
      this.viewYear.set(today.getFullYear());
      this.viewMonth.set(today.getMonth());
      return;
    }

    this.value = iso;
    this.viewYear.set(today.getFullYear());
    this.viewMonth.set(today.getMonth());
    this.onChange(this.value);
    this.closeDropdown();
  }

  clearDate(): void {
    this.value = '';
    this.onChange(this.value);
    this.closeDropdown();
  }

  private updatePosition(): void {
    const trigger = this.elementRef.nativeElement.querySelector('.datepicker-trigger');
    if (trigger) {
      const rect = trigger.getBoundingClientRect();
      const viewportHeight = window.innerHeight;
      const spaceBelow = viewportHeight - rect.bottom;

      const dropdownEl = this.elementRef.nativeElement.querySelector(
        '.datepicker-dropdown'
      ) as HTMLElement | null;
      const dropdownHeight = dropdownEl ? dropdownEl.offsetHeight : 340;

      if (spaceBelow < dropdownHeight && rect.top > dropdownHeight) {
        this.dropdownTop = Math.max(8, rect.top - dropdownHeight - 4);
      } else {
        this.dropdownTop = rect.bottom + 4;
      }
      this.dropdownLeft = rect.left;
      this.dropdownMinWidth = Math.max(rect.width, 280);
    }
  }
}
