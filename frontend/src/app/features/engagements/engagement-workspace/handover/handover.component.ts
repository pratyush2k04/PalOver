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
import { ActivatedRoute, Router } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { HandoverService } from '../../services/handover.service';
import { EngagementService } from '../../services/engagement.service';
import { catchError, of } from 'rxjs';
import { AuthService } from '@shared/services/auth';
import { KickoffReadinessDto, KickoffReadinessService } from '@shared/services/kickoff-readiness.service';
import { EngagementInfoDto } from '../../models/engagement.model';
import { FlaggedItemsBannerComponent, FlaggedItem } from '@shared/flagged-items-banner/flagged-items-banner.component';
import { ENGAGEMENT_ROUTES } from '@shared/constants/app.constants';
import {
  HandoverInfoDto,
  SaveHandoverDraftRequest,
  CreateCommitmentRequest,
  UpdateCommitmentRequest,
  CreateRealisticCommitmentRequest,
  UpdateRealisticCommitmentRequest,
  CommitmentDto
} from '../../models/handover.model';
import { AlertComponent } from '@common/components/alert/alert';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { StatusBadgeComponent, StatusBadgeTone } from '@common/components/status-badge/status-badge';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import { SectionHeaderComponent } from '@common/components/section-header/section-header';
import { ClientContactTransitionCardComponent } from './client-contact-transition-card/client-contact-transition-card.component';

const MAX_FREE_TEXT_LENGTH = 2000;
const SENTIMENT_OPTIONS = ['New', 'Warm', 'Strained'] as const;
const TIMELINE_UNITS = ['Days', 'Weeks', 'Months'] as const;

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
    FlaggedItemsBannerComponent,
    AlertComponent,
    ButtonComponent,
    InputComponent,
    TextareaComponent,
    SelectComponent,
    StatusBadgeComponent,
    EmptyStateComponent,
    ClientContactTransitionCardComponent,
  ],
  templateUrl: './handover.component.html',
  styleUrl: './handover.component.scss',
})
export class HandoverComponent {
  private static readonly KICKOFF_NOT_READY_MESSAGE =
    'Engagement is not kickoff-ready. All mandatory items must be complete before submitting the handover.';

  engagementInfo = signal<EngagementInfoDto | null>(null);

  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private handoverService = inject(HandoverService);
  private engagementService = inject(EngagementService);
  private kickoffReadinessService = inject(KickoffReadinessService);
  private authService = inject(AuthService);
  private fb = inject(FormBuilder);

  private parentParamMap = toSignal(this.route.parent!.paramMap);

  engagementId = computed(() => {
    const idStr = this.parentParamMap()?.get('id');
    return idStr ? +idStr : null;
  });

  handover = signal<HandoverInfoDto | null>(null);
  isLoading = signal(true);
  isSaving = signal(false);
  message = signal('');
  isError = signal(false);

  kickoffReadiness = signal<KickoffReadinessDto | null>(null);
  handoverFlaggedItems = computed<FlaggedItem[]>(() => {
    const r = this.kickoffReadiness();
    if (!r) return [];
    return [
      { label: 'mandatory handover field(s) incomplete', count: r.handoverFieldsApplicable - r.handoverFieldsPassed },
      { label: 'commitment(s) missing acknowledgement', count: r.commitmentsApplicable - r.commitmentsPassed }
    ];
  });

  readonly sentimentOptions: SelectOption[] = SENTIMENT_OPTIONS.map((value) => ({ value, label: value }));
  readonly timelineUnits = TIMELINE_UNITS;
  readonly timelineUnitOptions: SelectOption[] = TIMELINE_UNITS.map((value) => ({ value, label: value }));
  readonly maxFreeTextLength = MAX_FREE_TEXT_LENGTH;

