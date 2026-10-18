import { Component, input } from '@angular/core';

@Component({
  selector: 'app-loading-spinner',
  standalone: true,
  templateUrl: './loading-spinner.html',
  styleUrl: './loading-spinner.scss'
})
export class LoadingSpinnerComponent {
  readonly label = input<string>('Loading...');
  readonly size = input<'sm' | 'md'>('md');
  readonly inline = input<boolean>(false);
}
