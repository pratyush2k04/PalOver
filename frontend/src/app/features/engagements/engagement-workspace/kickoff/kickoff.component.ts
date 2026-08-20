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
import { KickoffReadinessDto, KickoffReadinessService } from '@shared/services/kickoff-readiness.service';
import {
  OverrideKickoffReadyDialogComponent,
  OverrideKickoffReadyDialogData,
  OverrideKickoffReadyDialogResult
} from './dialogs/override-kickoff-ready-dialog/override-kickoff-ready-dialog.component';
import { ButtonComponent } from '@common/components/button/button';
import { KICKOFF_MESSAGES } from '@shared/constants/kickoff.constants';

@Component({
  selector: 'app-kickoff',
  standalone: true,
  imports: [CommonModule, GovernanceProcessComponent, ChangeRequestsComponent, MeetingRecordComponent, ButtonComponent],
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

  readiness = signal<KickoffReadinessDto | null>(null);
  engagementName = signal('');
  isLoading = signal(true);
  isSaving = signal(false);
  loadFailed = signal(false);
  message = signal('');
  isError = signal(false);

  isLeadership = computed(() => this.authService.hasRole('Leadership'));
  canMarkReady = computed(() =>
    this.authService.hasRole('Project Management') || this.authService.hasRole('Tech Lead') || this.isLeadership());

  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.loadReadiness(id);
        this.engagementService.getEngagementInfo(id).subscribe({
          next: (data) => this.engagementName.set(data.engagementName),
          error: () => this.engagementName.set('')
        });
      }
    });
  }

  private loadReadiness(id: number) {
    this.isLoading.set(true);
    this.loadFailed.set(false);
    this.kickoffReadinessService.getReadiness(id).subscribe({
      next: (data) => {
        this.readiness.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Failed to load kickoff readiness', err);
        this.readiness.set(null);
        this.loadFailed.set(true);
        this.showMessage(this.extractErrorMessage(err, KICKOFF_MESSAGES.READINESS_LOAD_ERROR), true);
        this.isLoading.set(false);
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
        this.loadReadiness(id);
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
