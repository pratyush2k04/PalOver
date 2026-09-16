import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
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
import { AuthService } from '@shared/services/auth';
import { EngagementService } from '../../../../services/engagement.service';

import { InlineSVGModule } from 'ng-inline-svg-2';
import { TooltipDirective } from '@common/directives/tooltip.directive';

@Component({
  selector: 'app-dependencies',
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
  templateUrl: './dependencies.component.html',
  styleUrl: './dependencies.component.scss'
})
export class DependenciesComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private dependencyService = inject(DependencyService);
  private toastService = inject(ToastService);
  private dialogService = inject(DialogService);
  private authService = inject(AuthService);
  private engagementService = inject(EngagementService);

  engagementId = signal<number>(0);
  dependencies = signal<DependencyDto[]>([]);
  atRiskCount = computed(() => this.dependencies().filter(d => d.status === DependencyStatus.AtRisk).length);
  isLoading = signal<boolean>(false);
  readonly canAdd = computed(() => !this.engagementService.isSignedOff());

  DependencyStatus = DependencyStatus;

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
        this.loadDependencies();

        if (this.route.snapshot.queryParamMap.get('action') === 'create') {
          this.openDependencyDialog();
        }
      }
    });
  }

  loadDependencies(skipLoader = false, onDone?: () => void) {
    if (skipLoader) {
      this.isLoading.set(true);
    }
    this.dependencyService.getDependencies(this.engagementId(), skipLoader).subscribe({
      next: (data: DependencyDto[]) => {
        this.dependencies.set(data);
        if (skipLoader) {
          this.isLoading.set(false);
        }
        onDone?.();
      },
      error: (err: any) => {
        this.handleError(err, DEPENDENCY_MESSAGES.LOAD_ERROR);
        if (skipLoader) {
          this.isLoading.set(false);
        }
        onDone?.();
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
        data: { 
          dependency: dependency || null,
          engagementId: this.engagementId()
        },
        disableClose: true
      }
    );

    dialogRef.closed$.subscribe(result => {
      if (!result) return;

      if (dependency) {
        this.dependencyService.updateDependency(this.engagementId(), dependency.id, {
          ...result,
          status: result.status ?? dependency.status
        }).subscribe({
          next: () => {
            this.loadDependencies(true, () => this.toastService.success(DEPENDENCY_MESSAGES.UPDATE_SUCCESS));
          },
          error: (err: any) => this.handleError(err, DEPENDENCY_MESSAGES.UPDATE_ERROR)
        });
      } else {
        this.dependencyService.createDependency(this.engagementId(), result).subscribe({
          next: () => {
            this.loadDependencies(true, () => this.toastService.success(DEPENDENCY_MESSAGES.CREATE_SUCCESS));
          },
          error: (err: any) => this.handleError(err, DEPENDENCY_MESSAGES.CREATE_ERROR)
        });
      }
    });
  }

  canEdit(dependency: DependencyDto): boolean {
    return !this.engagementService.isSignedOff() 
      && dependency.status !== DependencyStatus.Met 
      && dependency.createdBy === String(this.authService.getUserId());
  }

  canMarkMet(dependency: DependencyDto): boolean {
    return !this.engagementService.isSignedOff() && (
      dependency.ownerUserId === this.authService.getUserId()
      || this.authService.hasRole('Project Management')
    );
  }

  markAsMet(dependency: DependencyDto) {
    const updateDto = {
      description: dependency.description,
      type: dependency.type,
      ownerId: dependency.ownerId,
      neededByDate: dependency.neededByDate,
      status: DependencyStatus.Met
    };

    this.dependencyService.updateDependency(this.engagementId(), dependency.id, updateDto).subscribe({
      next: () => {
        this.loadDependencies(true, () => this.toastService.success(DEPENDENCY_MESSAGES.UPDATE_SUCCESS));
      },
      error: (err: any) => this.handleError(err, DEPENDENCY_MESSAGES.UPDATE_ERROR)
    });
  }
}
