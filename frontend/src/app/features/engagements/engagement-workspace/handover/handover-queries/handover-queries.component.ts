import { Component, inject, input, output, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '@shared/services/auth';
import { DialogService } from '@common/services/dialog/dialog.service';
import { HandoverQueryService } from '../../../services/handover-query.service';
import {
  HandoverQueryDto,
  HandoverQueryMessageDto,
  HandoverQueryFieldSection,
  HANDOVER_SECTION_FRAGMENT_IDS
} from '../../../models/handover-query.model';
import { CommitmentDto } from '../../../models/handover.model';
import { AccordionComponent } from '@common/components/accordion/accordion';
import { AvatarComponent } from '@common/components/avatar/avatar';
import { ButtonComponent } from '@common/components/button/button';
import { StatusBadgeComponent } from '@common/components/status-badge/status-badge';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import { AlertComponent } from '@common/components/alert/alert';
import { ConfirmDialogComponent } from '@common/components/confirm-dialog/confirm-dialog';
import {
  RaiseHandoverQueryDialogComponent,
  RaiseHandoverQueryDialogData
} from './dialogs/raise-handover-query-dialog/raise-handover-query-dialog.component';
import {
  HANDOVER_MESSAGES,
  HANDOVER_QUERY_FIELD_LABELS
} from '@shared/constants/handover.constants';

@Component({
  selector: 'app-handover-queries',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    AccordionComponent,
    AvatarComponent,
    ButtonComponent,
    StatusBadgeComponent,
    TextareaComponent,
    EmptyStateComponent,
    AlertComponent,
    ConfirmDialogComponent
  ],
  templateUrl: './handover-queries.component.html',
  styleUrl: './handover-queries.component.scss',
})
export class HandoverQueriesComponent {
  engagementId = input.required<number>();
  commitments = input<CommitmentDto[]>([]);
  isReviewed = input<boolean>(false);
  queriesChange = output<HandoverQueryDto[]>();

  private handoverQueryService = inject(HandoverQueryService);
  private authService = inject(AuthService);
  private dialogService = inject(DialogService);

  queries = signal<HandoverQueryDto[]>([]);
  isLoading = signal(true);
  loadError = signal(false);
  message = signal('');
  isError = signal(false);

  messageDrafts = signal<Record<number, string>>({});
  expandedQueries = signal<Record<number, boolean>>({});
  pendingResolutionToggles = signal<ReadonlySet<number>>(new Set());
  submittingMessages = signal<ReadonlySet<number>>(new Set());

  // Delete Confirmation State
  pendingDeleteQuery = signal<HandoverQueryDto | null>(null);
  isDeletingQuery = signal<boolean>(false);

  isPreSales = computed(() => this.authService.hasRole('Pre Sales'));
  isProjectManagement = computed(() => this.authService.hasRole('Project Management'));

