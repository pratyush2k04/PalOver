import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { Component, inject, input, output, signal, computed, effect, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { TooltipDirective } from '@common/directives/tooltip.directive';
import { ToastService } from '@common/services/toast/toast';
import { ConfirmDialogComponent } from '@common/components/confirm-dialog/confirm-dialog';
import { StatusBadgeComponent, StatusBadgeTone } from '@common/components/status-badge/status-badge';
import { ButtonComponent } from '@common/components/button/button';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { DialogService } from '@common/services/dialog/dialog.service';
import { AuthService } from '@shared/services/auth';
import { EngagementService } from '@features/engagements/services/engagement.service';
import { TECHNICAL_READINESS_ROLES } from '@shared/constants/technical-readiness.constants';
import {
  ArchitectureValidationDto,
  DiscoveryFeatureItemDto
} from '@features/engagements/models/architecture-validation/architecture-validation.dtos';
import { ArchitectureStatus } from '@features/engagements/models/architecture-validation/architecture-validation.enums';
import {
  CreateDiscoveryFeatureItemRequest,
  UpdateDiscoveryFeatureItemRequest
} from '@features/engagements/models/architecture-validation/architecture-validation.requests';
import { ArchitectureValidationService } from '@features/engagements/services/architecture-validation.service';
import { DiscoveryFeatureDialogComponent, DiscoveryFeatureDialogData } from './dialogs/discovery-feature-dialog/discovery-feature-dialog.component';

@Component({
  selector: 'app-architecture-validation',
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    CommonModule,
    FormsModule,
    InlineSVGModule,
    TooltipDirective,
    ConfirmDialogComponent,
    StatusBadgeComponent,
    ButtonComponent,
    TextareaComponent
  ],
  templateUrl: './architecture-validation.component.html',
  styleUrl: './architecture-validation.component.scss'
})
export class ArchitectureValidationComponent {
  engagementId = input.required<number>();
  validationUpdated = output<ArchitectureValidationDto>();

  private service = inject(ArchitectureValidationService);
  private toastService = inject(ToastService);
  private authService = inject(AuthService);
  private engagementService = inject(EngagementService);
  private dialog = inject(DialogService);
  private destroyRef = inject(DestroyRef);
  private route = inject(ActivatedRoute);

  ArchitectureStatus = ArchitectureStatus;

  validation = signal<ArchitectureValidationDto | null>(null);
  isLoading = signal<boolean>(false);
  featureToDelete = signal<DiscoveryFeatureItemDto | null>(null);
  isDeleting = signal<boolean>(false);
  selectedStatus = signal<ArchitectureStatus>(ArchitectureStatus.StillValid);
  rationale = signal<string>('');
  rationaleError = signal<string>('');

  isTechLead = computed(() => this.authService.hasRole(TECHNICAL_READINESS_ROLES.TECH_LEAD));
  canManage = computed(() => this.isTechLead() && !this.engagementService.isSignedOff());
  features = computed(() => this.validation()?.features ?? []);
  committedTimelineWeeks = computed(() => this.validation()?.committedTimelineWeeks ?? 0);
  totalEstimatedWeeks = computed(() => this.validation()?.totalEstimatedWeeks ?? 0);
  sizingDeltaWeeks = computed(() => this.validation()?.sizingDeltaWeeks ?? 0);
  sizingDeltaPercentage = computed(() => this.validation()?.sizingDeltaPercentage ?? 0);
  isRiskRaised = computed(() => this.validation()?.isRiskRaised ?? false);
  riskSummary = computed(() => this.validation()?.riskSummary ?? null);
  isModified = computed(() => +this.selectedStatus() === ArchitectureStatus.Modified);

  deltaBadgeTone = computed<StatusBadgeTone>(() => {
    if (this.isRiskRaised()) return 'danger';
    if (this.sizingDeltaWeeks() <= 0) return 'success';
    return 'neutral';
  });

