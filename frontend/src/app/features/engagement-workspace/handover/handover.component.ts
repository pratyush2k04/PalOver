import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import {
  HandoverService,
  HandoverInfoDto,
  SaveHandoverDraftRequest,
  CreateCommitmentRequest,
  UpdateCommitmentRequest,
  CommitmentDto
} from './handover.service';

const MAX_FREE_TEXT_LENGTH = 2000;
const SENTIMENT_OPTIONS = ['New', 'Warm', 'Strained'] as const;

@Component({
  selector: 'app-handover',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './handover.component.html',
  styleUrl: './handover.component.scss'
})
export class HandoverComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private handoverService = inject(HandoverService);
  private fb = inject(FormBuilder);

  engagementId!: number;
  handover: HandoverInfoDto | null = null;
  isLoading = true;
  isSaving = false;
  message = '';
  isError = false;

  readonly sentimentOptions = SENTIMENT_OPTIONS;
  readonly maxFreeTextLength = MAX_FREE_TEXT_LENGTH;

  clientContextForm: FormGroup = this.fb.group({
    clientBackground: ['', [Validators.maxLength(MAX_FREE_TEXT_LENGTH)]],
    businessProblem: ['', [Validators.required, Validators.maxLength(MAX_FREE_TEXT_LENGTH)]],
    clientSentiment: ['', [Validators.required]],
    dealOrigin: ['', [Validators.maxLength(MAX_FREE_TEXT_LENGTH)]],
    hasNoCommitments: [false]
  });

  commitments: CommitmentDto[] = [];

  newCommitment: CreateCommitmentRequest = this.emptyCommitment();
  editingCommitmentId: number | null = null;
  editCommitment: UpdateCommitmentRequest = this.emptyCommitment();
  pendingDeleteId: number | null = null;

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

  get isSubmitted(): boolean {
    return this.handover?.status === 'Submitted';
  }

  ngOnInit() {
    this.route.parent?.paramMap.subscribe(params => {
      const idStr = params.get('id');
      if (idStr) {
        this.engagementId = +idStr;
        this.loadHandover();
      }
    });
  }

  private emptyCommitment(): CreateCommitmentRequest {
    return { description: '', commitmentTypeId: 1, quantity: '' };
  }

  loadHandover() {
    this.isLoading = true;
    this.handoverService.getHandover(this.engagementId).subscribe({
      next: (data) => {
        this.handover = data;
        this.clientContextForm.patchValue({
          clientBackground: data.clientBackground || '',
          businessProblem: data.businessProblem || '',
          clientSentiment: data.clientSentiment || '',
          dealOrigin: data.dealOrigin || '',
          hasNoCommitments: data.hasNoCommitments
        });
        this.commitments = data.commitments || [];
        this.isLoading = false;
        if (this.isSubmitted) {
          this.clientContextForm.disable();
        }
      },
      error: (err) => {
        // If 404, it might mean no draft exists yet, we can initialize empty
        if (err.status === 404) {
          this.handover = null;
          this.isLoading = false;
        } else {
          this.showMessage('Failed to load handover details', true);
          this.isLoading = false;
        }
      }
    });
  }

  saveDraft() {
    this.isSaving = true;
    const request: SaveHandoverDraftRequest = this.clientContextForm.getRawValue();

    this.handoverService.saveDraft(this.engagementId, request).subscribe({
      next: (data) => {
        this.handover = data;
        this.showMessage('Draft saved successfully', false);
        this.isSaving = false;
      },
      error: (err) => {
        this.showMessage(this.extractErrorMessage(err, 'Failed to save draft'), true);
        this.isSaving = false;
      }
    });
  }

  submitHandover() {
    if (!this.isValidForSubmit()) {
      this.showMessage('Please complete all required fields and ensure commitments are added or explicitly marked as none.', true);
      return;
    }

    this.isSaving = true;
    this.handoverService.submit(this.engagementId).subscribe({
      next: (data) => {
        this.handover = data;
        this.clientContextForm.disable();
        this.showMessage('Handover submitted successfully', false);
        this.isSaving = false;
      },
      error: (err) => {
        this.showMessage(this.extractErrorMessage(err, 'Failed to submit handover'), true);
        this.isSaving = false;
      }
    });
  }

  addCommitment() {
    if (!this.newCommitment.description) {
      this.showMessage('Commitment description is required.', true);
      return;
    }

    this.handoverService.addCommitment(this.engagementId, this.newCommitment).subscribe({
      next: (data) => {
        this.commitments.push(data);
        this.clientContextForm.patchValue({ hasNoCommitments: false });
        this.newCommitment = this.emptyCommitment();
        this.showMessage('Commitment added successfully', false);
      },
      error: (err) => {
        this.showMessage(this.extractErrorMessage(err, 'Failed to add commitment'), true);
      }
    });
  }

  startEditCommitment(c: CommitmentDto) {
    this.editingCommitmentId = c.commitmentId;
    this.editCommitment = {
      description: c.description,
      commitmentTypeId: c.commitmentTypeId,
      quantity: c.quantity
    };
  }

  cancelEditCommitment() {
    this.editingCommitmentId = null;
  }

  saveEditCommitment(id: number) {
    if (!this.editCommitment.description) {
      this.showMessage('Commitment description is required.', true);
      return;
    }

    this.handoverService.updateCommitment(this.engagementId, id, this.editCommitment).subscribe({
      next: (data) => {
        const index = this.commitments.findIndex(c => c.commitmentId === id);
        if (index !== -1) {
          this.commitments[index] = data;
        }
        this.editingCommitmentId = null;
        this.showMessage('Commitment updated successfully', false);
      },
      error: (err) => {
        this.showMessage(this.extractErrorMessage(err, 'Failed to update commitment'), true);
      }
    });
  }

  requestDeleteCommitment(id: number) {
    this.pendingDeleteId = id;
  }

  cancelDeleteCommitment() {
    this.pendingDeleteId = null;
  }

  confirmDeleteCommitment(id: number) {
    this.handoverService.deleteCommitment(this.engagementId, id).subscribe({
      next: () => {
        this.commitments = this.commitments.filter(c => c.commitmentId !== id);
        this.pendingDeleteId = null;
        this.showMessage('Commitment deleted', false);
      },
      error: (err) => {
        this.pendingDeleteId = null;
        this.showMessage(this.extractErrorMessage(err, 'Failed to delete commitment'), true);
      }
    });
  }

  onHasNoCommitmentsChange() {
    if (this.clientContextForm.get('hasNoCommitments')?.value) {
      this.commitments = [];
    }
  }

  private isValidForSubmit(): boolean {
    const { businessProblem, clientSentiment, hasNoCommitments } = this.clientContextForm.getRawValue();
    if (!businessProblem || !clientSentiment) {
      return false;
    }
    if (this.commitments.length === 0 && !hasNoCommitments) {
      return false;
    }
    return true;
  }

  private extractErrorMessage(err: unknown, fallback: string): string {
    const message = (err as { error?: { message?: string } })?.error?.message;
    return message || fallback;
  }

  private showMessage(msg: string, isErr: boolean) {
    this.message = msg;
    this.isError = isErr;
    setTimeout(() => { this.message = ''; }, 5000);
  }
}