  openQueries = computed(() =>
    this.queries()
      .filter((q) => !q.isResolved)
      .slice()
      .sort((a, b) => this.compareQueriesLatestFirst(a, b))
  );
  resolvedQueries = computed(() =>
    this.queries()
      .filter((q) => q.isResolved)
      .slice()
      .sort((a, b) => this.compareQueriesLatestFirst(a, b))
  );
  sortedQueries = computed(() => [...this.openQueries(), ...this.resolvedQueries()]);

  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.loadQueries(id);
      }
    });
  }

  loadQueries(id: number, onDone?: () => void): void {
    this.isLoading.set(true);
    this.loadError.set(false);
    this.handoverQueryService.getQueries(id).subscribe({
      next: (data) => {
        this.queries.set(data);
        this.queriesChange.emit(data);
        this.isLoading.set(false);
        onDone?.();
      },
      error: (err) => {
        this.isLoading.set(false);
        this.loadError.set(true);
        this.message.set(this.extractErrorMessage(err, HANDOVER_MESSAGES.QUERY_LOAD_ERROR));
        this.isError.set(true);
        onDone?.();
      },
    });
  }

  fieldSectionLabel(query: HandoverQueryDto): string {
    if (query.fieldSection === HandoverQueryFieldSection.Commitment && query.referenceId) {
      const commitment = this.commitments().find((c) => c.commitmentId === query.referenceId);
      return commitment ? `Commitment: ${commitment.title}` : HANDOVER_QUERY_FIELD_LABELS[query.fieldSection];
    }
    return HANDOVER_QUERY_FIELD_LABELS[query.fieldSection] ?? query.fieldSection;
  }

  jumpToField(query: HandoverQueryDto): void {
    const baseId = HANDOVER_SECTION_FRAGMENT_IDS[query.fieldSection];
    if (!baseId) return;

    const fragmentId =
      query.fieldSection === HandoverQueryFieldSection.Commitment && query.referenceId
        ? `${baseId}-${query.referenceId}`
        : baseId;

    const element = document.getElementById(fragmentId);
    if (element) {
      element.scrollIntoView({ behavior: 'smooth', block: 'center' });
      element.classList.add('field-highlight');
      setTimeout(() => element.classList.remove('field-highlight'), 2000);
    }
  }

  getInitialQuestion(query: HandoverQueryDto): string {
    return query.messages?.[0]?.messageText ?? '';
  }

  getReplies(query: HandoverQueryDto): HandoverQueryMessageDto[] {
    return query.messages?.slice(1) ?? [];
  }

  getRelativeTime(dateStr: string | Date | undefined): string {
    if (!dateStr) return '';
    const date = new Date(dateStr);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffSecs = Math.floor(diffMs / 1000);
    const diffMins = Math.floor(diffSecs / 60);
    const diffHours = Math.floor(diffMins / 60);
    const diffDays = Math.floor(diffHours / 24);

    if (diffSecs < 60) return 'just now';
    if (diffMins < 60) return `${diffMins}m ago`;
    if (diffHours < 24) return `${diffHours}h ago`;
    if (diffDays === 1) return 'yesterday';
    if (diffDays < 7) return `${diffDays}d ago`;
    return date.toLocaleDateString(undefined, { month: 'short', day: 'numeric' });
  }

  isExpanded(queryId: number): boolean {
    return this.expandedQueries()[queryId] ?? false;
  }

  hasNoDiscussion(query: HandoverQueryDto): boolean {
    return query.isResolved && this.getReplies(query).length === 0;
  }

  getDiscussionTitle(query: HandoverQueryDto): string {
    if (this.hasNoDiscussion(query)) {
      return 'Nothing Discussed';
    }
    return this.isExpanded(query.id) ? 'Close Discussion' : 'Open Discussion';
  }

  toggleExpand(queryId: number, expanded: boolean): void {
    this.expandedQueries.update((map) => ({ ...map, [queryId]: expanded }));
  }

  messageDraft(queryId: number): string {
    return this.messageDrafts()[queryId] ?? '';
  }

  updateMessageDraft(queryId: number, value: string): void {
    this.messageDrafts.update((drafts) => ({ ...drafts, [queryId]: value }));
  }

  canSubmitMessage(): boolean {
    return this.isPreSales() || this.isProjectManagement();
  }

  isSubmitting(queryId: number): boolean {
    return this.submittingMessages().has(queryId);
  }

  onTextareaKeyDown(event: KeyboardEvent, query: HandoverQueryDto): void {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.submitMessage(query);
    }
  }

  submitMessage(query: HandoverQueryDto): void {
    const id = this.engagementId();
    const text = this.messageDraft(query.id).trim();
    if (!text || this.isSubmitting(query.id)) return;

    this.submittingMessages.update((s) => new Set(s).add(query.id));

    this.handoverQueryService.addMessage(id, query.id, { messageText: text }).subscribe({
      next: (updated) => {
        this.queries.update((qs) => {
          const next = qs.map((q) => (q.id === updated.id ? updated : q));
          this.queriesChange.emit(next);
          return next;
        });
        this.updateMessageDraft(query.id, '');
      },
      error: (err) => this.showMessage(this.extractErrorMessage(err, HANDOVER_MESSAGES.QUERY_MESSAGE_SUBMIT_ERROR), true),
      complete: () => {
        this.submittingMessages.update((s) => {
          const next = new Set(s);
          next.delete(query.id);
          return next;
        });
      },
    });
  }

  toggleResolve(query: HandoverQueryDto): void {
    if (this.pendingResolutionToggles().has(query.id)) return;

    const id = this.engagementId();
    this.pendingResolutionToggles.update((s) => new Set(s).add(query.id));

    this.handoverQueryService.toggleQueryResolution(id, query.id).subscribe({
      next: (updated) => {
        this.queries.update((qs) => {
          const next = qs.map((q) => (q.id === updated.id ? updated : q));
          this.queriesChange.emit(next);
          return next;
        });
      },
      error: (err) => this.showMessage(this.extractErrorMessage(err, HANDOVER_MESSAGES.QUERY_UPDATE_ERROR), true),
      complete: () => {
        this.pendingResolutionToggles.update((s) => {
          const next = new Set(s);
          next.delete(query.id);
          return next;
        });
      },
    });
  }

  openDeleteConfirm(query: HandoverQueryDto): void {
    this.pendingDeleteQuery.set(query);
  }

  cancelDeleteQuery(): void {
    this.pendingDeleteQuery.set(null);
  }

  confirmDeleteQuery(query: HandoverQueryDto): void {
    const id = this.engagementId();
    this.isDeletingQuery.set(true);

    this.handoverQueryService.deleteQuery(id, query.id).subscribe({
      next: () => {
        this.queries.update((qs) => {
          const next = qs.filter((q) => q.id !== query.id);
          this.queriesChange.emit(next);
          return next;
        });
        this.pendingDeleteQuery.set(null);
        this.isDeletingQuery.set(false);
      },
      error: (err) => {
        this.isDeletingQuery.set(false);
        this.showMessage(this.extractErrorMessage(err, HANDOVER_MESSAGES.QUERY_DELETE_ERROR), true);
      },
    });
  }

  openRaiseQueryDialog(): void {
    const id = this.engagementId();
    const dialogRef = this.dialogService.open<RaiseHandoverQueryDialogComponent, RaiseHandoverQueryDialogData, boolean>(
      RaiseHandoverQueryDialogComponent,
      {
        data: {
          engagementId: id,
          commitments: this.commitments(),
        },
      }
    );

    dialogRef.afterClosed().subscribe((result) => {
      if (!result) return;

      this.loadQueries(id, () => this.showMessage(HANDOVER_MESSAGES.QUERY_RAISE_SUCCESS, false));
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

  private compareQueriesLatestFirst(a: HandoverQueryDto, b: HandoverQueryDto): number {
    const timeA = this.parseDate(a.raisedAt);
    const timeB = this.parseDate(b.raisedAt);
    if (timeA !== timeB) {
      return timeB - timeA;
    }
    return b.id - a.id;
  }

  private parseDate(dateStr?: string | Date): number {
    if (!dateStr) return 0;
    const time = new Date(dateStr).getTime();
    return isNaN(time) ? 0 : time;
  }
}
