import { Component, OnInit, inject, signal } from '@angular/core';

import { ActivatedRoute } from '@angular/router';
import { KickoffRecordCardComponent } from './components/kickoff-record-card/kickoff-record-card.component';

import { CommonModule } from '@angular/common';
import { GovernanceProcessComponent } from './components/governance-process/governance-process.component';
import { ChangeRequestsComponent } from './components/change-requests/change-requests.component';

@Component({
  selector: 'app-kickoff',
  standalone: true,
  imports: [CommonModule, KickoffRecordCardComponent, GovernanceProcessComponent, ChangeRequestsComponent],
  templateUrl: './kickoff.component.html',
  styleUrls: ['./kickoff.component.scss']
})
export class KickoffComponent implements OnInit {
  private route = inject(ActivatedRoute);
  engagementId = signal<number | null>(null);

  ngOnInit(): void {
    const parentParam = this.route.parent?.snapshot.paramMap.get('id');
    const directParam = this.route.snapshot.paramMap.get('id');
    const idStr = parentParam || directParam;

    if (idStr) {
      this.engagementId.set(+idStr);
    }
  }
}


