import { Component, input } from '@angular/core';
import { InlineSVGModule } from 'ng-inline-svg-2';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  imports: [InlineSVGModule],
  templateUrl: './empty-state.html',
  styleUrl: './empty-state.scss'
})
export class EmptyStateComponent {
  readonly message = input.required<string>();
  readonly icon = input<string | null>(null);
}
