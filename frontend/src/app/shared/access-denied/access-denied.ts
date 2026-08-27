import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { AlertComponent } from '@common/components/alert/alert';
import { ButtonComponent } from '@common/components/button/button';
import { AppContextService } from '@shared/services/app-context';

@Component({
  selector: 'app-access-denied',
  standalone: true,
  imports: [AlertComponent, ButtonComponent, InlineSVGModule],
  templateUrl: './access-denied.html',
  styleUrl: './access-denied.scss',
})
export class AccessDeniedComponent {
  private readonly router = inject(Router);
  private readonly appContext = inject(AppContextService);

  goHome(): void {
    const homeRoute = this.appContext.workspace()?.route;
    if (homeRoute) {
      this.router.navigateByUrl(homeRoute);
    }
  }
}
