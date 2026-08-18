import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { RiskService } from './services/risk.service';
import {
  Risk,
  RiskImpact,
  RiskLikelihood,
  RiskStatus,
  OwnerOption,
  CreateRiskRequest,
  UpdateRiskRequest} from './models/risk.model';
import { forkJoin, of, catchError } from 'rxjs';
import { EngagementService } from '../../services/engagement.service';
import { EngagementAllocationService } from '../../../engagement-allocation/services/engagement-allocation.service';
import { UserService } from '../../../users/services/user.service';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { ButtonComponent } from '@common/components/button/button';
import { AlertComponent } from '@common/components/alert/alert';
import { StatusBadgeComponent, StatusBadgeTone } from '@common/components/status-badge/status-badge';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import { InputComponent } from '@common/components/form/input/input';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { ConfirmDialogComponent } from '@common/components/confirm-dialog/confirm-dialog';
import { ToastService } from '@common/services/toast/toast';
import { RISK_MESSAGES } from '@shared/constants/app.constants';

@Component({
  selector: 'app-risks',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterLink,
    LoadingSpinnerComponent,
    ButtonComponent,
    AlertComponent,
    StatusBadgeComponent,
    EmptyStateComponent,
    InputComponent,
    SelectComponent,
    TextareaComponent 
  ],
  templateUrl: './risks.component.html',
  styleUrl: './risks.component.scss'
})
export class RisksComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private riskService = inject(RiskService);
  private engagementService = inject(EngagementService);
  private engagementAllocationService = inject(EngagementAllocationService);
  private userService = inject(UserService);
  private fb = inject(FormBuilder);
  private toastService = inject(ToastService);

  engagementId = signal<number | null>(null);
  risks = signal<Risk[]>([]);
  owners = signal<OwnerOption[]>([]);
  isLoading = signal<boolean>(true);
  isSubmitting = signal<boolean>(false);
  formSubmitted = signal<boolean>(false);
  closeFormSubmitted = signal<boolean>(false);

  // Filter signals
  statusFilter = signal<RiskStatus | 'ALL'>('ALL');
  ownerFilter = signal<number | 'ALL'>('ALL');

  readonly impactOptions: SelectOption[] = [
    { value: 'Low', label: 'Low' },
    { value: 'Medium', label: 'Medium' },
    { value: 'High', label: 'High' }
  ];

  readonly likelihoodOptions: SelectOption[] = [
    { value: 'Low', label: 'Low' },
    { value: 'Medium', label: 'Medium' },
    { value: 'High', label: 'High' }
  ];

  readonly statusEditOptions: SelectOption[] = [
    { value: 'Open', label: 'Open' },
    { value: 'Mitigating', label: 'Mitigating' }
  ];

  readonly statusFilterOptions: SelectOption[] = [
    { value: 'ALL', label: 'All Statuses' },
    { value: 'Open', label: 'Open' },
    { value: 'Mitigating', label: 'Mitigating' },
    { value: 'Closed', label: 'Closed' }
  ];

  readonly ownerFilterOptions = computed<SelectOption[]>(() => [
    { value: 'ALL', label: 'All Owners' },
    ...this.owners().map(o => ({
      value: o.id,
      label: o.role ? `${o.name} (${o.role})` : o.name
    }))
  ]);

  readonly ownerSelectOptions = computed<SelectOption[]>(() => [
    { value: null, label: '-- Select Owner --' },
    ...this.owners().map(o => ({
      value: o.id,
      label: o.role ? `${o.name} (${o.role})` : o.name
    }))
  ]);

  getSourceRoute(sourceType: string | null | undefined): string[] {
    const id = this.engagementId();
    if (!id) return [];
    if (sourceType === 'Checklist') {
      return ['/engagements', id.toString(), 'workspace', 'checklist'];
    }
    return ['/engagements', id.toString(), 'workspace', 'technical-readiness'];
  }

  onStatusFilterSelect(val: unknown): void {
    this.statusFilter.set((val as RiskStatus | 'ALL') || 'ALL');
  }

  onOwnerFilterSelect(val: unknown): void {
    if (val === 'ALL' || val === null || val === undefined) {
      this.ownerFilter.set('ALL');
    } else {
      this.ownerFilter.set(Number(val));
    }
  }

  // Filtered & Sorted Risks
  filteredRisks = computed(() => {
    let list = this.risks();
    const status = this.statusFilter();
    const owner = this.ownerFilter();

    if (status !== 'ALL') {
      list = list.filter((r) => r.status === status);
    }
    if (owner !== 'ALL') {
      list = list.filter((r) => r.ownerId === owner);
    }

    return list;
  });

  // Top Priority risks count
  topPriorityCount = computed(() => {
    return this.risks().filter((r) => r.impact === 'High' && r.likelihood === 'High' && r.status !== 'Closed').length;
  });

  // Modal State
  showFormModal = signal<boolean>(false);
  isEditMode = signal<boolean>(false);
  editingRiskId = signal<number | null>(null);

  showCloseModal = signal<boolean>(false);
  closingRisk = signal<Risk | null>(null);

  // Reactive Forms
  riskForm: FormGroup = this.fb.group({
    title: ['', [Validators.required, Validators.maxLength(300)]],
    description: ['', [Validators.required, Validators.maxLength(2000)]],
    impact: ['Medium', [Validators.required]],
    likelihood: ['Medium', [Validators.required]],
    ownerId: [null, [Validators.required]],
    mitigation: ['', [Validators.maxLength(2000)]],
    status: ['Open', [Validators.required]]
  });

  closeForm: FormGroup = this.fb.group({
    resolutionNote: ['', [Validators.required, Validators.maxLength(2000)]]
  });

  ngOnInit(): void {
    this.route.parent?.paramMap.subscribe((params) => {
      const id = params.get('id');
      if (id) {
        this.engagementId.set(+id);
        this.loadRisks(+id);
        this.loadOwners(+id);
      }
    });
  }

  loadRisks(engagementId: number): void {
    this.isLoading.set(true);
    this.riskService.getRisks(engagementId).subscribe({
      next: (data) => {
        this.risks.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Failed to load risks:', err);
        this.toastService.error(RISK_MESSAGES.LOAD_ERROR);
        this.isLoading.set(false);
      }
    });
  }

  loadOwners(engagementId: number): void {
    forkJoin({
      owners: this.engagementService.getEngagementOwners(engagementId).pipe(catchError(() => of(null))),
      leadership: this.engagementAllocationService.getLeadershipUsers(engagementId).pipe(catchError(() => of([]))),
      projectManagers: this.userService.getUsersInRole(2).pipe(catchError(() => of([]))),
      techLeads: this.userService.getUsersInRole(3).pipe(catchError(() => of([]))),
      preSales: this.userService.getUsersInRole(1).pipe(catchError(() => of([])))
    }).subscribe({
      next: ({ owners, leadership, projectManagers, techLeads, preSales }) => {
        const options: OwnerOption[] = [];

        if (owners?.projectManagerId && owners?.projectManagerName) {
          options.push({
            id: owners.projectManagerId,
            name: owners.projectManagerName,
            role: 'Project Manager (Assigned)'
          });
        }

        if (owners?.techLeadId && owners?.techLeadName) {
          options.push({
            id: owners.techLeadId,
            name: owners.techLeadName,
            role: 'Technical Lead (Assigned)'
          });
        }

        if (leadership && leadership.length > 0) {
          leadership.forEach((user) => {
            if (!options.some((o) => o.id === user.id)) {
              options.push({
                id: user.id,
                name: user.fullName,
                role: 'Leadership'
              });
            }
          });
        }

        if (projectManagers && projectManagers.length > 0) {
          projectManagers.forEach((user) => {
            if (!options.some((o) => o.id === user.id)) {
              options.push({
                id: user.id,
                name: user.fullName,
                role: 'Project Manager'
              });
            }
          });
        }

        if (techLeads && techLeads.length > 0) {
          techLeads.forEach((user) => {
            if (!options.some((o) => o.id === user.id)) {
              options.push({
                id: user.id,
                name: user.fullName,
                role: 'Technical Lead'
              });
            }
          });
        }

        if (preSales && preSales.length > 0) {
          preSales.forEach((user) => {
            if (!options.some((o) => o.id === user.id)) {
              options.push({
                id: user.id,
                name: user.fullName,
                role: 'PreSales'
              });
            }
          });
        }

        this.owners.set(options);
      },
      error: () => this.owners.set([])
    });
  }

  openCreateModal(): void {
    this.isEditMode.set(false);
    this.editingRiskId.set(null);
    this.formSubmitted.set(false);
    this.riskForm.reset({
      title: '',
      description: '',
      impact: 'Medium',
      likelihood: 'Medium',
      ownerId: null,
      mitigation: '',
      status: 'Open'
    });
    this.showFormModal.set(true);
  }

  openEditModal(risk: Risk): void {
    this.isEditMode.set(true);
    this.editingRiskId.set(risk.id);
    this.formSubmitted.set(false);
    this.riskForm.patchValue({
      title: risk.title,
      description: risk.description,
      impact: risk.impact,
      likelihood: risk.likelihood,
      ownerId: risk.ownerId ?? null,
      mitigation: risk.mitigation ?? '',
      status: risk.status
    });
    this.showFormModal.set(true);
  }

  closeFormModal(): void {
    this.showFormModal.set(false);
    this.formSubmitted.set(false);
    this.riskForm.reset();
  }

  saveRisk(): void {
    this.formSubmitted.set(true);

    if (this.riskForm.invalid) {
      this.riskForm.markAllAsTouched();
      return;
    }

    const engagementId = this.engagementId();
    if (!engagementId) return;

    const val = this.riskForm.value;
    this.isSubmitting.set(true);

    if (this.isEditMode() && this.editingRiskId()) {
      const updateReq: UpdateRiskRequest = {
        title: val.title,
        description: val.description,
        impact: val.impact as RiskImpact,
        likelihood: val.likelihood as RiskLikelihood,
        ownerId: val.ownerId ? +val.ownerId : null,
        mitigation: val.mitigation,
        status: val.status as RiskStatus
      };

      this.riskService.updateRisk(engagementId, this.editingRiskId()!, updateReq).subscribe({
        next: () => {
          this.toastService.success(RISK_MESSAGES.UPDATE_SUCCESS);
          this.closeFormModal();
          this.loadRisks(engagementId);
          this.isSubmitting.set(false);
        },
        error: (err) => {
          console.error(err);
          this.toastService.error(RISK_MESSAGES.UPDATE_ERROR);
          this.isSubmitting.set(false);
        }
      });
    } else {
      const createReq: CreateRiskRequest = {
        title: val.title,
        description: val.description,
        impact: val.impact as RiskImpact,
        likelihood: val.likelihood as RiskLikelihood,
        ownerId: val.ownerId ? +val.ownerId : null,
        mitigation: val.mitigation
      };

      this.riskService.createRisk(engagementId, createReq).subscribe({
        next: () => {
          this.toastService.success(RISK_MESSAGES.CREATE_SUCCESS);
          this.closeFormModal();
          this.loadRisks(engagementId);
          this.isSubmitting.set(false);
        },
        error: (err) => {
          console.error(err);
          this.toastService.error(RISK_MESSAGES.CREATE_ERROR);
          this.isSubmitting.set(false);
        }
      });
    }
  }

  openCloseModal(risk: Risk): void {
    this.closingRisk.set(risk);
    this.closeFormSubmitted.set(false);
    this.closeForm.reset({ resolutionNote: '' });
    this.showCloseModal.set(true);
  }

  closeCloseModal(): void {
    this.showCloseModal.set(false);
    this.closeFormSubmitted.set(false);
    this.closingRisk.set(null);
    this.closeForm.reset();
  }

  submitCloseRisk(): void {
    this.closeFormSubmitted.set(true);

    if (this.closeForm.invalid) {
      this.closeForm.markAllAsTouched();
      return;
    }

    const engagementId = this.engagementId();
    const risk = this.closingRisk();
    if (!engagementId || !risk) return;

    this.isSubmitting.set(true);
    this.riskService.closeRisk(engagementId, risk.id, {
      resolutionNote: this.closeForm.value.resolutionNote
    }).subscribe({
      next: () => {
        this.toastService.success(RISK_MESSAGES.CLOSE_SUCCESS);
        this.closeCloseModal();
        this.loadRisks(engagementId);
        this.isSubmitting.set(false);
      },
      error: (err) => {
        console.error(err);
        this.toastService.error(RISK_MESSAGES.CLOSE_ERROR);
        this.isSubmitting.set(false);
      }
    });
  }

  reopenRisk(risk: Risk): void {
    const engagementId = this.engagementId();
    if (!engagementId) return;

    this.riskService.reopenRisk(engagementId, risk.id).subscribe({
      next: () => {
        this.toastService.success(RISK_MESSAGES.REOPEN_SUCCESS);
        this.loadRisks(engagementId);
      },
      error: (err) => {
        console.error(err);
        this.toastService.error(RISK_MESSAGES.REOPEN_ERROR);
      }
    });
  }

  onStatusFilterChange(event: Event): void {
    const val = (event.target as HTMLSelectElement).value;
    this.statusFilter.set(val as RiskStatus | 'ALL');
  }

  onOwnerFilterChange(event: Event): void {
    const val = (event.target as HTMLSelectElement).value;
    this.ownerFilter.set(val === 'ALL' ? 'ALL' : +val);
  }

  isTopPriority(risk: Risk): boolean {
    return risk.impact === 'High' && risk.likelihood === 'High';
  }

  getStatusTone(status: RiskStatus): StatusBadgeTone {
    switch (status) {
      case 'Open':
        return 'success';
      case 'Mitigating':
        return 'warning';
      case 'Closed':
        return 'neutral';
    }
  }
}
