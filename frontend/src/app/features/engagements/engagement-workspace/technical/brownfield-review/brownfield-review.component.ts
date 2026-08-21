import { Component, inject, input, output, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { TooltipDirective } from '@common/directives/tooltip.directive';
import { ToastService } from '@common/services/toast/toast';
import { ConfirmDialogComponent } from '@common/components/confirm-dialog/confirm-dialog';
import { StatusBadgeComponent } from '@common/components/status-badge/status-badge';
import { ButtonComponent } from '@common/components/button/button';
import { DialogService } from '@common/services/dialog/dialog.service';
import { AuthService } from '@shared/services/auth';
import { TECHNICAL_READINESS_ROLES } from '@shared/constants/technical-readiness.constants';
import { BrownfieldFindingDto, BrownfieldReadinessResponse } from '@features/engagements/models/brownfield-review/brownfield-review.dtos';
import { BrownfieldSeverity, ClientDecision } from '@features/engagements/models/brownfield-review/brownfield-review.enums';
import { BrownfieldReviewService } from '@features/engagements/services/brownfield-review.service';
import {
  AddFindingDialogComponent,
  AddFindingDialogData,
  AddFindingDialogResult
} from './dialogs/add-finding-dialog/add-finding-dialog.component';

@Component({
  selector: 'app-brownfield-review',
  standalone: true,
  imports: [
    CommonModule,
    InlineSVGModule,
    TooltipDirective,
    ConfirmDialogComponent,
    StatusBadgeComponent,
    ButtonComponent
  ],
  templateUrl: './brownfield-review.component.html',
  styleUrl: './brownfield-review.component.scss'
})
export class BrownfieldReviewComponent {
  engagementId = input.required<number>();
  readinessUpdated = output<BrownfieldReadinessResponse>();

  private brownfieldService = inject(BrownfieldReviewService);
  private toastService = inject(ToastService);
  private authService = inject(AuthService);
  private dialog = inject(DialogService);

  readinessResponse = signal<BrownfieldReadinessResponse | null>(null);
  isLoading = signal<boolean>(true);
  findingToDelete = signal<BrownfieldFindingDto | null>(null);
  isDeleting = signal<boolean>(false);

  isTechLead = computed(() => this.authService.hasRole(TECHNICAL_READINESS_ROLES.TECH_LEAD));
  findings = computed(() => this.readinessResponse()?.items ?? []);
  totalCount = computed(() => this.readinessResponse()?.totalCount ?? 0);
  highSeverityPendingCount = computed(() => this.readinessResponse()?.highSeverityPendingCount ?? 0);
  isTechnicallyReady = computed(() => this.readinessResponse()?.isTechnicallyReady ?? true);

  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.loadFindings(id);
      }
    });
  }

  loadFindings(id: number = this.engagementId()): void {
    this.isLoading.set(true);
    this.brownfieldService.getFindings(id).subscribe({
      next: (res: BrownfieldReadinessResponse) => {
        this.readinessResponse.set(res);
        this.readinessUpdated.emit(res);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  onAddFinding(): void {
    const dialogRef = this.dialog.open<AddFindingDialogComponent, AddFindingDialogData, AddFindingDialogResult>(
      AddFindingDialogComponent,
      { data: { engagementId: this.engagementId() }, disableClose: true }
    );

    dialogRef.afterClosed().subscribe((result: AddFindingDialogResult | undefined) => {
      if (result) {
        this.brownfieldService.createFinding(this.engagementId(), result).subscribe({
          next: () => this.loadFindings()
        });
      }
    });
  }

  onUpdateDecision(findingId: number, decision: ClientDecision): void {
    this.brownfieldService.updateDecision(findingId, decision).subscribe({
      next: () => this.loadFindings()
    });
  }

  onDeleteFinding(finding: BrownfieldFindingDto): void {
    if (!this.isTechLead()) return;
    this.findingToDelete.set(finding);
  }

  cancelDeleteFinding(): void {
    this.findingToDelete.set(null);
  }

  confirmDeleteFinding(): void {
    const finding = this.findingToDelete();
    if (!finding) return;

    this.isDeleting.set(true);
    this.brownfieldService.deleteFinding(finding.id).subscribe({
      next: () => {
        this.toastService.success(`Finding "${finding.title}" deleted successfully`);
        this.isDeleting.set(false);
        this.findingToDelete.set(null);
        this.loadFindings();
      },
      error: (err: unknown) => {
        this.toastService.error(`Failed to delete finding "${finding.title}"`);
        console.error('API delete finding failed', err);
        this.isDeleting.set(false);
      }
    });
  }

  getSeverityLabel(severity: BrownfieldSeverity): string {
    switch (+severity) {
      case BrownfieldSeverity.High: return 'High';
      case BrownfieldSeverity.Medium: return 'Medium';
      case BrownfieldSeverity.Low: return 'Low';
      default: return 'Low';
    }
  }

  getSeverityTone(severity: BrownfieldSeverity): 'danger' | 'warning' | 'neutral' {
    switch (+severity) {
      case BrownfieldSeverity.High: return 'danger';
      case BrownfieldSeverity.Medium: return 'warning';
      case BrownfieldSeverity.Low: return 'neutral';
      default: return 'neutral';
    }
  }
}
