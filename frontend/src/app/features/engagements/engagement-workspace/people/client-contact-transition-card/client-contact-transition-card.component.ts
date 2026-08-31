import { Component, inject, input, output, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonComponent } from '@common/components/button/button';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';

import { ClientContactTransitionService } from '../../../services/client-contact-transition.service';
import { TransitionResponse, AddClientContactTransitionDialogData } from '../../../models/client-contact-transition.model';

import { AuthService } from '@shared/services/auth';
import { DialogService } from '@common/services/dialog/dialog.service';
import { ToastService } from '@common/services/toast/toast';
import {
  FORM_LABELS,
  BUTTON_LABELS,
  DIALOG_LABELS,
  STATUS_MESSAGES,
} from '@shared/constants/client-contact-transition.constants';
import { AddClientContactTransitionDialogComponent } from '../../handover/dialogs/add-client-contact-transition-dialog/add-client-contact-transition-dialog.component';

@Component({
  selector: 'app-client-contact-transition-card',
  standalone: true,
  imports: [
    CommonModule,
    ButtonComponent,
  ],
  templateUrl: './client-contact-transition-card.component.html',
  styleUrl: './client-contact-transition-card.component.scss',
})
export class ClientContactTransitionCardComponent {
  engagementId = input.required<number>();
  clientName = input<string>('');
  isSubmitted = input<boolean>(false);
  onSaved = output<void>();

  private transitionService = inject(ClientContactTransitionService);
  private authService = inject(AuthService);
  private dialog = inject(DialogService);
  private toastService = inject(ToastService);

  formLabels = FORM_LABELS;
  buttonLabels = BUTTON_LABELS;
  dialogLabels = DIALOG_LABELS;
  statusMessages = STATUS_MESSAGES;

  transitions = signal<TransitionResponse[]>([]);
  isLoading = signal(false);

  isPreSales = computed(() => this.authService.hasRole('Pre Sales'));

  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.loadHistory();
      }
    });
  }

  loadHistory(): void {
    this.isLoading.set(true);
    this.transitionService.getTransitions(this.engagementId()).subscribe({
      next: (data) => {
        this.transitions.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.transitions.set([]);
        this.isLoading.set(false);
      },
    });
  }

  openAddTransitionDialog(): void {
    const data: AddClientContactTransitionDialogData = {
      engagementId: this.engagementId(),
    };

    const dialogRef = this.dialog.open<AddClientContactTransitionDialogComponent, AddClientContactTransitionDialogData, TransitionResponse>(
      AddClientContactTransitionDialogComponent,
      { data }
    );

    dialogRef.closed$.subscribe((response) => {
      if (!response) return;
      this.toastService.success(this.statusMessages.SAVE_SUCCESS);
      this.loadHistory();
      this.onSaved.emit();
    });
  }

  getInitials(name: string): string {
    if (!name) return '??';
    const parts = name.trim().split(/\s+/);
    if (parts.length >= 2) {
      return (parts[0][0] + parts[1][0]).toUpperCase();
    }
    return name.slice(0, 2).toUpperCase();
  }
}
