import { Component, OnInit, inject, signal, computed, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { AssumptionService } from '../../../../services/assumption.service';
import { EngagementService } from '../../../../services/engagement.service';
import { AssumptionDto, AssumptionStatus } from '../../../../models/assumption.model';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { ButtonComponent } from '@common/components/button/button';
import { StatusBadgeComponent, StatusBadgeTone } from '@common/components/status-badge/status-badge';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';

import { DialogService } from '@common/services/dialog/dialog.service';
import { ToastService } from '@common/services/toast/toast';
import { AssumptionFormDialogComponent, AssumptionFormDialogResult } from './dialogs/assumption-form-dialog/assumption-form-dialog.component';
import { ASSUMPTION_MESSAGES } from '@shared/constants/app.constants';
import { AuthService } from '@shared/services/auth';

import { InlineSVGModule } from 'ng-inline-svg-2';
import { TooltipDirective } from '@common/directives/tooltip.directive';

@Component({
  selector: 'app-assumptions',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    LoadingSpinnerComponent,
    ButtonComponent,
    StatusBadgeComponent,
    EmptyStateComponent,
    InlineSVGModule,
    TooltipDirective
  ],
  templateUrl: './assumptions.component.html',
  styleUrl: './assumptions.component.scss'
})
export class AssumptionsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private assumptionService = inject(AssumptionService);
  private toastService = inject(ToastService);
  private dialogService = inject(DialogService);
  private authService = inject(AuthService);
  private engagementService = inject(EngagementService);
  @Output() metricsChanged = new EventEmitter<{ invalidCount: number, unvalidatedCount: number, totalCount: number }>();

  engagementId = signal<number>(0);
  assumptions = signal<AssumptionDto[]>([]);
  unvalidatedCount = computed(() => this.assumptions().filter(a => a.status === AssumptionStatus.Unvalidated).length);
  invalidCount = computed(() => this.assumptions().filter(a => a.status === AssumptionStatus.Invalid).length);
  isLoading = signal<boolean>(true);
  readonly canAdd = computed(() => !this.engagementService.isSignedOff());

  AssumptionStatus = AssumptionStatus;

  private handleError(err: any, fallbackMessage: string) {
    console.error(err);
    const backendMsg = err?.error?.message || err?.error?.detail || err?.error?.title;
    this.toastService.error(backendMsg || fallbackMessage);
  }

  ngOnInit() {
    this.route.parent?.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.engagementId.set(+id);
        this.loadAssumptions();
      }
    });
  }

  loadAssumptions(skipLoader = false, onDone?: () => void) {
    if (skipLoader) {
      this.isLoading.set(true);
    }
    this.assumptionService.getAssumptions(this.engagementId(), skipLoader).subscribe({
      next: (data) => {
        this.assumptions.set(data);
        this.metricsChanged.emit({
          invalidCount: this.invalidCount(),
          unvalidatedCount: this.unvalidatedCount(),
          totalCount: data.length
        });
        this.isLoading.set(false);
        onDone?.();
      },
      error: (err) => {
        this.handleError(err, ASSUMPTION_MESSAGES.LOAD_ERROR);
        this.isLoading.set(false);
        onDone?.();
      }
    });
  }

  getStatusTone(status: AssumptionStatus): StatusBadgeTone {
    switch (status) {
      case AssumptionStatus.Unvalidated: return 'warning';
      case AssumptionStatus.Validated: return 'success';
      case AssumptionStatus.Invalid: return 'danger';
      default: return 'neutral';
    }
  }

  getStatusLabel(status: AssumptionStatus): string {
    switch (status) {
      case AssumptionStatus.Unvalidated: return 'Unvalidated';
      case AssumptionStatus.Validated: return 'Validated';
      case AssumptionStatus.Invalid: return 'Invalid';
      default: return 'Unknown';
    }
  }

  canEdit(assumption: AssumptionDto): boolean {
    return !this.engagementService.isSignedOff()
      && assumption.status === AssumptionStatus.Unvalidated
      && Number(assumption.ownerId) === Number(this.authService.getUserId());
  }

  isPm(): boolean {
    return this.authService.hasRole('Project Management');
  }

  openAssumptionDialog(assumption?: AssumptionDto) {
    const dialogRef = this.dialogService.open<AssumptionFormDialogComponent, any, AssumptionFormDialogResult>(
      AssumptionFormDialogComponent,
      {
        data: { assumption },
        disableClose: true
      }
    );

    dialogRef.closed$.subscribe((result: AssumptionFormDialogResult | undefined) => {
      if (!result) return;

      if (assumption) {
        this.assumptionService.updateAssumption(this.engagementId(), assumption.id, result).subscribe({
          next: () => {
            this.loadAssumptions(true, () => this.toastService.success(ASSUMPTION_MESSAGES.UPDATE_SUCCESS));
          },
          error: (err) => this.handleError(err, ASSUMPTION_MESSAGES.UPDATE_ERROR)
        });
      } else {
        this.assumptionService.createAssumption(this.engagementId(), result).subscribe({
          next: () => {
            this.loadAssumptions(true, () => this.toastService.success(ASSUMPTION_MESSAGES.CREATE_SUCCESS));
          },
          error: (err) => this.handleError(err, ASSUMPTION_MESSAGES.CREATE_ERROR)
        });
      }
    });
  }

  validateAssumption(id: number, status: AssumptionStatus) {
    this.assumptionService.validateAssumption(this.engagementId(), id, { status }).subscribe({
      next: () => {
        this.loadAssumptions(true, () =>
          this.toastService.success(ASSUMPTION_MESSAGES.UPDATE_STATUS_SUCCESS(this.getStatusLabel(status)))
        );
      },
      error: (err) => {
        this.handleError(err, ASSUMPTION_MESSAGES.UPDATE_STATUS_ERROR);
      }
    });
  }

  markInvalid(id: number) {
    this.validateAssumption(id, AssumptionStatus.Invalid);
  }
}
