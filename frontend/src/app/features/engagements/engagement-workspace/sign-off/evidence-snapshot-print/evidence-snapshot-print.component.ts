import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { ButtonComponent } from '@common/components/button/button';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { StatusBadgeComponent, StatusBadgeTone } from '@common/components/status-badge/status-badge';
import { EngagementService } from '@features/engagements/services/engagement.service';
import { EvidenceSnapshot, EvidenceSnapshotItemStatus } from '@features/engagements/models/evidence-snapshot.model';

@Component({
  selector: 'app-evidence-snapshot-print',
  standalone: true,
  imports: [CommonModule, ButtonComponent, LoadingSpinnerComponent, StatusBadgeComponent],
  templateUrl: './evidence-snapshot-print.component.html',
  styleUrl: './evidence-snapshot-print.component.scss'
})
export class EvidenceSnapshotPrintComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private engagementService = inject(EngagementService);
  private titleService = inject(Title);

  snapshot = signal<EvidenceSnapshot | null>(null);
  isLoading = signal<boolean>(true);
  error = signal<string | null>(null);

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('engagementId') ?? this.route.snapshot.paramMap.get('id');
    const engagementId = idParam ? +idParam : null;

    if (!engagementId || isNaN(engagementId)) {
      this.error.set('Invalid Engagement ID');
      this.isLoading.set(false);
      return;
    }

    this.loadSnapshot(engagementId);
  }

  loadSnapshot(engagementId: number): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.engagementService.getEvidenceSnapshot(engagementId).subscribe({
      next: (data) => {
        this.snapshot.set(data);
        this.isLoading.set(false);
        this.titleService.setTitle('Day-1 Evidence Snapshot');
      },
      error: () => {
        this.error.set('Failed to load the evidence snapshot.');
        this.isLoading.set(false);
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

  onPrint(): void {
    window.print();
  }

  onClose(): void {
    window.close();
  }
}
