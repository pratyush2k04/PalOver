import { Component, inject, input, output, signal, computed, effect, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { StatusBadgeComponent, StatusBadgeTone } from '@common/components/status-badge/status-badge';
import { ButtonComponent } from '@common/components/button/button';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { DialogService } from '@common/services/dialog/dialog.service';
import { AuthService } from 'src/app/services/auth';
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
    CommonModule,
    FormsModule,
    InlineSVGModule,
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
  private authService = inject(AuthService);
  private dialog = inject(DialogService);
  private destroyRef = inject(DestroyRef);

  ArchitectureStatus = ArchitectureStatus;

  validation = signal<ArchitectureValidationDto | null>(null);
  isLoading = signal<boolean>(true);
  selectedStatus = signal<ArchitectureStatus>(ArchitectureStatus.StillValid);
  rationale = signal<string>('');
  rationaleError = signal<string>('');

  isTechLead = computed(() => this.authService.hasRole(TECHNICAL_READINESS_ROLES.TECH_LEAD));
  features = computed(() => this.validation()?.features ?? []);
  committedTimelineWeeks = computed(() => this.validation()?.committedTimelineWeeks ?? 0);
  totalEstimatedWeeks = computed(() => this.validation()?.totalEstimatedWeeks ?? 0);
  sizingDeltaWeeks = computed(() => this.validation()?.sizingDeltaWeeks ?? 0);
  sizingDeltaPercentage = computed(() => this.validation()?.sizingDeltaPercentage ?? 0);
  isRiskRaised = computed(() => this.validation()?.isRiskRaised ?? false);
  riskSummary = computed(() => this.validation()?.riskSummary ?? null);
  isModified = computed(() => +this.selectedStatus() === ArchitectureStatus.Modified);

  statusBadgeLabel = computed(() =>
    +this.selectedStatus() === ArchitectureStatus.StillValid ? 'Complete' : 'In Progress'
  );

  statusBadgeTone = computed<StatusBadgeTone>(() =>
    +this.selectedStatus() === ArchitectureStatus.StillValid ? 'success' : 'info'
  );

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

  loadValidation(id: number = this.engagementId()): void {
    this.isLoading.set(true);
    this.service.getValidation(id).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (res: ArchitectureValidationDto) => {
        this.validation.set(res);
        this.selectedStatus.set(res.status);
        this.rationale.set(res.rationale ?? '');
        this.validationUpdated.emit(res);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  onStatusSelect(status: ArchitectureStatus): void {
    if (!this.isTechLead()) return;
    this.selectedStatus.set(status);
    this.rationaleError.set('');
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
      DiscoveryFeatureDialogComponent
    );

    dialogRef.afterClosed().subscribe((result) => {
      if (!result) return;
      this.service.addFeatureItem(this.engagementId(), result).pipe(
        takeUntilDestroyed(this.destroyRef)
      ).subscribe({
        next: () => this.loadValidation()
      });
    });
  }

  onEditFeature(feature: DiscoveryFeatureItemDto): void {
    if (!this.isTechLead()) return;

    const dialogRef = this.dialog.open<DiscoveryFeatureDialogComponent, DiscoveryFeatureDialogData, UpdateDiscoveryFeatureItemRequest>(
      DiscoveryFeatureDialogComponent,
      { data: { feature } }
    );

    dialogRef.afterClosed().subscribe((result) => {
      if (!result) return;
      this.service.updateFeatureItem(feature.id, result).pipe(
        takeUntilDestroyed(this.destroyRef)
      ).subscribe({
        next: () => this.loadValidation()
      });
    });
  }

  onDeleteFeature(featureId: number): void {
    if (!this.isTechLead()) return;

    if (confirm('Are you sure you want to delete this discovery feature item?')) {
      this.service.deleteFeatureItem(featureId).pipe(
        takeUntilDestroyed(this.destroyRef)
      ).subscribe({
        next: () => this.loadValidation()
      });
    }
  }
}
