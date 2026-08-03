import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { HandoverService, HandoverInfoDto, SaveHandoverDraftRequest, CreateCommitmentRequest, CommitmentDto } from './handover.service';

@Component({
  selector: 'app-handover',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './handover.component.html',
  styleUrls: ['./handover.component.css']
})
export class HandoverComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private handoverService = inject(HandoverService);

  engagementId!: number;
  handover: HandoverInfoDto | null = null;
  isLoading = true;
  isSaving = false;
  message = '';
  isError = false;

  // Form Models
  clientBackground = '';
  businessProblem = '';
  clientSentiment = '';
  dealOrigin = '';
  hasNoCommitments = false;

  commitments: CommitmentDto[] = [];
  
  newCommitment: CreateCommitmentRequest = {
    description: '',
    commitmentTypeId: 1, // Default to Software
    quantity: ''
  };

  commitmentTypes = [
    { id: 1, name: 'Software' },
    { id: 2, name: 'Services' },
    { id: 3, name: 'Training' }
  ];

  ngOnInit() {
    this.route.parent?.paramMap.subscribe(params => {
      const idStr = params.get('id');
      if (idStr) {
        this.engagementId = +idStr;
        this.loadHandover();
      }
    });
  }

  loadHandover() {
    this.isLoading = true;
    this.handoverService.getHandover(this.engagementId).subscribe({
      next: (data) => {
        this.handover = data;
        this.clientBackground = data.clientBackground || '';
        this.businessProblem = data.businessProblem || '';
        this.clientSentiment = data.clientSentiment || '';
        this.dealOrigin = data.dealOrigin || '';
        this.hasNoCommitments = data.hasNoCommitments;
        this.commitments = data.commitments || [];
        this.isLoading = false;
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
    const request: SaveHandoverDraftRequest = {
      clientBackground: this.clientBackground,
      businessProblem: this.businessProblem,
      clientSentiment: this.clientSentiment,
      dealOrigin: this.dealOrigin,
      hasNoCommitments: this.hasNoCommitments
    };

    this.handoverService.saveDraft(this.engagementId, request).subscribe({
      next: (data) => {
        this.handover = data;
        this.showMessage('Draft saved successfully', false);
        this.isSaving = false;
      },
      error: () => {
        this.showMessage('Failed to save draft', true);
        this.isSaving = false;
      }
    });
  }

  submitHandover() {
    if (!this.isValidForSubmit()) {
      this.showMessage('Please complete all required fields and ensure commitments are added or explicitly marked as none.', true);
      return;
    }

    if (confirm('Are you sure you want to submit? This cannot be undone.')) {
      this.isSaving = true;
      this.handoverService.submit(this.engagementId).subscribe({
        next: (data) => {
          this.handover = data;
          this.showMessage('Handover submitted successfully', false);
          this.isSaving = false;
        },
        error: (err) => {
          this.showMessage(err.error?.Message || 'Failed to submit handover', true);
          this.isSaving = false;
        }
      });
    }
  }

  addCommitment() {
    if (!this.newCommitment.description) {
      this.showMessage('Commitment description is required.', true);
      return;
    }

    this.handoverService.addCommitment(this.engagementId, this.newCommitment).subscribe({
      next: (data) => {
        this.commitments.push(data);
        this.hasNoCommitments = false;
        this.newCommitment = { description: '', commitmentTypeId: 1, quantity: '' };
        this.showMessage('Commitment added successfully', false);
      },
      error: () => {
        this.showMessage('Failed to add commitment', true);
      }
    });
  }

  deleteCommitment(id: number) {
    if (confirm('Delete this commitment?')) {
      this.handoverService.deleteCommitment(this.engagementId, id).subscribe({
        next: () => {
          this.commitments = this.commitments.filter(c => c.commitmentId !== id);
          this.showMessage('Commitment deleted', false);
        },
        error: () => {
          this.showMessage('Failed to delete commitment', true);
        }
      });
    }
  }

  private isValidForSubmit(): boolean {
    if (!this.clientBackground || !this.businessProblem || !this.clientSentiment) {
      return false;
    }
    if (this.commitments.length === 0 && !this.hasNoCommitments) {
      return false;
    }
    return true;
  }

  private showMessage(msg: string, isErr: boolean) {
    this.message = msg;
    this.isError = isErr;
    setTimeout(() => { this.message = ''; }, 5000);
  }
}
