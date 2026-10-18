import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { GovernanceProcessComponent } from './components/governance-process/governance-process.component';
import { ChangeRequestsComponent } from './components/change-requests/change-requests.component';
import { MeetingRecordComponent } from './components/meeting-record/meeting-record.component';
import { AuthService } from '@shared/services/auth';
import { DialogService } from '@common/services/dialog/dialog.service';
import { EngagementService } from '../../services/engagement.service';
import { KickoffReadinessService } from '@shared/services/kickoff-readiness.service';
import {
  OverrideKickoffReadyDialogComponent,
  OverrideKickoffReadyDialogData,
  OverrideKickoffReadyDialogResult
} from './dialogs/override-kickoff-ready-dialog/override-kickoff-ready-dialog.component';
import { KICKOFF_MESSAGES } from '@shared/constants/kickoff.constants';
import { USER_ROLES } from '@shared/constants/users.constants';

@Component({
  selector: 'app-kickoff',
  standalone: true,
  imports: [CommonModule, GovernanceProcessComponent, ChangeRequestsComponent, MeetingRecordComponent],
  templateUrl: './kickoff.component.html',
  styleUrl: './kickoff.component.scss'
})
export class KickoffComponent {
  private route = inject(ActivatedRoute);
  private authService = inject(AuthService);
  private dialog = inject(DialogService);
  private engagementService = inject(EngagementService);
  private kickoffReadinessService = inject(KickoffReadinessService);

  private parentParamMap = toSignal(this.route.parent!.paramMap);

  engagementId = computed(() => {
    const idStr = this.parentParamMap()?.get('id');
    return idStr ? +idStr : null;
  });

  readiness = this.kickoffReadinessService.readiness;
  isLoading = this.kickoffReadinessService.isLoading;
  engagementName = signal('');
  isSaving = signal(false);
  message = signal('');
  isError = signal(false);

  isLeadership = computed(() => this.engagementService.isWorkspaceLeadership() || this.authService.isAdmin());
  canMarkReady = computed(() =>
    this.engagementService.isWorkspacePm() ||
    this.engagementService.isWorkspaceTechLead() ||
    this.isLeadership());

  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.kickoffReadinessService.track(id);
        this.engagementService.getEngagementInfo(id).subscribe({
          next: (data) => this.engagementName.set(data.engagementName),
          error: () => this.engagementName.set('')
        });
      }
    });
  }

  markReady() {
    const id = this.engagementId();
    if (!id) return;

    this.isSaving.set(true);
    this.kickoffReadinessService.markKickoffReady(id).subscribe({
      next: () => {
        this.showMessage(KICKOFF_MESSAGES.MARKED_READY_SUCCESS, false);
        this.isSaving.set(false);
        this.kickoffReadinessService.refresh();
      },
      error: (err) => {
        this.showMessage(this.extractErrorMessage(err, KICKOFF_MESSAGES.MARK_READY_ERROR), true);
        this.isSaving.set(false);
      }
    });
  }

  openOverrideDialog() {
    const id = this.engagementId();
    if (!id) return;

    const dialogRef = this.dialog.open<
      OverrideKickoffReadyDialogComponent,
      OverrideKickoffReadyDialogData,
      OverrideKickoffReadyDialogResult
    >(OverrideKickoffReadyDialogComponent, {
      data: {
        engagementName: this.engagementName(),
        score: this.readiness()?.score ?? 0
      }
    });

    dialogRef.closed$.subscribe((result) => {
      if (result) {
        this.markReady();
      }
    });
  }

  private extractErrorMessage(err: unknown, fallback: string): string {
    const errorMessage = (err as { error?: { message?: string } })?.error?.message;
    return errorMessage || fallback;
  }

  private showMessage(msg: string, isErr: boolean) {
    this.message.set(msg);
    this.isError.set(isErr);
    setTimeout(() => { this.message.set(''); }, 5000);
  }
}
