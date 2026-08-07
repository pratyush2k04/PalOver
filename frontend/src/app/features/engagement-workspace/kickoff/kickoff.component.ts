import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GovernanceProcessComponent } from './components/governance-process/governance-process.component';
import { ChangeRequestsComponent } from './components/change-requests/change-requests.component';

@Component({
  selector: 'app-kickoff',
  standalone: true,
  imports: [CommonModule, GovernanceProcessComponent, ChangeRequestsComponent],
  templateUrl: './kickoff.component.html',
  styleUrl: './kickoff.component.scss'
})
export class KickoffComponent {}
