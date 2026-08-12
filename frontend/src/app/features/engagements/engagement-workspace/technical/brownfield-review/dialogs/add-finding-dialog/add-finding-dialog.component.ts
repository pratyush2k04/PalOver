import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { BrownfieldSeverity } from '../../../../models/brownfield-review.model';
import { DocumentService } from '../../../../services/document.service';

export interface AddFindingDialogData {
  engagementId: number;
}

export interface AddFindingDialogResult {
  title: string;
  description?: string;
  severity: BrownfieldSeverity;
  documentId?: number | null;
}

@Component({
  selector: 'app-add-finding-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent, InputComponent, TextareaComponent, SelectComponent],
  templateUrl: './add-finding-dialog.component.html',
  styleUrl: './add-finding-dialog.component.scss'
})
export class AddFindingDialogComponent implements OnInit {
  private dialogRef = inject<DialogRef<AddFindingDialogResult, AddFindingDialogData>>(DialogRef);
  private documentService = inject(DocumentService);

  title = signal('');
  description = signal('');
  severity = signal<BrownfieldSeverity>(BrownfieldSeverity.Low);
  documentId = signal<number | null>(null);
  titleError = signal('');

  severityOptions: SelectOption[] = [
    { value: BrownfieldSeverity.Low, label: 'Low' },
    { value: BrownfieldSeverity.Medium, label: 'Medium' },
    { value: BrownfieldSeverity.High, label: 'High' }
  ];

  documentOptions = signal<SelectOption[]>([]);

  ngOnInit(): void {
    const engagementId = this.dialogRef.data?.engagementId;
    if (engagementId) {
      this.documentService.getEngagementDocuments(engagementId).subscribe({
        next: (docs) => {
          const options: SelectOption[] = docs.map(d => ({ value: d.id, label: d.fileName }));
          this.documentOptions.set([{ value: null, label: 'None (No document)' }, ...options]);
        },
        error: () => this.documentOptions.set([{ value: null, label: 'None (No document)' }])
      });
    } else {
      this.documentOptions.set([{ value: null, label: 'None (No document)' }]);
    }
  }

  onTitleChange(val: string): void {
    this.title.set(val);
    this.titleError.set('');
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    const titleVal = this.title().trim();
    if (!titleVal) {
      this.titleError.set('Finding title is required');
      return;
    }

    this.dialogRef.close({
      title: titleVal,
      description: this.description().trim() || undefined,
      severity: Number(this.severity()) as BrownfieldSeverity,
      documentId: this.documentId()
    });
  }
}
