import { Component, inject, input, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from 'src/app/services/auth';
import { DialogService } from '@common/services/dialog/dialog.service';
import { HandoverQueryService } from '../../../services/handover-query.service';
import {
  HandoverQueryDto,
  HandoverQueryFieldSection,
  HANDOVER_SECTION_FRAGMENT_IDS
} from '../../../models/handover-query.model';
import { CommitmentDto } from '../../../models/handover.model';
import { ButtonComponent } from '@common/components/button/button';
import { StatusBadgeComponent } from '@common/components/status-badge/status-badge';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import { AlertComponent } from '@common/components/alert/alert';
import {
  RaiseHandoverQueryDialogComponent,
  RaiseHandoverQueryDialogData
} from './dialogs/raise-handover-query-dialog/raise-handover-query-dialog.component';

const FIELD_SECTION_LABELS: Record<HandoverQueryFieldSection, string> = {
  [HandoverQueryFieldSection.ClientBackground]: 'Client Background',
  [HandoverQueryFieldSection.BusinessProblem]: 'Business Problem',
  [HandoverQueryFieldSection.ClientSentiment]: 'Client Sentiment',
  [HandoverQueryFieldSection.DealOrigin]: 'Deal Origin',
  [HandoverQueryFieldSection.Commitment]: 'Commitment',
};

@Component({
  selector: 'app-handover-queries',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent, StatusBadgeComponent, TextareaComponent, EmptyStateComponent, AlertComponent],
  templateUrl: './handover-queries.component.html',
  styleUrl: './handover-queries.component.scss',
})
export class HandoverQueriesComponent {
  engagementId = input.required<number>();
  commitments = input<CommitmentDto[]>([]);

  private handoverQueryService = inject(HandoverQueryService);
  private authService = inject(AuthService);
  private dialogService = inject(DialogService);

  queries = signal<HandoverQueryDto[]>([]);
  isLoading = signal(true);
  loadError = signal(false);
  message = signal('');
  isError = signal(false);

  responseDrafts = signal<Record<number, string>>({});
  pendingResolutionToggles = signal<ReadonlySet<number>>(new Set());

  isPreSales = computed(() => this.authService.hasRole('Pre Sales'));
  isProjectManagement = computed(() => this.authService.hasRole('Project Management'));

  openQueries = computed(() => this.queries().filter((q) => !q.isResolved));
  resolvedQueries = computed(() => this.queries().filter((q) => q.isResolved));

  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.loadQueries(id);
      }
    });
  }

  loadQueries(id: number): void {
    this.isLoading.set(true);
    this.loadError.set(false);
    this.handoverQueryService.getQueries(id).subscribe({
      next: (data) => {
        this.queries.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.loadError.set(true);
        this.message.set(this.extractErrorMessage(err, 'Failed to load handover queries.'));
        this.isError.set(true);
      },
    });
  }

  fieldSectionLabel(query: HandoverQueryDto): string {
    if (query.fieldSection === HandoverQueryFieldSection.Commitment && query.referenceId) {
      const commitment = this.commitments().find((c) => c.commitmentId === query.referenceId);
      return commitment ? `Commitment: ${commitment.title}` : FIELD_SECTION_LABELS[query.fieldSection];
    }
    return FIELD_SECTION_LABELS[query.fieldSection];
  }

  jumpToField(query: HandoverQueryDto): void {
    const baseId = HANDOVER_SECTION_FRAGMENT_IDS[query.fieldSection];
    const fragmentId =
      query.fieldSection === HandoverQueryFieldSection.Commitment && query.referenceId
        ? `${baseId}-${query.referenceId}`
        : baseId;

    document.getElementById(fragmentId)?.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }

  responseDraft(queryId: number): string {
    return this.responseDrafts()[queryId] ?? '';
  }

  updateResponseDraft(queryId: number, value: string): void {
    this.responseDrafts.update((drafts) => ({ ...drafts, [queryId]: value }));
  }

  submitResponse(query: HandoverQueryDto): void {
    const id = this.engagementId();
    const responseText = this.responseDraft(query.queryId).trim();
    if (!responseText) return;

    this.handoverQueryService.respondToQuery(id, query.queryId, { responseText }).subscribe({
      next: (updated) => {
        this.queries.update((qs) => qs.map((q) => (q.queryId === updated.queryId ? updated : q)));
        this.showMessage('Response submitted.', false);
      },
      error: (err) => this.showMessage(this.extractErrorMessage(err, 'Failed to submit response.'), true),
    });
  }

  toggleResolve(query: HandoverQueryDto): void {
    if (this.pendingResolutionToggles().has(query.queryId)) return;

    const id = this.engagementId();
    this.pendingResolutionToggles.update((s) => new Set(s).add(query.queryId));

    this.handoverQueryService.toggleQueryResolution(id, query.queryId).subscribe({
      next: (updated) => {
        this.queries.update((qs) => qs.map((q) => (q.queryId === updated.queryId ? updated : q)));
      },
      error: (err) => this.showMessage(this.extractErrorMessage(err, 'Failed to update query.'), true),
      complete: () => {
        this.pendingResolutionToggles.update((s) => {
          const next = new Set(s);
          next.delete(query.queryId);
          return next;
        });
      },
    });
  }

  openRaiseQueryDialog(): void {
    const dialogRef = this.dialogService.open<RaiseHandoverQueryDialogComponent, RaiseHandoverQueryDialogData, HandoverQueryDto>(
      RaiseHandoverQueryDialogComponent,
      {
        data: {
          engagementId: this.engagementId(),
          commitments: this.commitments(),
        },
      }
    );

    dialogRef.afterClosed().subscribe((created) => {
      if (created) {
        this.queries.update((qs) => [...qs, created]);
        this.showMessage('Query raised.', false);
      }
    });
  }

  private showMessage(msg: string, isErr: boolean): void {
    this.message.set(msg);
    this.isError.set(isErr);
    setTimeout(() => this.message.set(''), 5000);
  }

  private extractErrorMessage(err: unknown, fallback: string): string {
    const errorMessage = (err as { error?: { message?: string } })?.error?.message;
    return errorMessage || fallback;
  }
}
