import { Component, ViewChild, inject, signal, computed, effect, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  NgModel,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { HandoverService } from '../../services/handover.service';
import { EngagementService } from '../../services/engagement.service';
import { catchError, of } from 'rxjs';
import { AuthService } from '@shared/services/auth';
import { KickoffReadinessService } from '@shared/services/kickoff-readiness.service';
import { EngagementInfoDto } from '../../models/engagement.model';
import { DialogService } from '@common/services/dialog/dialog.service';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { AddCommitmentDialogComponent } from './dialogs/add-commitment-dialog/add-commitment-dialog.component';
import {
  HANDOVER_MESSAGES,
  HANDOVER_SENTIMENT_OPTIONS,
  HANDOVER_TIMELINE_UNITS,
  HANDOVER_LABELS,
  HANDOVER_PLACEHOLDERS,
  HANDOVER_BUTTONS,
  MAX_FREE_TEXT_LENGTH,
} from '@shared/constants/handover.constants';
import {
  HandoverInfoDto,
  SaveHandoverDraftRequest,
  CreateCommitmentRequest,
  UpdateCommitmentRequest,
  CreateRealisticCommitmentRequest,
  UpdateRealisticCommitmentRequest,
  CommitmentDto,
  AddCommitmentDialogData
} from '../../models/handover.model';
import { HandoverReadinessItemDto } from '../../models/handover-readiness.model';
import { AlertComponent } from '@common/components/alert/alert';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { StatusBadgeComponent, StatusBadgeTone } from '@common/components/status-badge/status-badge';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import { SectionHeaderComponent } from '@common/components/section-header/section-header';
import { HandoverQueryService } from '../../services/handover-query.service';
import { HandoverQueryDto } from '../../models/handover-query.model';
import { HandoverQueriesComponent } from './handover-queries/handover-queries.component';
import { ToastService } from '@common/services/toast/toast';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';

import { ConfirmDialogComponent } from '@common/components/confirm-dialog/confirm-dialog';
import { HandoverReadinessComponent } from './handover-readiness/handover-readiness.component';

function timelinesMatch(timeline1: number, unit1: string, timeline2: number, unit2: string): boolean {
  return timeline1 === timeline2 && unit1 === unit2;
}

interface RealisticEditForm {
  description: string;
  timeline: number;
  timelineUnit: string;
  acknowledgementMessage: string;
}

@Component({
  selector: 'app-handover',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    SectionHeaderComponent,
    AlertComponent,
    ButtonComponent,
    InputComponent,
    TextareaComponent,
    SelectComponent,
    StatusBadgeComponent,
    EmptyStateComponent,
    HandoverQueriesComponent,
    ConfirmDialogComponent,
    HandoverReadinessComponent,
    InlineSVGModule,
  ],
  templateUrl: './handover.component.html',
  styleUrl: './handover.component.scss',
})
export class HandoverComponent {
  private static readonly KICKOFF_NOT_READY_MESSAGE = HANDOVER_MESSAGES.KICKOFF_NOT_READY;

  HANDOVER_LABELS = HANDOVER_LABELS;
  HANDOVER_PLACEHOLDERS = HANDOVER_PLACEHOLDERS;
  HANDOVER_BUTTONS = HANDOVER_BUTTONS;
  HANDOVER_MESSAGES = HANDOVER_MESSAGES;

  engagementInfo = signal<EngagementInfoDto | null>(null);

  private route = inject(ActivatedRoute);
  private handoverService = inject(HandoverService);
  private handoverQueryService = inject(HandoverQueryService);
  private engagementService = inject(EngagementService);
  private kickoffReadinessService = inject(KickoffReadinessService);
  private authService = inject(AuthService);
  private toastService = inject(ToastService);
  private dialog = inject(DialogService);
  private fb = inject(FormBuilder);

  private parentParamMap = toSignal(this.route.parent!.paramMap);

  engagementId = computed(() => {
    const idStr = this.parentParamMap()?.get('id');
    return idStr ? +idStr : null;
  });

