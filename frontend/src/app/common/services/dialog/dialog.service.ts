import { ApplicationRef, ComponentRef, EnvironmentInjector, Injectable, InjectionToken, Injector, Type, afterNextRender, createComponent } from '@angular/core';
import { DialogRef } from './dialog-ref';

export const DIALOG_DATA = new InjectionToken<unknown>('DIALOG_DATA');

export interface DialogConfig<D> {
  data?: D;
  disableClose?: boolean;
}

// Native form elements rendered by the common/ form controls, matched in DOM order.
// Disabled and readonly fields are skipped so the caret lands somewhere the user can
// actually type (e.g. read-only evidence, already-confirmed allocation slots).
const FOCUSABLE_FIELD_SELECTOR = [
  'input:not([type="hidden"]):not([disabled]):not([readonly])',
  'textarea:not([disabled]):not([readonly])',
  'select:not([disabled])',
].join(', ');

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

    afterNextRender(() => this.focusFirstField(panel), { injector: this.environmentInjector });

    const disableClose = config.disableClose ?? true;

    const onBackdropClick = (event: MouseEvent) => {
      if (event.target === backdrop && !disableClose) {
        dialogRef.close();
      }
    };
    backdrop.addEventListener('click', onBackdropClick);

    const onKeydown = (event: KeyboardEvent) => {
      if (event.key === 'Escape' ) {
        dialogRef.close();
      }
    };
    document.addEventListener('keydown', onKeydown);

    const onPanelKeydown = (event: KeyboardEvent) => {
      if (event.key !== 'Enter' || event.shiftKey || event.ctrlKey || event.altKey || event.metaKey) {
        return;
      }

      // isComposing guards IME candidate selection; defaultPrevented respects any
      // component that already handled Enter itself.
      if (event.isComposing || event.defaultPrevented) {
        return;
      }

      const target = event.target as HTMLElement | null;

      // Buttons and links already activate on Enter natively - never hijack them,
      // otherwise Enter on a focused Cancel button would submit instead.
      if (target?.closest('button, a, [role="button"]')) {
        return;
      }

      const form = target?.closest('form') ?? panel.querySelector('form');
      const submitButton = form?.querySelector<HTMLButtonElement>('button[type="submit"]:not([disabled])');

      // No enabled submit action means there is nothing Enter should do.
      if (!form || !submitButton) {
        return;
      }

      event.preventDefault();
      form.requestSubmit(submitButton);
    };
    panel.addEventListener('keydown', onPanelKeydown);

    const cleanup = () => {
      backdrop.removeEventListener('click', onBackdropClick);
      document.removeEventListener('keydown', onKeydown);
      panel.removeEventListener('keydown', onPanelKeydown);
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

  private focusFirstField(panel: HTMLElement): void {
    const field = panel.querySelector<HTMLElement>(FOCUSABLE_FIELD_SELECTOR);

    if (field) {
      field.focus();
      return;
    }

    // No typable field (e.g. a confirm-only dialog): move focus into the panel anyway
    // so it does not stay stranded on the trigger button behind the modal.
    panel.tabIndex = -1;
    panel.focus();
  }
}
