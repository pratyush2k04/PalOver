import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-paginator',
  standalone: true,
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

  onPageSizeChange(value: string): void {
    this.pageSizeChange.emit(Number(value));
  }
}
