import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { AccessItemDto } from '../../../models/technical-readiness.model';
import { AccessTrackerComponent } from './access-tracker/access-tracker.component';

@Component({
  selector: 'app-technical-readiness',
  standalone: true,
  imports: [CommonModule, AccessTrackerComponent],
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

  grantedCount = computed(() =>
    this.accessItems().filter(item => item.status === 'Granted' || item.status === '2').length
  );

  totalCount = computed(() => this.accessItems().length);

  isDayOneReady = computed(() =>
    this.totalCount() > 0 && this.grantedCount() === this.totalCount()
  );

  onAccessItemsUpdated(items: AccessItemDto[]) {
    this.accessItems.set(items);
  }

  shareSummary() {
    alert('Day-1 Summary shared with team!');
  }
}
