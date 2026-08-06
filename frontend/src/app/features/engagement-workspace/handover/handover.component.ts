import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import {
  HandoverService,
  HandoverInfoDto,
  SaveHandoverDraftRequest,
  CreateCommitmentRequest,
  UpdateCommitmentRequest,
  CommitmentDto
} from './handover.service';
import { AlertComponent } from '../../../common/components/alert/alert';
import { EmptyStateComponent } from '../../../common/components/empty-state/empty-state';
import { ButtonComponent } from '../../../common/components/button/button';

const MAX_FREE_TEXT_LENGTH = 2000;
const SENTIMENT_OPTIONS = ['New', 'Warm', 'Strained'] as const;

@Component({
  selector: 'app-handover',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, AlertComponent, EmptyStateComponent, ButtonComponent],
  templateUrl: './handover.component.html',
  styleUrl: './handover.component.scss'
})
export class HandoverComponent {
  private route = inject(ActivatedRoute);
  private handoverService = inject(HandoverService);
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

  readonly sentimentOptions = SENTIMENT_OPTIONS;
  readonly maxFreeTextLength = MAX_FREE_TEXT_LENGTH;

  clientContextForm: FormGroup = this.fb.group({
    clientBackground: ['', [Validators.maxLength(MAX_FREE_TEXT_LENGTH)]],
    businessProblem: ['', [Validators.required, Validators.maxLength(MAX_FREE_TEXT_LENGTH)]],
    clientSentiment: ['', [Validators.required]],
    dealOrigin: ['', [Validators.maxLength(MAX_FREE_TEXT_LENGTH)]],
    hasNoCommitments: [false]
  });

  commitments = signal<CommitmentDto[]>([]);

  newCommitment = signal<CreateCommitmentRequest>(this.emptyCommitment());
  newCommitmentTouched = signal(false);
  editingCommitmentId = signal<number | null>(null);
  editCommitment = signal<UpdateCommitmentRequest>(this.emptyCommitment());
  pendingDeleteId = signal<number | null>(null);

  commitmentTypes = [
    { id: 1, name: 'Software' },
    { id: 2, name: 'Services' },
    { id: 3, name: 'Training' }
  ];

  get clientBackgroundControl() {
    return this.clientContextForm.get('clientBackground')!;
  }

  get businessProblemControl() {
    return this.clientContextForm.get('businessProblem')!;
  }

  get clientSentimentControl() {
    return this.clientContextForm.get('clientSentiment')!;
  }

  get dealOriginControl() {
    return this.clientContextForm.get('dealOrigin')!;
  }

  isSubmitted = computed(() => this.handover()?.status === 'Submitted');

  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.loadHandover(id);
      }
    });
  }

  private emptyCommitment(): CreateCommitmentRequest {
    return { description: '', commitmentTypeId: 1, quantity: '' };
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
          hasNoCommitments: data.hasNoCommitments
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
      }
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
      }
    });
  }

  submitHandover() {
    const id = this.engagementId();
    if (!id) return;

    if (!this.isValidForSubmit()) {
      this.clientContextForm.markAllAsTouched();
      this.showMessage('Please complete all required fields and ensure commitments are added or explicitly marked as none.', true);
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
        this.showMessage(this.extractErrorMessage(err, 'Failed to submit handover'), true);
        this.isSaving.set(false);
      }
    });
  }

  addCommitment() {
    const id = this.engagementId();
    if (!id) return;

    this.newCommitmentTouched.set(true);
    const currentNew = this.newCommitment();
    if (!currentNew.description) {
      return;
    }

    this.handoverService.addCommitment(id, currentNew).subscribe({
      next: (data) => {
        this.commitments.update(c => [...c, data]);
        this.clientContextForm.patchValue({ hasNoCommitments: false });
        this.newCommitment.set(this.emptyCommitment());
        this.showMessage('Commitment added successfully', false);
      },
      error: (err) => {
        this.showMessage(this.extractErrorMessage(err, 'Failed to add commitment'), true);
      }
    });
  }

  startEditCommitment(c: CommitmentDto) {
    this.editingCommitmentId.set(c.commitmentId);
    this.editCommitment.set({
      description: c.description,
      commitmentTypeId: c.commitmentTypeId,
      quantity: c.quantity
    });
  }

  cancelEditCommitment() {
    this.editingCommitmentId.set(null);
  }

  saveEditCommitment(commitmentId: number) {
    const id = this.engagementId();
    if (!id) return;

    const currentEdit = this.editCommitment();
    if (!currentEdit.description) {
      this.showMessage('Commitment description is required.', true);
      return;
    }

    this.handoverService.updateCommitment(id, commitmentId, currentEdit).subscribe({
      next: (data) => {
        this.commitments.update(commits => {
          const arr = [...commits];
          const index = arr.findIndex(c => c.commitmentId === commitmentId);
          if (index !== -1) arr[index] = data;
          return arr;
        });
        this.editingCommitmentId.set(null);
        this.showMessage('Commitment updated successfully', false);
      },
      error: (err) => {
        this.showMessage(this.extractErrorMessage(err, 'Failed to update commitment'), true);
      }
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
        this.commitments.update(c => c.filter(item => item.commitmentId !== commitmentId));
        this.pendingDeleteId.set(null);
        this.showMessage('Commitment deleted', false);
      },
      error: (err) => {
        this.pendingDeleteId.set(null);
        this.showMessage(this.extractErrorMessage(err, 'Failed to delete commitment'), true);
      }
    });
  }

  onHasNoCommitmentsChange() {
    if (this.clientContextForm.get('hasNoCommitments')?.value) {
      this.commitments.set([]);
    }
  }

  private isValidForSubmit(): boolean {
    const { businessProblem, clientSentiment, hasNoCommitments } = this.clientContextForm.getRawValue();
    if (!businessProblem || !clientSentiment) {
      return false;
    }
    if (this.commitments().length === 0 && !hasNoCommitments) {
      return false;
    }
    return true;
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
