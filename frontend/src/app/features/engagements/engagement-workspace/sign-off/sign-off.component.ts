import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { AlertComponent } from '@common/components/alert/alert';
import { BadgeComponent } from '@common/components/badge/badge';
import { ButtonComponent } from '@common/components/button/button';
import {
  StatusBadgeComponent,
  StatusBadgeTone,
} from '@common/components/status-badge/status-badge';
import { EngagementService } from '@features/engagements/services/engagement.service';
import {
  EvidenceSnapshot,
  EvidenceSnapshotItemStatus,
} from '@features/engagements/models/evidence-snapshot.model';
import { ENGAGEMENT_ROUTES } from '@shared/constants/routes.constants';

@Component({
  selector: 'app-sign-off',
  standalone: true,
  imports: [CommonModule, InlineSVGModule, AlertComponent, ButtonComponent, StatusBadgeComponent],
  templateUrl: './sign-off.component.html',
  styleUrl: './sign-off.component.scss',
})
export class SignOffComponent {
  private route = inject(ActivatedRoute);
  private engagementService = inject(EngagementService);

  private parentParamMap = toSignal(this.route.parent!.paramMap);

  engagementId = computed(() => {
    const idStr = this.parentParamMap()?.get('id');
    return idStr ? +idStr : null;
  });

  snapshot = signal<EvidenceSnapshot | null>(null);

  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.engagementService.getEvidenceSnapshot(id).subscribe({
          next: (snapshot) => this.snapshot.set(snapshot),
          error: () => this.snapshot.set(null),
        });
      }
    });
  }

  statusTone(status: EvidenceSnapshotItemStatus): StatusBadgeTone {
    if (status === 'Complete') return 'success';
    if (status === 'NotApplicable') return 'neutral';
    return 'warning';
  }

  statusLabel(status: EvidenceSnapshotItemStatus): string {
    return status === 'NotApplicable' ? 'Not Applicable' : status;
  }

  exportSnapshot(): void {
    const id = this.engagementId();
    if (!id) return;
    window.open(
      `/${ENGAGEMENT_ROUTES.BASE}/${id}/${ENGAGEMENT_ROUTES.EVIDENCE_SNAPSHOT_PRINT}`,
      '_blank',
    );
  }
}
