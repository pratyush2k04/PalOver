import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { EngagementInfoDto, EngagementService, EngagementTypeOption, CommercialModelOption } from '../../../services/engagement.service';
import { AuthService } from '../../../services/auth';
import { ChangeTypeDialogComponent, ChangeTypeResult } from './dialogs/change-type-dialog.component';
import { ConfirmChangeDialogComponent } from './dialogs/confirm-change-dialog.component';

@Component({
  selector: 'app-overview',
  standalone: true,
  imports: [
    CommonModule,
    ChangeTypeDialogComponent,
    ConfirmChangeDialogComponent
  ],
  template: `
    <div class="overview-container">
      <!-- Engagement Type and Commercial Model Card -->
      <div class="card type-card" *ngIf="!isLoading(); else loadingState">
        <div class="card-header">
          <div class="card-title-group">
            <h2>Engagement Type and Commercial Model</h2>
          </div>
          <button
            *ngIf="canEdit()"
            type="button"
            class="edit-type-btn"
            (click)="openChangeTypeModal()">
            <svg xmlns="http://www.w3.org/2000/svg" class="btn-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z" />
            </svg>
            Change type
          </button>
        </div>

        <div class="card-body">
          <p class="type-summary">
            {{ currentNature() }} &middot; {{ currentCommercialModel() }}
          </p>

          <p class="type-description">
            <ng-container *ngIf="currentNature() === 'Brownfield'; else greenfieldDesc">
              Technical readiness includes code review and architecture analysis.
            </ng-container>
            <ng-template #greenfieldDesc>
              Standard technical readiness path.
            </ng-template>
          </p>
        </div>
      </div>

      <ng-template #loadingState>
        <div class="card loading-card">
          <p>Loading overview information...</p>
        </div>
      </ng-template>

      <!-- Change Type Modal -->
      <app-change-type-dialog
        *ngIf="showChangeTypeModal()"
        [currentNature]="currentNature()"
        [currentCommercialModel]="currentCommercialModel()"
        (save)="onSaveTypeRequest($event)"
        (cancel)="closeModals()">
      </app-change-type-dialog>

      <!-- Confirmation Modal -->
      <app-confirm-change-dialog
        *ngIf="showConfirmModal()"
        [targetNature]="pendingNature"
        (confirm)="onConfirmTypeChange()"
        (cancel)="closeModals()">
      </app-confirm-change-dialog>
    </div>
  `,
  styles: [`
    .overview-container {
      display: flex;
      flex-direction: column;
      gap: 1.5rem;
    }

    .card {
      background: #ffffff;
      border: 1px solid #e5e7eb;
      border-radius: 0.75rem;
      padding: 1.5rem;
      box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
    }

    .type-card {
      background: #ffffff;
    }

    .card-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-bottom: 1rem;
    }

    .card-title-group {
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }

    .card-title-group h2 {
      margin: 0;
      font-size: 1.125rem;
      font-weight: 600;
      color: #111827;
    }

    .edit-type-btn {
      display: inline-flex;
      align-items: center;
      gap: 0.375rem;
      padding: 0.375rem 0.875rem;
      font-size: 0.875rem;
      font-weight: 500;
      color: #374151;
      background: #ffffff;
      border: 1px solid #d1d5db;
      border-radius: 0.5rem;
      cursor: pointer;
      transition: all 0.15s ease;
    }

    .edit-type-btn:hover {
      background: #f9fafb;
      border-color: #9ca3af;
      color: #111827;
    }

    .btn-icon {
      width: 1rem;
      height: 1rem;
    }

    .type-summary {
      font-size: 1rem;
      font-weight: 600;
      color: #374151;
      margin: 0 0 0.5rem 0;
    }

    .type-description {
      font-size: 0.875rem;
      color: #6b7280;
      margin: 0;
    }

    .loading-card {
      color: #6b7280;
      font-size: 0.875rem;
    }
  `]
})
export class OverviewComponent {
  private route = inject(ActivatedRoute);
  private engagementService = inject(EngagementService);
  private authService = inject(AuthService);

  private parentParamMap = toSignal(this.route.parent!.paramMap);

