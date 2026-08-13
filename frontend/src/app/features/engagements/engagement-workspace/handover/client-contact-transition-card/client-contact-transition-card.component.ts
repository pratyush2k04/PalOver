import { Component, inject, input, output, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonComponent } from '@common/components/button/button';
import { AlertComponent } from '@common/components/alert/alert';

import { ClientContactTransitionService } from '../../../services/client-contact-transition.service';
import { TransitionResponse } from '../../../models/client-contact-transition.model';

import { AuthService } from 'src/app/services/auth';
import { DialogService } from '@common/services/dialog/dialog.service';
import {
  FORM_LABELS,
  BUTTON_LABELS,
  DIALOG_LABELS,
  STATUS_MESSAGES,
} from '@shared/constants/client-contact-transition.constants';
import { AddClientContactTransitionDialogComponent, AddClientContactTransitionDialogData } from '../dialogs/add-client-contact-transition-dialog/add-client-contact-transition-dialog.component';

@Component({
  selector: 'app-client-contact-transition-card',
  standalone: true,
  imports: [
    CommonModule,
    ButtonComponent,
    AlertComponent,
  ],
  templateUrl: './client-contact-transition-card.component.html',
  styleUrl: './client-contact-transition-card.component.scss',
})
export class ClientContactTransitionCardComponent {
  engagementId = input.required<number>();
  clientName = input<string>('');
  onSaved = output<void>();

  private transitionService = inject(ClientContactTransitionService);
  private authService = inject(AuthService);
  private dialog = inject(DialogService);

  formLabels = FORM_LABELS;
  buttonLabels = BUTTON_LABELS;
  dialogLabels = DIALOG_LABELS;
  statusMessages = STATUS_MESSAGES;

  transitions = signal<TransitionResponse[]>([]);
  isLoading = signal(false);
  successMsg = signal('');

  isPreSales = computed(() => this.authService.hasRole('Pre Sales'));

  currentIncomingContact = computed(() => {
    const trans = this.transitions();
    if (trans.length > 0) {
      const latest = trans[trans.length - 1];
      return {
        name: latest.incomingContactName,
        role: latest.incomingContactRole,
      };
    }
    return {
      name: this.clientName(),
      role: '',
    };
  });

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
    const current = this.currentIncomingContact();
    const data: AddClientContactTransitionDialogData = {
      engagementId: this.engagementId(),
      outgoingContactName: current.name,
      outgoingContactRole: current.role,
    };

    const dialogRef = this.dialog.open<AddClientContactTransitionDialogComponent, AddClientContactTransitionDialogData, TransitionResponse>(
      AddClientContactTransitionDialogComponent,
      { data }
    );

    dialogRef.closed$.subscribe((response) => {
      if (!response) return;
      this.successMsg.set(this.statusMessages.SAVE_SUCCESS);
      this.loadHistory();
      this.onSaved.emit();
    });
  }
}