  getSentimentTone(sentiment: string | null | undefined): StatusBadgeTone {
    switch (sentiment) {
      case 'Warm':
        return 'info';
      case 'Strained':
        return 'danger';
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

  commitmentTypes = [
    { id: 1, name: 'Software' },
    { id: 2, name: 'Services' },
    { id: 3, name: 'Training' },
  ];

  readonly commitmentTypeOptions: SelectOption[] = this.commitmentTypes.map((t) => ({
    value: t.id,
    label: t.name
  }));

  isSubmitted = computed(() => this.handover()?.status === 'Submitted');
  newCommitmentTouched = signal(false);

  isPreSales = computed(() => this.authService.hasRole('Pre Sales'));
  isProjectManagement = computed(() => this.authService.hasRole('Project Management'));

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
        this.loadKickoffReadiness(id);
        this.loadEngagement(id);
      }
    });
  }

  private loadKickoffReadiness(id: number) {
    this.kickoffReadinessService.getReadiness(id).pipe(
      catchError(() => of(null))
    ).subscribe(data => this.kickoffReadiness.set(data));
  }

  private loadEngagement(id: number) {
    this.engagementService.getEngagementInfo(id).pipe(
      catchError(() => of(null))   //when an api falls it catches the error and returns null instead the crashing the component
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
        if (this.isSubmitted()) {
          this.clientContextForm.disable();
        }
      },
      error: (err) => {
        if (err.status === 404) {
          this.handover.set(null);
          this.isLoading.set(false);
        } else {
          this.showMessage('Failed to load handover details', true);
          this.isLoading.set(false);
        }
      },
    });
  }

  saveDraft() {
    const id = this.engagementId();
    if (!id) return;

    const { businessProblem, clientSentiment } = this.clientContextForm.getRawValue();
    if (!businessProblem || !clientSentiment) {
      this.clientContextForm.markAllAsTouched();
      this.showMessage('Business Problem and Client Sentiment are required to save a draft.', true);
      window.scrollTo({ top: 0, behavior: 'smooth' });
      return;
    }

    this.isSaving.set(true);
    const request: SaveHandoverDraftRequest = this.clientContextForm.getRawValue();

    this.handoverService.saveDraft(id, request).subscribe({
      next: (data) => {
        this.handover.set(data);
        this.showMessage('Draft saved successfully', false);
        this.isSaving.set(false);
      },
      error: (err) => {
        this.showMessage(this.extractErrorMessage(err, 'Failed to save draft'), true);
        this.isSaving.set(false);
      },
    });
  }

  submitHandover() {
    const id = this.engagementId();
    if (!id) return;

    if (!this.isValidForSubmit()) {
      this.clientContextForm.markAllAsTouched();
      this.showMessage('Please complete all required fields.', true);
      window.scrollTo({ top: 0, behavior: 'smooth' });
      return;
    }

    this.isSaving.set(true);
    this.handoverService.submit(id).subscribe({
      next: (data) => {
        this.handover.set(data);
        this.clientContextForm.disable();
        this.showMessage('Handover submitted successfully', false);
        this.isSaving.set(false);
      },
      error: (err) => {
        const errorMessage = this.extractErrorMessage(err, 'Failed to submit handover');
        this.showMessage(errorMessage, true);
        this.isSaving.set(false);

        if (errorMessage === HandoverComponent.KICKOFF_NOT_READY_MESSAGE) {
          setTimeout(() => {
            this.router.navigate(['../', ENGAGEMENT_ROUTES.KICKOFF], { relativeTo: this.route });
          }, 1500);
        }
      }
    });
  }

  addCommitment() {
    const id = this.engagementId();
    if (!id) return;

    const currentNew = this.newCommitment();
    if (!currentNew.description || !currentNew.title) {
      return;
    }

    this.handoverService.addCommitment(id, currentNew).subscribe({
      next: (data) => {
        this.commitments.update((c) => [...c, data]);
        this.newCommitment.set(this.emptyCommitment());
        this.newCommitmentTouched.set(false);
        this.showMessage('Commitment added successfully', false);
      },
      error: (err) => {
        this.showMessage(this.extractErrorMessage(err, 'Failed to add commitment'), true);
      },
    });
  }

  startEditCommitment(c: CommitmentDto) {
    this.editCommitmentErrors.set([]);
    this.editingCommitmentId.set(c.commitmentId);
    this.editCommitment.set({
      description: c.description,
      commitmentTypeId: c.commitmentTypeId,
      title: c.title,
      timeline: c.timeline,
      timelineUnit: c.timelineUnit,
    });
  }

  cancelEditCommitment() {
    this.editingCommitmentId.set(null);
    this.editCommitmentErrors.set([]);
  }

  saveEditCommitment(commitmentId: number) {
    const id = this.engagementId();
    if (!id) return;

    const currentEdit = this.editCommitment();
    const errors: string[] = [];
    if (!currentEdit.title) errors.push('Title is required.');
    if (!currentEdit.description) errors.push('Description is required.');

    if (errors.length > 0) {
      this.editCommitmentErrors.set(errors);
      return;
    }

    this.editCommitmentErrors.set([]);

    this.handoverService.updateCommitment(id, commitmentId, currentEdit).subscribe({
      next: (data) => {
        this.commitments.update((commits) => {
          const arr = [...commits];
          const index = arr.findIndex((c) => c.commitmentId === commitmentId);
          if (index !== -1) arr[index] = data;
          return arr;
        });
        this.editingCommitmentId.set(null);
        this.showMessage('Commitment updated successfully', false);
      },
      error: (err) => {
        this.showMessage(this.extractErrorMessage(err, 'Failed to update commitment'), true);
      },
    });
  }

  requestDeleteCommitment(commitmentId: number) {
    this.pendingDeleteId.set(commitmentId);
  }

  cancelDeleteCommitment() {
    this.pendingDeleteId.set(null);
  }

  confirmDeleteCommitment(commitmentId: number) {
    const id = this.engagementId();
    if (!id) return;

    this.handoverService.deleteCommitment(id, commitmentId).subscribe({
      next: () => {
        this.commitments.update((c) => c.filter((item) => item.commitmentId !== commitmentId));
        this.pendingDeleteId.set(null);
        this.showMessage('Commitment deleted', false);
      },
      error: (err) => {
        this.pendingDeleteId.set(null);
        this.showMessage(this.extractErrorMessage(err, 'Failed to delete commitment'), true);
      },
    });
  }

  startEditRealisticCommitment(c: CommitmentDto) {
    this.realisticEditErrors.set([]);
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
    this.realisticEditErrors.set([]);
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

    const form = this.realisticEditForm();
    const errors: string[] = [];
    if (!form.description) errors.push('Description is required.');
    if (this.editFormNeedsAcknowledgement(c) && !form.acknowledgementMessage) {
      errors.push('An acknowledgement message is required when the realistic timeline differs from the promised timeline.');
    }

    if (errors.length > 0) {
      this.realisticEditErrors.set(errors);
      return;
    }

    this.realisticEditErrors.set([]);

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
        this.commitments.update((commits) =>
          commits.map((item) =>
            item.commitmentId === c.commitmentId ? { ...item, realisticCommitment: data } : item,
          ),
        );
        this.editingRealisticCommitmentId.set(null);
        this.showMessage('Realistic commitment saved', false);
      },
      error: (err) => {
        this.showMessage(
          this.extractErrorMessage(err, 'Failed to save realistic commitment'),
          true,
        );
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
    this.showMessage('Client contact transition recorded.', false);
  }

  private showMessage(msg: string, isErr: boolean) {
    this.message.set(msg);
    this.isError.set(isErr);
    setTimeout(() => {
      this.message.set('');
    }, 5000);
  }
}
