import { Component, input, computed } from '@angular/core';

@Component({
  selector: 'app-role-badge',
  standalone: true,
  templateUrl: './role-badge.html',
  styleUrl: './role-badge.scss'
})
export class RoleBadgeComponent {
  readonly role = input.required<string>();

  readonly label = computed(() => this.role().toUpperCase());
}