  engagementId = computed(() => {
    const idStr = this.parentParamMap()?.get('id');
    return idStr ? +idStr : null;
  });

  engagement = signal<EngagementInfoDto | null>(null);
  isLoading = signal<boolean>(true);

  showChangeTypeModal = signal<boolean>(false);
  showConfirmModal = signal<boolean>(false);

  pendingNature: string = '';
  pendingCommercialModel: string = '';

  typesList: EngagementTypeOption[] = [];
  modelsList: CommercialModelOption[] = [];

  // Role check: Only Pre Sales users can edit/change the type
  canEdit = computed(() => {
    return (
      this.authService.hasRole('Pre Sales') ||
      this.authService.hasRole('PreSales') ||
      this.authService.currentUser()?.role?.toLowerCase().includes('pre sales') ||
      this.authService.currentUser()?.role?.toLowerCase().includes('presales')
    );
  });

  currentNature = computed(() => {
    return this.engagement()?.engagementType || 'Brownfield';
  });

  currentCommercialModel = computed(() => {
    const model = this.engagement()?.commercialModel;
    if (!model) return 'Fixed';
    const modelLower = model.toLowerCase();
    if (modelLower.includes('time') || modelLower.includes('t&m')) return 'Time & Material';
    if (modelLower.includes('capped')) return 'Capped';
    return model;
  });

  private getCommercialModelId(modelName: string): number {
    if (!modelName) return 2;
    const norm = modelName.toLowerCase().replace('&', 'and').replace(/\s+/g, '');
    const found = this.modelsList.find(m => {
      const mNorm = m.name.toLowerCase().replace('&', 'and').replace(/\s+/g, '');
      return mNorm.includes(norm) || norm.includes(mNorm) || (norm.includes('time') && mNorm.includes('time')) || (norm.includes('capped') && mNorm.includes('capped'));
    });
    if (found) return found.id;
    if (modelName.toLowerCase().includes('time') || modelName.toLowerCase().includes('t&m')) return 1;
    if (modelName.toLowerCase().includes('fixed')) return 2;
    if (modelName.toLowerCase().includes('capped')) return 3;
    return 2;
  }

  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.loadData(id);
      }
    });
  }

  private loadData(id: number) {
    this.isLoading.set(true);
    this.engagementService.getEngagementInfo(id).subscribe({
      next: (data: EngagementInfoDto) => {
        this.engagement.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });

    this.engagementService.getEngagementTypes().subscribe({
      next: (res: { engagementTypes: EngagementTypeOption[] }) => {
        this.typesList = res.engagementTypes || [];
      }
    });

    this.engagementService.getCommercialModels().subscribe({
      next: (res: { commercialModels: CommercialModelOption[] }) => {
        this.modelsList = res.commercialModels || [];
      }
    });
  }

  openChangeTypeModal() {
    this.showChangeTypeModal.set(true);
  }

  closeModals() {
    this.showChangeTypeModal.set(false);
    this.showConfirmModal.set(false);
  }

  onSaveTypeRequest(result: ChangeTypeResult) {
    this.pendingNature = result.nature;
    this.pendingCommercialModel = result.commercialModel;
    this.showChangeTypeModal.set(false);

    // If nature changed, show confirm modal
    if (result.nature !== this.currentNature()) {
      this.showConfirmModal.set(true);
    } else {
      // Direct update if only commercial model changed
      this.onConfirmTypeChange();
    }
  }

  onConfirmTypeChange() {
    const id = this.engagementId();
    if (!id) return;

    const targetType = this.typesList.find(t => t.name.toLowerCase() === this.pendingNature.toLowerCase());
    const typeId = targetType ? targetType.id : (this.pendingNature.toLowerCase() === 'brownfield' ? 2 : 1);
    const modelId = this.getCommercialModelId(this.pendingCommercialModel);

    this.engagementService.updateEngagementTypeAndModel(id, typeId, modelId).subscribe({
      next: () => {
        this.closeModals();
        this.loadData(id);
      },
      error: (err) => {
        console.error('Error updating engagement type and commercial model', err);
        this.closeModals();
      }
    });
  }
}
