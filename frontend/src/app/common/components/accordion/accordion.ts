import { Component, input, model } from '@angular/core';
import { InlineSVGModule } from 'ng-inline-svg-2';

@Component({
  selector: 'app-accordion',
  standalone: true,
  imports: [InlineSVGModule],
  templateUrl: './accordion.html',
  styleUrl: './accordion.scss'
})
export class AccordionComponent {
  readonly title = input<string>('');
  readonly icon = input<string>();
  readonly badge = input<string | number>();
  readonly expanded = model<boolean>(true);

  toggle(): void {
    this.expanded.update((v) => !v);
  }
}
