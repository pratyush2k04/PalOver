import { Component, input } from '@angular/core';

@Component({
  selector: 'app-section-header',
  standalone: true,
  templateUrl: './section-header.html',
  styleUrl: './section-header.scss',
})
export class SectionHeaderComponent {
  readonly title = input.required<string>();
  readonly description = input<string | null>(null);
  readonly badge = input<string | null>(null);
}
