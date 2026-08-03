import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EngagementDocumentsComponent } from '../engagement-documents/engagement-documents.component';

@Component({
  selector: 'app-documents-demo',
  standalone: true,
  imports: [CommonModule, FormsModule, EngagementDocumentsComponent],
  templateUrl: './documents-demo.component.html',
  styleUrl: './documents-demo.component.scss'
})
export class DocumentsDemoComponent {
  inputEngagementId = signal<number>(1);
  activeEngagementId = signal<number>(1);

  onEngagementIdChange(val: string): void {
    const parsed = parseInt(val, 10);
    if (!isNaN(parsed) && parsed > 0) {
      this.inputEngagementId.set(parsed);
    }
  }

  loadEngagement(): void {
    this.activeEngagementId.set(this.inputEngagementId());
  }
}
