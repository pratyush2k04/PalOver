import { Component, inject, input, output, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ClientContactTransitionService, SaveClientContactTransitionRequest } from '../../../services/client-contact-transition.service';
import { AuthService } from 'src/app/services/auth';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { AlertComponent } from '@common/components/alert/alert';
import { ConfirmDialogComponent } from '@common/components/confirm-dialog/confirm-dialog';
import { CLIENT_CONTACT_TRANSITION_MESSAGES } from '@shared/constants/handover.constants';

@Component({
  selector: 'app-client-contact-transition-card',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonComponent, InputComponent, AlertComponent, ConfirmDialogComponent],
  templateUrl: './client-contact-transition-card.component.html',
  styleUrl: './client-contact-transition-card.component.scss'
})
export class ClientContactTransitionCardComponent {
  engagementId = input.required<number>();
  onSaved = output<void>();

  readonly confirmTitle = CLIENT_CONTACT_TRANSITION_MESSAGES.CONFIRM_TITLE;
  readonly confirmLabel = CLIENT_CONTACT_TRANSITION_MESSAGES.CONFIRM_LABEL;
  readonly cancelLabel = CLIENT_CONTACT_TRANSITION_MESSAGES.CANCEL_LABEL;
  readonly errorMessages = {
    required: CLIENT_CONTACT_TRANSITION_MESSAGES.VALIDATION_REQUIRED,
    maxlength: CLIENT_CONTACT_TRANSITION_MESSAGES.VALIDATION_MAXLENGTH
  };

  private transitionService = inject(ClientContactTransitionService);
  private authService = inject(AuthService);
  private fb = inject(FormBuilder);

  isPreSales = computed(() => this.authService.hasRole('Pre Sales'));
  isSaving = signal(false);
  message = signal('');
  isError = signal(false);

  form = this.fb.group({
    outgoingContactName: ['', [Validators.required, Validators.maxLength(200)]],
    outgoingContactRole: ['', [Validators.required, Validators.maxLength(200)]],
    incomingContactName: ['', [Validators.required, Validators.maxLength(200)]],
    incomingContactRole: ['', [Validators.required, Validators.maxLength(200)]]
  });

  pendingRequest = signal<SaveClientContactTransitionRequest | null>(null);
  transition = signal<any>(null);
  isLoadingTransition = signal(false);

  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.loadTransition();
      }
    });
  }

  private loadTransition(): void {
    this.isLoadingTransition.set(true);
    this.transitionService.getTransition(this.engagementId()).subscribe({
      next: (data) => {
        this.transition.set(data);
        this.isLoadingTransition.set(false);
      },
      error: () => {
        this.isLoadingTransition.set(false);
      }
    });
  }

  showConfirm(): void {
    if (!this.form.valid) {
      this.message.set('Please fill in all required fields.');
      this.isError.set(true);
      return;
    }
    this.pendingRequest.set(this.form.value as SaveClientContactTransitionRequest);
  }

  confirmSave(): void {
    const request = this.pendingRequest();
    if (!request) return;

    this.isSaving.set(true);
    this.transitionService.saveTransition(this.engagementId(), request).subscribe({
      next: () => {
        this.form.reset();
        this.pendingRequest.set(null);
        this.message.set(CLIENT_CONTACT_TRANSITION_MESSAGES.SAVE_SUCCESS);
        this.isError.set(false);
        this.isSaving.set(false);
        this.onSaved.emit();
      },
      error: () => {
        this.message.set(CLIENT_CONTACT_TRANSITION_MESSAGES.SAVE_ERROR);
        this.isError.set(true);
        this.isSaving.set(false);
      }
    });
  }

  cancelConfirm(): void {
    this.pendingRequest.set(null);
  }
}
