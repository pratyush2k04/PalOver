import { Component, input } from '@angular/core';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';

@Component({
  selector: 'app-workspace-card',
  standalone: true,
  imports: [EmptyStateComponent],
  templateUrl: './workspace-card.html',
  styleUrl: './workspace-card.scss'
})
export class WorkspaceCardComponent {
  readonly emptyState = input.required<string>();
}
