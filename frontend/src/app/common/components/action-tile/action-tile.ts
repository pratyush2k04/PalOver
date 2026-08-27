import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { InlineSVGModule } from 'ng-inline-svg-2';

export type ActionTileTone = 'info' | 'success' | 'warning' | 'danger' | 'neutral';

@Component({
  selector: 'app-action-tile',
  standalone: true,
  imports: [RouterLink, InlineSVGModule],
  templateUrl: './action-tile.html',
  styleUrl: './action-tile.scss'
})
export class ActionTileComponent {
  readonly icon = input.required<string>();
  readonly label = input.required<string>();
  readonly tone = input<ActionTileTone>('neutral');
  readonly routerLink = input.required<string | any[]>();
  readonly queryParams = input<Record<string, string> | null>(null);
}
