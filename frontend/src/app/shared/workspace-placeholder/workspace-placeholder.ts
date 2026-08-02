import { Component, input } from '@angular/core';

@Component({
  selector: 'app-workspace-placeholder',
  standalone: true,
  templateUrl: './workspace-placeholder.html',
  styleUrl: './workspace-placeholder.scss'
})
export class WorkspacePlaceholderComponent {
  readonly emptyState = input.required<string>();
}
