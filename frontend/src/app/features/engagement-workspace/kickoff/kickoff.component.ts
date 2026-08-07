import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GovernanceProcessComponent } from './components/governance-process/governance-process.component';

@Component({
  selector: 'app-kickoff',
  standalone: true,
  imports: [CommonModule, GovernanceProcessComponent],
  templateUrl: './kickoff.component.html',
  styleUrl: './kickoff.component.scss'
})
export class KickoffComponent {}
