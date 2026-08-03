import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-engagement-workspace',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './engagement-workspace.component.html',
  styleUrls: ['./engagement-workspace.component.css']
})
export class EngagementWorkspaceComponent implements OnInit {
  engagementDetails = {
    engagementName: 'Project Phoenix',
    clientName: 'Acme Corp',
    engagementType: 'Delivery',
    kickoffDate: '2026-09-01',
    billingDate: '2026-09-15',
    readinessScore: 85,
    kickoffReadyState: 'Ready'
  };

  tabs = [
    { label: 'Handover', route: 'handover' },
    { label: 'Documents', route: 'documents' },
    { label: 'People', route: 'people' },
    { label: 'Checklist', route: 'checklist' },
    { label: 'Kickoff', route: 'kickoff' },
    { label: 'Resourcing', route: 'resourcing' },
    { label: 'Technical Readiness', route: 'technical-readiness' },
    { label: 'Risks', route: 'risks' },
    { label: 'Reporting', route: 'reporting' }
  ];

  ngOnInit() {
    // Future: fetch engagement details dynamically based on route param
  }
}
