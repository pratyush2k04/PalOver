import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { AccessItemDto } from './models/technical-readiness.model';
import { EnvironmentToolingItemDto, EnvironmentToolingStatus } from './models/environment-tooling.model';
import { AccessTrackerComponent } from './access-tracker/access-tracker.component';
import { EnvironmentToolingComponent } from './environment-tooling/environment-tooling.component';

@Component({
  selector: 'app-technical-readiness',
  standalone: true,
  imports: [CommonModule, AccessTrackerComponent, EnvironmentToolingComponent],
  templateUrl: './technical-readiness.component.html',
  styleUrl: './technical-readiness.component.scss'
})
export class TechnicalReadinessComponent {
  private route = inject(ActivatedRoute);

  private parentParamMap = toSignal(this.route.parent!.paramMap);

  engagementId = computed(() => {
    const idStr = this.parentParamMap()?.get('id');
    return idStr ? +idStr : null;
  });

  accessItems = signal<AccessItemDto[]>([]);
  toolingItems = signal<EnvironmentToolingItemDto[]>([]);

  // Access Metrics
  accessGrantedCount = computed(() =>
    this.accessItems().filter(i => i.status === 'Granted' || i.status === '2').length
  );
  accessTotalCount = computed(() => this.accessItems().length);

  // Tooling Metrics (Not Applicable items are excluded from required total)
  toolingApplicableItems = computed(() =>
    this.toolingItems().filter(i => +i.status !== EnvironmentToolingStatus.NotApplicable && i.status !== ('NotApplicable' as any))
  );

  toolingReadyCount = computed(() =>
    this.toolingApplicableItems().filter(i => +i.status === EnvironmentToolingStatus.Ready || i.status === ('Ready' as any)).length
  );

  toolingTotalCount = computed(() => this.toolingApplicableItems().length);

  // Blockers Check
  hasAccessBlockers = computed(() =>
    this.accessItems().some(i => i.status === 'Blocked' || i.status === '3')
  );

  hasBlockers = computed(() => this.hasAccessBlockers());

  // Total readiness check
  totalRequired = computed(() => this.accessTotalCount() + this.toolingTotalCount());
  totalReady = computed(() => this.accessGrantedCount() + this.toolingReadyCount());

  isDayOneReady = computed(() =>
    !this.hasBlockers() &&
    this.totalRequired() > 0 &&
    this.totalReady() === this.totalRequired()
  );

  onAccessItemsUpdated(items: AccessItemDto[]) {
    this.accessItems.set(items);
  }

  onToolingItemsUpdated(items: EnvironmentToolingItemDto[]) {
    this.toolingItems.set(items);
  }

  shareSummary() {
    alert('Day-1 Summary shared with team!');
  }
}