  handover = signal<HandoverInfoDto | null>(null);
  queries = signal<HandoverQueryDto[]>([]);
  isLoading = signal(true);
  isSaving = signal(false);
  message = signal('');
  isError = signal(false);
  handoverReadinessItems = signal<HandoverReadinessItemDto[]>([]);

  hasUnresolvedQueries = computed(() => this.queries().some((q) => !q.isResolved));

  kickoffReadiness = this.kickoffReadinessService.readiness;

  onHandoverReadinessItemsUpdated(items: HandoverReadinessItemDto[]): void {
    this.handoverReadinessItems.set(items);
  }

  readonly readinessItemsApplicable = computed(() =>
    this.handoverReadinessItems().filter(i => i.status !== 'Blocked').length
  );

  readonly readinessItemsPassed = computed(() =>
    this.handoverReadinessItems().filter(i => i.status === 'Granted').length
  );

  readonly isReadinessChecklistComplete = computed(() => {
    const applicable = this.readinessItemsApplicable();
    const passed = this.readinessItemsPassed();
    return applicable > 0 && passed === applicable;
  });

  isHandoverReadyToSubmit = computed(() => {
    const r = this.kickoffReadiness();
    if (!r) return false;
    return (
      r.handoverFieldsPassed >= r.handoverFieldsApplicable &&
      this.isReadinessChecklistComplete()
    );
  });


  readonly sentimentOptions: SelectOption[] = HANDOVER_SENTIMENT_OPTIONS.map((value) => ({ value, label: value }));
  readonly timelineUnits = HANDOVER_TIMELINE_UNITS;
  readonly timelineUnitOptions: SelectOption[] = HANDOVER_TIMELINE_UNITS.map((value) => ({ value, label: value }));
  readonly maxFreeTextLength = MAX_FREE_TEXT_LENGTH;

  getSentimentTone(sentiment: string | null | undefined): StatusBadgeTone {
    switch (sentiment) {
      case 'New':
        return 'info';
      case 'Warm':
        return 'warning';
      case 'Strained':
        return 'danger';
      default:
        return 'neutral';
    }
  }

  getCommitmentTypeTone(typeId: number): StatusBadgeTone {
    switch (typeId) {
      case 1:
        return 'info';
      case 2:
        return 'success';
      case 3:
        return 'warning';
      default:
        return 'neutral';
    }
  }

  clientContextForm: FormGroup = this.fb.group({
    clientBackground: ['', [Validators.maxLength(MAX_FREE_TEXT_LENGTH)]],
    businessProblem: ['', [Validators.required, Validators.maxLength(MAX_FREE_TEXT_LENGTH)]],
    clientSentiment: ['', [Validators.required]],
    dealOrigin: ['', [Validators.maxLength(MAX_FREE_TEXT_LENGTH)]],
  });

  commitments = signal<CommitmentDto[]>([]);

  newCommitment = signal<CreateCommitmentRequest>(this.emptyCommitment());
  editingCommitmentId = signal<number | null>(null);
  editCommitment = signal<UpdateCommitmentRequest>(this.emptyCommitment());
  pendingDeleteId = signal<number | null>(null);

  editingRealisticCommitmentId = signal<number | null>(null);
  realisticEditForm = signal<RealisticEditForm>(this.emptyRealisticEditForm());

  editCommitmentErrors = signal<string[]>([]);
  realisticEditErrors = signal<string[]>([]);
  editCommitmentTouched = signal(false);
  realisticEditTouched = signal(false);

  commitmentTypes = [
    { id: 1, name: HANDOVER_LABELS.SOFTWARE },
    { id: 2, name: HANDOVER_LABELS.SERVICES },
    { id: 3, name: HANDOVER_LABELS.TRAINING },
  ];

  readonly commitmentTypeOptions: SelectOption[] = this.commitmentTypes.map((t) => ({
    value: t.id,
    label: t.name
  }));

  isDraft = computed(() => {
    const status = this.handover()?.status?.toLowerCase();
    return !status || status === 'draft';
  });
  isSubmitted = computed(() => this.handover()?.status?.toLowerCase() === 'submitted');
  isReviewed = computed(() => this.handover()?.status === 'Reviewed');
  hasBeenSubmitted = computed(() => this.isSubmitted() || this.isReviewed());
  hasHandoverDraft = computed(() => !!this.handover()?.handoverId);
  newCommitmentTouched = signal(false);

