import { Component, computed, input } from '@angular/core';

@Component({
  selector: 'app-page-header',
  standalone: true,
  templateUrl: './page-header.html',
  styleUrl: './page-header.scss',
})
export class PageHeaderComponent {
  readonly title = input.required<string>();
  readonly description = input<string>();
  readonly subtitle = input<string>();

  readonly activeSubtitle = computed(() => this.description() ?? this.subtitle() ?? null);
}
