import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { EngagementDetailComponent } from './components/engagement-detail/engagement-detail';
import { OpenItemsByAreaComponent } from './components/open-items-by-area/open-items-by-area.component';
import { QuickActionsComponent } from './components/quick-actions/quick-actions.component';

@Component({
  selector: 'app-overview',
  standalone: true,
  imports: [CommonModule, EngagementDetailComponent, OpenItemsByAreaComponent, QuickActionsComponent],
  templateUrl: './overview.component.html',
  styleUrl: './overview.component.scss'
})
export class OverviewComponent {
  private route = inject(ActivatedRoute);

  engagementId = signal<number | null>(null);

  constructor() {
    const parentParam = this.route.parent?.snapshot.paramMap.get('id');
    const directParam = this.route.snapshot.paramMap.get('id');
    const idStr = parentParam || directParam;

    if (idStr) {
      this.engagementId.set(+idStr);
    }
  }
}