  readonly isSignedOff = computed(() => this.engagementService.isSignedOff());
  isPreSales = computed(() => !this.isSignedOff() && this.authService.hasRole('Pre Sales'));
  isProjectManagement = computed(() => !this.isSignedOff() && this.authService.hasRole('Project Management'));

  originalAcknowledgementMessage = signal('');

  acknowledgementWarningVisible = computed(() => {
    return (
      this.originalAcknowledgementMessage() !== '' &&
      this.realisticEditForm().acknowledgementMessage === this.originalAcknowledgementMessage()
    );
  });

  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.loadHandover(id);
        this.kickoffReadinessService.track(id);
        this.loadEngagement(id);
      }
    });
  }

  private loadEngagement(id: number) {
    this.engagementService.getEngagementInfo(id).pipe(
      catchError(() => of(null))
    ).subscribe(data => this.engagementInfo.set(data)); 
  }

  private emptyCommitment(): CreateCommitmentRequest {
    return { description: '', commitmentTypeId: 1, title: '', timeline: 1, timelineUnit: 'Weeks' };
  }

  private emptyRealisticEditForm(): RealisticEditForm {
    return {
      description: '',
      timeline: 1,
      timelineUnit: 'Weeks',
      acknowledgementMessage: '',
    };
  }

  loadHandover(id: number) {
    this.isLoading.set(true);
    this.handoverService.getHandover(id).subscribe({
      next: (data) => {
        this.handover.set(data);
        this.clientContextForm.patchValue({
          clientBackground: data.clientBackground || '',
          businessProblem: data.businessProblem || '',
          clientSentiment: data.clientSentiment || '',
          dealOrigin: data.dealOrigin || '',
        });
        this.commitments.set(data.commitments || []);
        this.isLoading.set(false);
        if (!this.isDraft()) {
          this.clientContextForm.disable();
          this.handoverQueryService.getQueries(id).subscribe({
            next: (qs) => this.queries.set(qs),
            error: () => {},
          });
        }

        if (
          this.isPreSales() &&
          this.isDraft() &&
          this.route.snapshot.queryParamMap.get('action') === 'create'
        ) {
          this.openAddCommitmentDialog();
        }
      },
      error: (err) => {
        if (err.status === 404) {
          this.handover.set(null);
          this.isLoading.set(false);
        } else {
          this.showMessage(HANDOVER_MESSAGES.LOAD_ERROR, true);
          this.isLoading.set(false);
        }
      },
    });
  }

  saveDraft() {
    const id = this.engagementId();
    if (!id || !this.isDraft()) return;

    const { businessProblem, clientSentiment } = this.clientContextForm.getRawValue();
    if (!businessProblem || !clientSentiment) {
      this.clientContextForm.markAllAsTouched();
      this.toastService.error(HANDOVER_MESSAGES.DRAFT_REQUIRED_FIELDS);
      return;
    }

    this.isSaving.set(true);
    const request: SaveHandoverDraftRequest = this.clientContextForm.getRawValue();

    this.handoverService.saveDraft(id, request).subscribe({
      next: (data) => {
        this.handover.set(data);
        this.toastService.success(HANDOVER_MESSAGES.SAVE_DRAFT_SUCCESS);
        this.isSaving.set(false);
        this.kickoffReadinessService.refresh();
      },
      error: (err) => {
        this.toastService.error(this.extractErrorMessage(err, HANDOVER_MESSAGES.SAVE_DRAFT_ERROR));
        this.isSaving.set(false);
      },
    });
  }

  submitHandover() {
    const id = this.engagementId();
    if (!id || !this.isDraft()) return;

    if (!this.isValidForSubmit()) {
      this.clientContextForm.markAllAsTouched();
      this.showMessage(HANDOVER_MESSAGES.SUBMIT_REQUIRED_FIELDS, true);
      window.scrollTo({ top: 0, behavior: 'smooth' });
      return;
    }

    this.isSaving.set(true);
    this.handoverService.submit(id).subscribe({
      next: (data) => {
        this.handover.set(data);
        this.clientContextForm.disable();
        this.showMessage(HANDOVER_MESSAGES.SUBMIT_SUCCESS, false);
        this.isSaving.set(false);
        this.kickoffReadinessService.refresh();
      },
      error: (err) => {
        const errorMessage = this.extractErrorMessage(err, HANDOVER_MESSAGES.SUBMIT_ERROR);
        this.showMessage(errorMessage, true);
        this.isSaving.set(false);
      }
    });
  }

  markReviewed() {
    const id = this.engagementId();
    if (!id) return;

    this.isSaving.set(true);
    this.handoverService.markReviewed(id).subscribe({
      next: (data) => {
        this.handover.set(data);
        this.showMessage('Handover marked reviewed', false);
        this.isSaving.set(false);
      },
      error: (err) => {
        this.showMessage(this.extractErrorMessage(err, 'Failed to mark handover reviewed'), true);
        this.isSaving.set(false);
      },
    });
  }

  openAddCommitmentDialog(): void {
    const id = this.engagementId();
    if (!id) return;

    const data: AddCommitmentDialogData = { engagementId: id };
    const dialogRef = this.dialog.open<AddCommitmentDialogComponent, AddCommitmentDialogData, CommitmentDto>(
      AddCommitmentDialogComponent,
      { data }
    );

    dialogRef.closed$.subscribe((response) => {
      if (!response) return;
      this.commitments.update((c) => [...c, response]);
      this.toastService.success(HANDOVER_MESSAGES.COMMITMENT_ADDED);
      this.kickoffReadinessService.refresh();
    });
  }

  startEditCommitment(c: CommitmentDto) {
    const id = this.engagementId();
    if (!id) return;

    const data: AddCommitmentDialogData = { engagementId: id, commitment: c };
    const dialogRef = this.dialog.open<AddCommitmentDialogComponent, AddCommitmentDialogData, CommitmentDto>(
      AddCommitmentDialogComponent,
      { data }
    );

    dialogRef.closed$.subscribe((response) => {
      if (!response) return;
      this.commitments.update((commits) => {
        const arr = [...commits];
        const index = arr.findIndex((item) => item.commitmentId === c.commitmentId);
        if (index !== -1) arr[index] = response;
        return arr;
      });
      this.toastService.success(HANDOVER_MESSAGES.COMMITMENT_UPDATED);
    });
  }

  cancelEditCommitment() {
    this.editingCommitmentId.set(null);
    this.editCommitmentTouched.set(false);
  }

  saveEditCommitment(commitmentId: number) {
    const id = this.engagementId();
    if (!id) return;

    this.editCommitmentTouched.set(true);
    const currentEdit = this.editCommitment();
    if (!currentEdit.title || !currentEdit.description) {
      return;
    }

    this.handoverService.updateCommitment(id, commitmentId, currentEdit).subscribe({
      next: (data) => {
        this.commitments.update((commits) => {
          const arr = [...commits];
          const index = arr.findIndex((c) => c.commitmentId === commitmentId);
          if (index !== -1) arr[index] = data;
          return arr;
        });
        this.editingCommitmentId.set(null);
        this.toastService.success(HANDOVER_MESSAGES.COMMITMENT_UPDATED);
        this.kickoffReadinessService.refresh();
      },
      error: (err) => {
        this.toastService.error(this.extractErrorMessage(err, HANDOVER_MESSAGES.COMMITMENT_UPDATE_ERROR));
      },
    });
  }

  isDeletingCommitment = signal(false);

  requestDeleteCommitment(commitmentId: number) {
    this.pendingDeleteId.set(commitmentId);
  }

  cancelDeleteCommitment() {
    this.pendingDeleteId.set(null);
  }

  confirmDeleteCommitment(commitmentId: number) {
    const id = this.engagementId();
    if (!id) return;

    this.isDeletingCommitment.set(true);
    this.handoverService.deleteCommitment(id, commitmentId).subscribe({
      next: () => {
        this.commitments.update((c) => c.filter((item) => item.commitmentId !== commitmentId));
        this.pendingDeleteId.set(null);
        this.isDeletingCommitment.set(false);
        this.toastService.success(HANDOVER_MESSAGES.COMMITMENT_DELETED);
        this.kickoffReadinessService.refresh();
      },
      error: (err) => {
        this.pendingDeleteId.set(null);
        this.isDeletingCommitment.set(false);
        this.toastService.error(this.extractErrorMessage(err, HANDOVER_MESSAGES.COMMITMENT_DELETE_ERROR));
      },
    });
  }

  startEditRealisticCommitment(c: CommitmentDto) {
    this.realisticEditTouched.set(false);
    this.editingRealisticCommitmentId.set(c.commitmentId);
    const existing = c.realisticCommitment;
    const ackMessage = existing?.acknowledgementMessage || '';
    this.originalAcknowledgementMessage.set(ackMessage);
    this.realisticEditForm.set({
      description: existing?.description || '',
      timeline: existing?.timeline || c.timeline,
      timelineUnit: existing?.timelineUnit || c.timelineUnit,
      acknowledgementMessage: ackMessage,
    });
  }

  cancelEditRealisticCommitment() {
    this.editingRealisticCommitmentId.set(null);
    this.realisticEditTouched.set(false);
  }

  commitmentNeedsAcknowledgement(c: CommitmentDto): boolean {
    if (!c.realisticCommitment) return false;
    return !timelinesMatch(c.timeline, c.timelineUnit, c.realisticCommitment.timeline, c.realisticCommitment.timelineUnit);
  }

  editFormNeedsAcknowledgement(c: CommitmentDto): boolean {
    const form = this.realisticEditForm();
    return !timelinesMatch(c.timeline, c.timelineUnit, form.timeline, form.timelineUnit);
  }

  saveRealisticCommitment(c: CommitmentDto) {
    const id = this.engagementId();
    if (!id) return;

    this.realisticEditTouched.set(true);
    const form = this.realisticEditForm();
    if (!form.description || (this.editFormNeedsAcknowledgement(c) && !form.acknowledgementMessage)) {
      return;
    }

    const payload = {
      description: form.description,
      timeline: form.timeline,
      timelineUnit: form.timelineUnit,
      acknowledgementMessage: form.acknowledgementMessage || null,
    };

    const request$ = c.realisticCommitment
      ? this.handoverService.updateRealisticCommitment(
          id,
          c.commitmentId,
          payload as UpdateRealisticCommitmentRequest,
        )
      : this.handoverService.createRealisticCommitment(
          id,
          c.commitmentId,
          payload as CreateRealisticCommitmentRequest,
        );

    request$.subscribe({
      next: (data) => {
        this.editingRealisticCommitmentId.set(null);
        this.toastService.success(HANDOVER_MESSAGES.REALISTIC_SAVED);
        this.handoverService.getHandover(id).subscribe({
          next: (refreshed) => {
            this.commitments.set(refreshed.commitments || []);
          },
        });
        this.kickoffReadinessService.refresh();
      },
      error: (err) => {
        this.toastService.error(this.extractErrorMessage(err, HANDOVER_MESSAGES.REALISTIC_SAVE_ERROR));
      },
    });
  }

  formatTimeline(timeline: number, unit: string): string {
    if (!timeline || !unit) return '';
    return timeline === 1 && unit.endsWith('s')
      ? `${timeline} ${unit.slice(0, -1)}`
      : `${timeline} ${unit}`;
  }

  private isValidForSubmit(): boolean {
    const { businessProblem, clientSentiment } = this.clientContextForm.getRawValue();
    return !!businessProblem && !!clientSentiment;
  }

  private extractErrorMessage(err: unknown, fallback: string): string {
    const errorMessage = (err as { error?: { message?: string } })?.error?.message;
    return errorMessage || fallback;
  }

  onClientContactSaved(): void {
    this.showMessage(HANDOVER_MESSAGES.CLIENT_CONTACT_TRANSITION_SAVED, false);
  }

  private showMessage(msg: string, isErr: boolean) {
    this.message.set(msg);
    this.isError.set(isErr);
    setTimeout(() => {
      this.message.set('');
    }, 5000);
  }
}
