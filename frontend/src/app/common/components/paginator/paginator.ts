import { Component, computed, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';

@Component({
  selector: 'app-paginator',
  standalone: true,
  imports: [FormsModule, SelectComponent],
  templateUrl: './paginator.html',
  styleUrl: './paginator.scss'
})
export class PaginatorComponent {
  readonly page = input.required<number>();
  readonly totalPages = input.required<number>();
  readonly pageSize = input<number | null>(null);
  readonly pageSizeOptions = input<number[] | null>(null);
  readonly showFirstLast = input(false);

  readonly pageChange = output<number>();
  readonly pageSizeChange = output<number>();

  readonly selectOptions = computed<SelectOption[]>(() => {
    const sizes = this.pageSizeOptions() || [];
    return sizes.map((size) => ({
      value: size,
      label: String(size),
    }));
  });

  goToFirst(): void {
    this.pageChange.emit(1);
  }

  goToPrevious(): void {
    this.pageChange.emit(Math.max(1, this.page() - 1));
  }

  goToNext(): void {
    this.pageChange.emit(Math.min(this.totalPages(), this.page() + 1));
  }

  goToLast(): void {
    this.pageChange.emit(this.totalPages());
  }

  onPageSizeChange(value: unknown): void {
    if (value !== null && value !== undefined) {
      this.pageSizeChange.emit(Number(value));
    }
  }
}
