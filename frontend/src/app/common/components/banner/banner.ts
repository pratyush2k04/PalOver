import { Component, input } from '@angular/core';

@Component({
  selector: 'app-banner',
  standalone: true,
  templateUrl: './banner.html',
  styleUrl: './banner.scss'
})
export class BannerComponent {
  readonly message = input.required<string>();
}
