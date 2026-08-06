import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { EngagementChecklistService } from '../../../services/engagement-checklist.service';
import { EngagementChecklist } from '../../../models/engagement-checklist.model';
import { ENGAGEMENT_CHECKLIST_MESSAGES } from '../../../shared/constants/checklist.constants';

@Component({
  selector: 'app-engagement-checklist',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './engagement-checklist.component.html',
  styleUrl: './engagement-checklist.component.scss'
})
export class EngagementChecklistComponent {
  private route = inject(ActivatedRoute);
  private checklistService = inject(EngagementChecklistService);

  private paramMap = toSignal(this.route.parent!.paramMap);

  engagementId = computed(() => {
    const idStr = this.paramMap()?.get('id');
    return idStr ? +idStr : null;
  });

  checklist = signal<EngagementChecklist | null>(null);
  isLoading = signal<boolean>(true);
  error = signal<string | null>(null);

  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.loadChecklist(id);
      }
    });
  }

  private loadChecklist(engagementId: number): void {
    this.isLoading.set(true);
    this.error.set(null);
    this.checklistService.getEngagementChecklist(engagementId).subscribe({
      next: (data) => {
        this.checklist.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.error.set(ENGAGEMENT_CHECKLIST_MESSAGES.LOAD_FAILED);
        this.isLoading.set(false);
      }
    });
  }
}
