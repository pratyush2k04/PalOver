import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { CommunicationCadenceService } from '../../../../services/communication-cadence.service';
import { EngagementService } from '../../../../services/engagement.service';
import { AuthService } from '@shared/services/auth';
import { CommunicationCadence, CreateCommunicationCadence, EngagementParticipant } from '../../../../models/cadence.model';
import { CADENCE_CONSTANTS } from '@shared/constants/cadence.constants';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import { ButtonComponent } from '@common/components/button/button';
import { DialogService } from '@common/services/dialog/dialog.service';
import { AddCadenceDialogComponent, AddCadenceDialogData } from './dialogs/add-cadence-dialog/add-cadence-dialog.component';

@Component({
  selector: 'app-governance-process',
  standalone: true,
  imports: [
    CommonModule,
    EmptyStateComponent,
    ButtonComponent
  ],
  templateUrl: './governance-process.component.html',
  styleUrl: './governance-process.component.scss'
})
export class GovernanceProcessComponent {
  private route = inject(ActivatedRoute);
  private cadenceService = inject(CommunicationCadenceService);
  private authService = inject(AuthService);
  private engagementService = inject(EngagementService);
  private dialog = inject(DialogService);

  readonly constants = CADENCE_CONSTANTS;

  readonly isSignedOff = computed(() => this.engagementService.isSignedOff());
  isProjectManager = computed(() => !this.isSignedOff() && this.authService.hasRole('Project Management'));

  private paramMap = toSignal(this.route.paramMap);
  private parentParamMap = toSignal(this.route.parent?.paramMap ?? this.route.paramMap);

  engagementId = computed(() => {
    const idStr = this.paramMap()?.get('id') || this.parentParamMap()?.get('id');
    return idStr ? +idStr : null;
  });

  cadences = signal<CommunicationCadence[]>([]);

  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.loadCadences(id);
      }
    });
  }

  loadCadences(engagementId: number): void {
    this.cadenceService.getCadences(engagementId).subscribe({
      next: (data) => this.cadences.set(data),
      error: (err) => console.error('Failed to load communication cadences:', err)
    });
  }

  openAddCadenceModal(): void {
    const id = this.engagementId();
    if (!id || !this.isProjectManager()) return;

    this.cadenceService.getAssignedParticipants(id).subscribe({
      next: (participants) => this.openAddCadenceDialog(id, participants),
      error: (err) => {
        console.error('Failed to load assigned participants:', err);
        this.openAddCadenceDialog(id, []);
      }
    });
  }

  private openAddCadenceDialog(engagementId: number, participants: EngagementParticipant[]): void {
    const dialogRef = this.dialog.open<AddCadenceDialogComponent, AddCadenceDialogData, CreateCommunicationCadence>(
      AddCadenceDialogComponent,
      { data: { engagementId, participants } }
    );

    dialogRef.closed$.subscribe((request) => {
      if (!request) return;

      this.cadenceService.addCadence(engagementId, request).subscribe({
        next: (newCadence) => {
          this.cadences.update(list => [...list, newCadence]);
        },
        error: (err) => {
          console.error('Failed to add communication cadence:', err);
        }
      });
    });
  }
}
