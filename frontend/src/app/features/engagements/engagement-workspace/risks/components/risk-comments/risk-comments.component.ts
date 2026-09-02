import { Component, OnInit, inject, input, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RiskCommentService } from '../../services/risk-comment.service';
import { EngagementService } from '../../../../services/engagement.service';
import { RiskComment, EngagementMember } from '../../models/risk.model';
import { AccordionComponent } from '@common/components/accordion/accordion';
import { AvatarComponent } from '@common/components/avatar/avatar';
import { ButtonComponent } from '@common/components/button/button';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { TooltipDirective } from '@common/directives/tooltip.directive';
import { ToastService } from '@common/services/toast/toast';
import { AuthService } from '@shared/services/auth';
import { RISK_COMMENT_MESSAGES, RISK_COMMENT_CONSTANTS } from '@shared/constants/app.constants';
import { InlineSVGModule } from 'ng-inline-svg-2';

@Component({
  selector: 'app-risk-comments',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    InlineSVGModule,
    AccordionComponent,
    AvatarComponent,
    ButtonComponent,
    EmptyStateComponent,
    LoadingSpinnerComponent,
    TextareaComponent,
    TooltipDirective
  ],
  templateUrl: './risk-comments.component.html',
  styleUrl: './risk-comments.component.scss'
})
export class RiskCommentsComponent implements OnInit {
  readonly engagementId = input.required<number>();
  readonly riskId = input.required<number>();

  readonly constants = RISK_COMMENT_CONSTANTS;

  private commentService = inject(RiskCommentService);
  private toastService = inject(ToastService);
  private engagementService = inject(EngagementService);
  private authService = inject(AuthService);

  readonly currentUser = this.authService.currentUser;
  readonly isSignedOff = computed(() => this.engagementService.isSignedOff());

  comments = signal<RiskComment[]>([]);
  members = signal<EngagementMember[]>([]);
  isLoading = signal<boolean>(true);
  isSubmitting = signal<boolean>(false);
  isExpanded = signal<boolean>(true);

  newCommentText = signal<string>('');
  selectedMentionIds = signal<number[]>([]);

  // Editing state
  editingCommentId = signal<number | null>(null);
  editingCommentText = signal<string>('');

  // Mention Popover State
  showMentionDropdown = signal<boolean>(false);
  mentionQuery = signal<string>('');
  mentionStartIndex = signal<number>(-1);

  filteredMembers = computed(() => {
    const q = this.mentionQuery().toLowerCase().trim();
    const list = this.members();
    if (!q) return list;
    return list.filter((m) => m.fullName.toLowerCase().includes(q));
  });

  ngOnInit(): void {
    if (this.engagementId() && this.riskId()) {
      this.loadComments();
      this.loadMembers();
    }
  }

  toggleExpanded(): void {
    this.isExpanded.update((v) => !v);
  }

  isOwnMessage(comment: RiskComment): boolean {
    if (comment.canEdit) {
      return true;
    }

    const currentId = this.currentUser()?.id;
    if (comment.authorId && currentId && Number(comment.authorId) === Number(currentId)) {
      return true;
    }

    const currentName = (this.currentUser()?.fullName || this.getTokenFullName()).trim().toLowerCase();
    const authorName = comment.authorName?.trim().toLowerCase();
    if (currentName && authorName && currentName === authorName) {
      return true;
    }

    return false;
  }

  private getTokenFullName(): string {
    try {
      const token = localStorage.getItem('token');
      if (token) {
        const payload = JSON.parse(atob(token.split('.')[1]));
        return payload.fullname || payload.fullName || payload.name || '';
      }
    } catch {}
    return '';
  }

