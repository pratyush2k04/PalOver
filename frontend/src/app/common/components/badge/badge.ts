import { Component, input, computed } from '@angular/core';

@Component({
  selector: 'app-badge',
  standalone: true,
  templateUrl: './badge.html',
  styleUrl: './badge.scss'
})
export class BadgeComponent {
  readonly text = input.required<string>();

  readonly label = computed(() => this.text().toUpperCase());
}
