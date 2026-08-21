import { ApplicationRef, ComponentRef, EnvironmentInjector, Injectable, InjectionToken, Injector, Type, createComponent } from '@angular/core';
import { DialogRef } from './dialog-ref';

export const DIALOG_DATA = new InjectionToken<unknown>('DIALOG_DATA');

export interface DialogConfig<D> {
  data?: D;
  disableClose?: boolean;
}

@Injectable({ providedIn: 'root' })
export class DialogService {
  constructor(
    private environmentInjector: EnvironmentInjector,
    private applicationRef: ApplicationRef
  ) {}

  open<T, D = unknown, R = unknown>(component: Type<T>, config: DialogConfig<D> = {}): DialogRef<R> {
    const dialogRef = new DialogRef<R>();

    const backdrop = document.createElement('div');
    backdrop.className = 'app-dialog-backdrop';

    const panel = document.createElement('div');
    panel.className = 'app-dialog-panel';
    panel.setAttribute('role', 'dialog');
    panel.setAttribute('aria-modal', 'true');
    backdrop.appendChild(panel);
    document.body.appendChild(backdrop);
    document.body.classList.add('app-dialog-open');

    const injector = Injector.create({
      providers: [
        { provide: DialogRef, useValue: dialogRef },
        { provide: DIALOG_DATA, useValue: config.data },
      ],
      parent: this.environmentInjector,
    });

    const componentRef: ComponentRef<T> = createComponent(component, {
      environmentInjector: this.environmentInjector,
      elementInjector: injector,
      hostElement: panel,
    });

    this.applicationRef.attachView(componentRef.hostView);

    const onBackdropClick = (event: MouseEvent) => {
      if (event.target === backdrop && !config.disableClose) {
        dialogRef.close();
      }
    };
    backdrop.addEventListener('click', onBackdropClick);

    const onKeydown = (event: KeyboardEvent) => {
      if (event.key === 'Escape') {
        dialogRef.close();
      }
    };
    document.addEventListener('keydown', onKeydown);

    const cleanup = () => {
      backdrop.removeEventListener('click', onBackdropClick);
      document.removeEventListener('keydown', onKeydown);
      this.applicationRef.detachView(componentRef.hostView);
      componentRef.destroy();
      backdrop.remove();
      if (!document.querySelector('.app-dialog-backdrop')) {
        document.body.classList.remove('app-dialog-open');
      }
    };

    dialogRef.closed$.subscribe({ complete: cleanup });

    return dialogRef;
  }
}