  getRelativeTime(dateStr: string | Date): string {
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

  formatContent(content: string): string {
    if (!content) return '';
    let escaped = this.escapeHtml(content);

    const currentMembers = this.members();
    if (currentMembers && currentMembers.length > 0) {
      const sortedMembers = [...currentMembers].sort((a, b) => b.fullName.length - a.fullName.length);
      for (const member of sortedMembers) {
        const mentionStr = `@${this.escapeHtml(member.fullName)}`;
        if (escaped.includes(mentionStr)) {
          const replaceRegex = new RegExp(this.escapeRegExp(mentionStr), 'g');
          escaped = escaped.replace(replaceRegex, `<span class="mention-highlight">${mentionStr}</span>`);
        }
      }
    }

    escaped = escaped.replace(/(?<!<span class="mention-highlight">)(@[A-Za-z0-9_.-]+)/g, '<span class="mention-highlight">$1</span>');
    return escaped;
  }

  private escapeHtml(str: string): string {
    return str
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;')
      .replace(/'/g, '&#039;');
  }

  private escapeRegExp(str: string): string {
    return str.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
  }

  loadComments(): void {
    this.isLoading.set(true);
    this.commentService.getComments(this.engagementId(), this.riskId()).subscribe({
      next: (data) => {
        this.comments.set(data);
        this.isLoading.set(false);
        this.scrollToBottom();
      },
      error: (err) => {
        console.error(RISK_COMMENT_MESSAGES.LOAD_ERROR, err);
        this.toastService.error(RISK_COMMENT_MESSAGES.LOAD_ERROR);
        this.isLoading.set(false);
      }
    });
  }

  loadMembers(): void {
    this.commentService.getEngagementMembers(this.engagementId()).subscribe({
      next: (data) => this.members.set(data),
      error: (err) => console.error(RISK_COMMENT_MESSAGES.MEMBERS_LOAD_ERROR, err)
    });
  }

  onInputTextChange(val: string): void {
    const oldText = this.newCommentText();
    let updatedVal = val;

    if (val.length < oldText.length) {
      updatedVal = this.handleMentionDeletion(oldText, val);
    }

    this.newCommentText.set(updatedVal);
    this.checkMentionTrigger(updatedVal);
  }

  private handleMentionDeletion(oldText: string, newText: string): string {
    if (newText.length >= oldText.length) {
      return newText;
    }

    let resultText = newText;
    const currentMembers = this.members();

    for (const member of currentMembers) {
      const fullMentionWithSpace = `@${member.fullName} `;
      const fullMentionWithoutSpace = `@${member.fullName}`;

      if (oldText.includes(fullMentionWithSpace) && !newText.includes(fullMentionWithSpace)) {
        const mentionIndex = oldText.indexOf(fullMentionWithSpace);
        resultText = oldText.substring(0, mentionIndex) + oldText.substring(mentionIndex + fullMentionWithSpace.length);
        this.selectedMentionIds.update((ids) => ids.filter((id) => id !== member.id));
        break;
      } else if (oldText.includes(fullMentionWithoutSpace) && !newText.includes(fullMentionWithoutSpace)) {
        const mentionIndex = oldText.indexOf(fullMentionWithoutSpace);
        const hasTrailingSpace =
          oldText.substring(mentionIndex + fullMentionWithoutSpace.length, mentionIndex + fullMentionWithSpace.length) === ' ';
        const removeLength = hasTrailingSpace ? fullMentionWithSpace.length : fullMentionWithoutSpace.length;
        resultText = oldText.substring(0, mentionIndex) + oldText.substring(mentionIndex + removeLength);
        this.selectedMentionIds.update((ids) => ids.filter((id) => id !== member.id));
        break;
      }
    }

    return resultText;
  }

  private checkMentionTrigger(text: string): void {
    const lastAtIndex = text.lastIndexOf('@');

    if (lastAtIndex !== -1) {
      const queryCandidate = text.substring(lastAtIndex + 1);
      if (!queryCandidate.includes(' ') && !queryCandidate.includes('\n')) {
        this.mentionStartIndex.set(lastAtIndex);
        this.mentionQuery.set(queryCandidate);
        this.showMentionDropdown.set(true);
        return;
      }
    }

    this.showMentionDropdown.set(false);
    this.mentionStartIndex.set(-1);
  }

  selectMention(member: EngagementMember): void {
    const text = this.newCommentText();
    const startIndex = this.mentionStartIndex();

    if (startIndex !== -1) {
      const beforeAt = text.substring(0, startIndex);
      const inserted = `@${member.fullName} `;
      const newText = beforeAt + inserted;

      this.newCommentText.set(newText);

      if (!this.selectedMentionIds().includes(member.id)) {
        this.selectedMentionIds.update((ids) => [...ids, member.id]);
      }

      this.showMentionDropdown.set(false);
      this.mentionStartIndex.set(-1);
    }
  }

  onTextareaKeyDown(event: KeyboardEvent): void {
    if (event.key === 'Enter' && !event.shiftKey) {
      if (this.showMentionDropdown() && this.filteredMembers().length > 0) {
        event.preventDefault();
        this.selectMention(this.filteredMembers()[0]);
        return;
      }
      event.preventDefault();
      this.submitComment();
    }
  }

  onEditKeyDown(event: KeyboardEvent, commentId: number): void {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.saveEdit(commentId);
    } else if (event.key === 'Escape') {
      event.preventDefault();
      this.cancelEditing();
    }
  }

  submitComment(): void {
    const content = this.newCommentText().trim();
    if (!content) return;

    if (content.length > RISK_COMMENT_CONSTANTS.MAX_LENGTH) {
      this.toastService.error(RISK_COMMENT_CONSTANTS.TEXT_EXCEED_ERROR);
      return;
    }

    this.isSubmitting.set(true);
    this.commentService.addComment(this.engagementId(), this.riskId(), {
      content,
      mentionedUserIds: this.selectedMentionIds()
    }).subscribe({
      next: (created) => {
        this.comments.update((list) => [...list, created]);
        this.newCommentText.set('');
        this.selectedMentionIds.set([]);
        this.showMentionDropdown.set(false);
        this.toastService.success(RISK_COMMENT_MESSAGES.ADD_SUCCESS);
        this.isSubmitting.set(false);
        this.scrollToBottom();
      },
      error: (err) => {
        console.error(err);
        this.toastService.error(RISK_COMMENT_MESSAGES.ADD_ERROR);
        this.isSubmitting.set(false);
      }
    });
  }

  private scrollToBottom(): void {
    setTimeout(() => {
      const container = document.querySelector('.comments-list');
      if (container) {
        container.scrollTop = container.scrollHeight;
      }
    }, 50);
  }

  startEditing(comment: RiskComment): void {
    if (!comment.canEdit) {
      this.toastService.error(RISK_COMMENT_MESSAGES.EDIT_WINDOW_EXPIRED);
      return;
    }
    this.editingCommentId.set(comment.id);
    this.editingCommentText.set(comment.content);
  }

  cancelEditing(): void {
    this.editingCommentId.set(null);
    this.editingCommentText.set('');
  }

  saveEdit(commentId: number): void {
    const content = this.editingCommentText().trim();
    if (!content) return;

    if (content.length > RISK_COMMENT_CONSTANTS.MAX_LENGTH) {
      this.toastService.error(RISK_COMMENT_CONSTANTS.TEXT_EXCEED_ERROR);
      return;
    }

    this.commentService.updateComment(this.engagementId(), this.riskId(), commentId, { content }).subscribe({
      next: (updated) => {
        this.comments.update((list) => list.map((c) => (c.id === commentId ? updated : c)));
        this.cancelEditing();
        this.toastService.success(RISK_COMMENT_MESSAGES.UPDATE_SUCCESS);
      },
      error: (err) => {
        console.error(err);
        const errorMsg = err?.error?.message || RISK_COMMENT_MESSAGES.UPDATE_ERROR;
        this.toastService.error(errorMsg);
      }
    });
  }
}
