import {
  ChangeDetectionStrategy,
  Component,
  inject
} from '@angular/core';

import { LoaderService } from '@shared/services/loader.service';

@Component({
  selector: 'app-global-loader',
  standalone: true,
  templateUrl: './global-loader.html',
  styleUrl: './global-loader.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class GlobalLoader {
  readonly loader = inject(LoaderService);
}

