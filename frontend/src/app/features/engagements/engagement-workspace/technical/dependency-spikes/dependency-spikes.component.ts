import { Component, OnInit, inject, input, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { TooltipDirective } from '@common/directives/tooltip.directive';
import { StatusBadgeComponent, StatusBadgeTone } from '@common/components/status-badge/status-badge';
import { ButtonComponent } from '@common/components/button/button';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import { ConfirmDialogComponent } from '@common/components/confirm-dialog/confirm-dialog';
import { DialogService } from '@common/services/dialog/dialog.service';
import { ToastService } from '@common/services/toast/toast';
import { AuthService } from '@shared/services/auth';
import { TECHNICAL_READINESS_ROLES } from '@shared/constants/technical-readiness.constants';
import { DependencySpikeService } from '../../../services/dependency-spike.service';
import {
  DependencySpikeDto,
  DependencySpikeStatus,
  UpdateDependencySpikeRequest
} from '../../../models/dependency-spikes/dependency-spike.model';
import {
  DependencySpikeFormDialogComponent,
  DependencySpikeDialogData
} from '../dialogs/dependency-spike-form-dialog/dependency-spike-form-dialog.component';
import {
  EditOwnerDialogComponent,
  EditOwnerDialogData,
  EditOwnerDialogResult
} from '../dialogs/edit-owner-dialog/edit-owner-dialog.component';

@Component({
  selector: 'app-dependency-spikes',
  standalone: true,
  imports: [
    CommonModule,
    InlineSVGModule,
    TooltipDirective,
    StatusBadgeComponent,
    ButtonComponent,
    EmptyStateComponent,
    ConfirmDialogComponent
  ],
  templateUrl: './dependency-spikes.component.html',
  styleUrl: './dependency-spikes.component.scss'
})
export class DependencySpikesComponent implements OnInit {
  engagementId = input.required<number>();

  private spikeService = inject(DependencySpikeService);
  private toastService = inject(ToastService);
  private authService = inject(AuthService);
  private dialog = inject(DialogService);

  spikes = signal<DependencySpikeDto[]>([]);
  isLoading = signal<boolean>(true);
  spikeToDelete = signal<DependencySpikeDto | null>(null);
  isDeleting = signal<boolean>(false);

  isTechLead = computed(() => this.authService.hasRole(TECHNICAL_READINESS_ROLES.TECH_LEAD));
  canManage = computed(() => this.isTechLead());

  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.loadSpikes(id);
      }
    });
  }

  ngOnInit(): void {
    if (this.engagementId()) {
      this.loadSpikes(this.engagementId());
    }
  }

  loadSpikes(engagementId: number): void {
    this.isLoading.set(true);
    this.spikeService.getSpikes(engagementId).subscribe({
      next: (data) => {
        this.spikes.set(data || []);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  openAddDialog(): void {
    const ref = this.dialog.open<DependencySpikeFormDialogComponent, DependencySpikeDialogData, DependencySpikeDto>(
      DependencySpikeFormDialogComponent,
      {
        data: {
          engagementId: this.engagementId()
        }
      }
    );

    ref.closed$.subscribe((newSpike) => {
      if (newSpike) {
        this.spikes.update((list) => [newSpike, ...list]);
      }
    });
  }

  openEditDialog(spike: DependencySpikeDto): void {
    const ref = this.dialog.open<DependencySpikeFormDialogComponent, DependencySpikeDialogData, DependencySpikeDto>(
      DependencySpikeFormDialogComponent,
      {
        data: {
          engagementId: this.engagementId(),
          spike
        }
      }
    );

    ref.closed$.subscribe((updatedSpike) => {
      if (updatedSpike) {
        this.spikes.update((list) =>
          list.map((s) => (s.id === updatedSpike.id ? updatedSpike : s))
        );
      }
    });
  }

  openEditOwnerModal(spike: DependencySpikeDto): void {
    const dialogRef = this.dialog.open<EditOwnerDialogComponent, EditOwnerDialogData, EditOwnerDialogResult>(
      EditOwnerDialogComponent,
      {
        data: {
          itemName: spike.dependencyName,
          engagementId: this.engagementId(),
          currentOwnerId: spike.ownerId ?? undefined
        }
      }
    );

    dialogRef.closed$.subscribe((result) => {
      if (!result) return;
      this.updateOwner(spike, result.ownerId);
    });
  }

  private updateOwner(spike: DependencySpikeDto, ownerId: number): void {
    const req: UpdateDependencySpikeRequest = {
      dependencyName: spike.dependencyName,
      objective: spike.objective,
      status: Number(spike.status) as DependencySpikeStatus,
      ownerId: ownerId,
      outcomeFindings: spike.outcomeFindings
    };

    this.spikeService.updateSpike(this.engagementId(), spike.id, req).subscribe({
      next: (updatedSpike) => {
        this.toastService.success('Owner updated successfully');
        this.spikes.update((list) =>
          list.map((s) => (s.id === updatedSpike.id ? updatedSpike : s))
        );
      },
      error: () => this.toastService.error('Failed to update owner')
    });
  }

  onDeleteSpike(spike: DependencySpikeDto): void {
    this.spikeToDelete.set(spike);
  }

  cancelDelete(): void {
    this.spikeToDelete.set(null);
  }

  confirmDelete(): void {
    const item = this.spikeToDelete();
    if (!item) return;

    this.isDeleting.set(true);
    this.spikeService.deleteSpike(this.engagementId(), item.id).subscribe({
      next: () => {
        this.toastService.success('Dependency spike deleted.');
        this.spikes.update((list) => list.filter((s) => s.id !== item.id));
        this.spikeToDelete.set(null);
        this.isDeleting.set(false);
      },
      error: () => {
        this.isDeleting.set(false);
      }
    });
  }

  getStatusTone(status: DependencySpikeStatus | number | string): StatusBadgeTone {
    const s = Number(status);
    if (s === DependencySpikeStatus.Passed || status === 'Passed') return 'success';
    if (s === DependencySpikeStatus.Failed || status === 'Failed') return 'danger';
    if (s === DependencySpikeStatus.InProgress || status === 'InProgress') return 'info';
    return 'neutral';
  }

  getStatusLabel(status: DependencySpikeStatus | number | string): string {
    const s = Number(status);
    if (s === DependencySpikeStatus.Passed || status === 'Passed') return 'Passed';
    if (s === DependencySpikeStatus.Failed || status === 'Failed') return 'Failed';
    if (s === DependencySpikeStatus.InProgress || status === 'InProgress') return 'In Progress';
    return String(status);
  }
}
