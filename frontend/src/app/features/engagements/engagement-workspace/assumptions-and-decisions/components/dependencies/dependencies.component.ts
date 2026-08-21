import { Component, OnInit, inject, signal, computed, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DependencyService } from '../../../../services/dependency.service';
import { DependencyDto, DependencyStatus } from '../../../../models/dependency.model';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { ButtonComponent } from '@common/components/button/button';
import { StatusBadgeComponent, StatusBadgeTone } from '@common/components/status-badge/status-badge';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import { DialogService } from '@common/services/dialog/dialog.service';
import { ToastService } from '@common/services/toast/toast';
import { DependencyFormDialogComponent, DependencyFormDialogResult } from './dialogs/dependency-form-dialog/dependency-form-dialog.component';
import { DEPENDENCY_MESSAGES } from '@shared/constants/app.constants';

@Component({
  selector: 'app-dependencies',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    LoadingSpinnerComponent,
    ButtonComponent,
    StatusBadgeComponent,
    EmptyStateComponent
  ],
  templateUrl: './dependencies.component.html',
  styleUrl: './dependencies.component.scss'
})
export class DependenciesComponent implements OnInit {
  @Input({ required: true }) engagementId!: number;
  
  private dependencyService = inject(DependencyService);
  private toastService = inject(ToastService);
  private dialogService = inject(DialogService);

  dependencies = signal<DependencyDto[]>([]);
  atRiskCount = computed(() => this.dependencies().filter(d => d.status === DependencyStatus.AtRisk).length);
  isLoading = signal<boolean>(true);

  DependencyStatus = DependencyStatus;

  private handleError(err: any, fallbackMessage: string) {
    console.error(err);
    const backendMsg = err?.error?.message || err?.error?.detail || err?.error?.title;
    this.toastService.error(backendMsg || fallbackMessage);
  }

  ngOnInit() {
    if (this.engagementId) {
      this.loadDependencies();
    }
  }

  loadDependencies() {
    this.isLoading.set(true);
    this.dependencyService.getDependencies(this.engagementId).subscribe({
      next: (data: DependencyDto[]) => {
        this.dependencies.set(data);
        this.isLoading.set(false);
      },
      error: (err: any) => {
        this.handleError(err, DEPENDENCY_MESSAGES.LOAD_ERROR);
        this.isLoading.set(false);
      }
    });
  }

  getStatusTone(status: DependencyStatus): StatusBadgeTone {
    switch (status) {
      case DependencyStatus.Open: return 'info';
      case DependencyStatus.Met: return 'success';
      case DependencyStatus.AtRisk: return 'warning';
      default: return 'neutral';
    }
  }

  getStatusLabel(status: DependencyStatus): string {
    switch (status) {
      case DependencyStatus.Open: return 'Open';
      case DependencyStatus.Met: return 'Met';
      case DependencyStatus.AtRisk: return 'At Risk';
      default: return 'Unknown';
    }
  }

  openDependencyDialog(dependency?: DependencyDto) {
    const dialogRef = this.dialogService.open<DependencyFormDialogComponent, any, DependencyFormDialogResult>(
      DependencyFormDialogComponent,
      { 
        data: { dependency, engagementId: this.engagementId },
        disableClose: true
      }
    );

    dialogRef.closed$.subscribe((result: DependencyFormDialogResult | undefined) => {
      if (!result) return;

      if (dependency) {
        const updateDto = {
          ...result,
          status: result.status ?? DependencyStatus.Open
        };
        this.dependencyService.updateDependency(this.engagementId, dependency.id, updateDto).subscribe({
          next: () => {
            this.toastService.success(DEPENDENCY_MESSAGES.UPDATE_SUCCESS);
            this.loadDependencies();
          },
          error: (err: any) => this.handleError(err, DEPENDENCY_MESSAGES.UPDATE_ERROR)
        });
      } else {
        this.dependencyService.createDependency(this.engagementId, result).subscribe({
          next: () => {
            this.toastService.success(DEPENDENCY_MESSAGES.CREATE_SUCCESS);
            this.loadDependencies();
          },
          error: (err: any) => this.handleError(err, DEPENDENCY_MESSAGES.CREATE_ERROR)
        });
      }
    });
  }

  markAsMet(dependency: DependencyDto) {
    const updateDto = {
      description: dependency.description,
      type: dependency.type,
      ownerId: dependency.ownerId,
      neededByDate: dependency.neededByDate,
      status: DependencyStatus.Met
    };

    this.dependencyService.updateDependency(this.engagementId, dependency.id, updateDto).subscribe({
      next: () => {
        this.toastService.success(DEPENDENCY_MESSAGES.UPDATE_SUCCESS);
        this.loadDependencies();
      },
      error: (err: any) => this.handleError(err, DEPENDENCY_MESSAGES.UPDATE_ERROR)
    });
  }
}
