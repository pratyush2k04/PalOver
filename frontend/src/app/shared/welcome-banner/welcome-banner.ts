import { Component, input } from '@angular/core';

@Component({
  selector: 'app-welcome-banner',
  standalone: true,
  templateUrl: './welcome-banner.html',
  styleUrl: './welcome-banner.scss'
})
export class WelcomeBannerComponent {
  readonly welcomeMessage = input.required<string>();
}