  hasStatusChanged = computed(() => {
    const current = this.validation();
    if (!current) return false;
    const statusChanged = +this.selectedStatus() !== +current.status;
    const rationaleChanged = (this.rationale().trim() || '') !== (current.rationale?.trim() || '');
    return statusChanged || rationaleChanged;
  });

  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.loadValidation(id);
      }
    });
  }

  loadValidation(id: number = this.engagementId(), skipLoader = false): void {
    if (skipLoader) {
      this.isLoading.set(true);
    }
    this.service.getValidation(id, skipLoader).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (res: ArchitectureValidationDto) => {
        this.validation.set(res);
        this.selectedStatus.set(res.status);
        this.rationale.set(res.rationale ?? '');
        this.validationUpdated.emit(res);
        if (skipLoader) {
          this.isLoading.set(false);
        }

        if (this.isTechLead() && this.route.snapshot.queryParamMap.get('action') === 'create') {
          this.onAddFeature();
        }
      },
      error: () => {
        if (skipLoader) {
          this.isLoading.set(false);
        }
      }
    });
  }

  onStatusSelect(status: ArchitectureStatus): void {
    if (!this.isTechLead()) return;
    this.selectedStatus.set(status);
    this.rationaleError.set('');

    if (+status === ArchitectureStatus.StillValid) {
      this.rationale.set('');
      if (this.hasStatusChanged()) {
        this.onSaveStatus();
      }
    }
  }

  onRationaleChange(val: string): void {
    this.rationale.set(val);
    this.rationaleError.set('');
  }

  onSaveStatus(): void {
    if (!this.isTechLead()) return;

    const statusVal = +this.selectedStatus();
    const rationaleVal = this.rationale().trim();

    if (statusVal === ArchitectureStatus.Modified && !rationaleVal) {
      this.rationaleError.set('Rationale is mandatory when architecture is modified.');
      return;
    }

    if (rationaleVal.length > 500) {
      this.rationaleError.set('Rationale cannot exceed 500 characters.');
      return;
    }

    this.service.saveValidation(this.engagementId(), {
      status: statusVal,
      rationale: statusVal === ArchitectureStatus.Modified ? rationaleVal : undefined
    }).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (res: ArchitectureValidationDto) => {
        this.validation.set(res);
        this.selectedStatus.set(res.status);
        this.rationale.set(res.rationale ?? '');
        this.validationUpdated.emit(res);
      }
    });
  }

  onAddFeature(): void {
    if (!this.isTechLead()) return;

    const dialogRef = this.dialog.open<DiscoveryFeatureDialogComponent, DiscoveryFeatureDialogData, CreateDiscoveryFeatureItemRequest>(
      DiscoveryFeatureDialogComponent,
      { disableClose: true }
    );

    dialogRef.afterClosed().subscribe((result) => {
      if (!result) return;
      this.service.addFeatureItem(this.engagementId(), result).pipe(
        takeUntilDestroyed(this.destroyRef)
      ).subscribe({
        next: () => this.loadValidation(this.engagementId(), true)
      });
    });
  }

  onEditFeature(feature: DiscoveryFeatureItemDto): void {
    if (!this.isTechLead()) return;

    const dialogRef = this.dialog.open<DiscoveryFeatureDialogComponent, DiscoveryFeatureDialogData, UpdateDiscoveryFeatureItemRequest>(
      DiscoveryFeatureDialogComponent,
      { data: { feature }, disableClose: true }
    );

    dialogRef.afterClosed().subscribe((result) => {
      if (!result) return;
      this.service.updateFeatureItem(feature.id, result).pipe(
        takeUntilDestroyed(this.destroyRef)
      ).subscribe({
        next: () => this.loadValidation(this.engagementId(), true)
      });
    });
  }

  onDeleteFeature(feature: DiscoveryFeatureItemDto): void {
    if (!this.isTechLead()) return;
    this.featureToDelete.set(feature);
  }

  cancelDeleteFeature(): void {
    this.featureToDelete.set(null);
  }

  confirmDeleteFeature(): void {
    const feature = this.featureToDelete();
    if (!feature) return;

    this.isDeleting.set(true);
    this.service.deleteFeatureItem(feature.id).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: () => {
        this.toastService.success(`Feature "${feature.featureName}" deleted successfully`);
        this.isDeleting.set(false);
        this.featureToDelete.set(null);
        this.loadValidation(this.engagementId(), true);
      },
      error: (err) => {
        this.toastService.error(`Failed to delete feature "${feature.featureName}"`);
        console.error('API delete feature failed', err);
        this.isDeleting.set(false);
      }
    });
  }
}
