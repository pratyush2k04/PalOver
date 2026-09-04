import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { BrownfieldSeverity } from '@features/engagements/models/brownfield-review/brownfield-review.enums';
import { CreateBrownfieldFindingRequest } from '@features/engagements/models/brownfield-review/brownfield-review.requests';
import { DocumentService } from '@features/engagements/services/document.service';
import { DocumentItem } from '@features/engagements/models/document.model';

export interface AddFindingDialogData {
  engagementId: number;
}

export interface AddFindingDialogResult {
  title: string;
  description: string;
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
  private dialogRef = inject<DialogRef<AddFindingDialogResult>>(DialogRef);
  readonly data = inject<AddFindingDialogData>(DIALOG_DATA, { optional: true });
  private documentService = inject(DocumentService);

  title = signal('');
  description = signal('');
  severity = signal<BrownfieldSeverity>(BrownfieldSeverity.Low);
  documentId = signal<number | null>(null);
  titleError = signal('');
  descriptionError = signal('');

  severityOptions: SelectOption[] = [
    { value: BrownfieldSeverity.Low, label: 'Low' },
    { value: BrownfieldSeverity.Medium, label: 'Medium' },
    { value: BrownfieldSeverity.High, label: 'High' }
  ];

  documentOptions = signal<SelectOption[]>([]);

  ngOnInit(): void {
    const engagementId = this.data?.engagementId;
    if (engagementId) {
      this.documentService.getEngagementDocuments(engagementId).subscribe({
        next: (docs: DocumentItem[]) => {
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

  onDescriptionChange(val: string): void {
    this.description.set(val);
    this.descriptionError.set('');
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    const titleVal = this.title().trim();
    const descVal = this.description().trim();
    let isValid = true;

    if (!titleVal) {
      this.titleError.set('Finding title is required');
      isValid = false;
    } else if (titleVal.length > 200) {
      this.titleError.set('Finding title cannot exceed 200 characters');
      isValid = false;
    }

    if (!descVal) {
      this.descriptionError.set('Finding description is required');
      isValid = false;
    } else if (descVal.length > 1000) {
      this.descriptionError.set('Finding description cannot exceed 1000 characters');
      isValid = false;
    }

    if (!isValid) {
      return;
    }

    this.dialogRef.close({
      title: titleVal,
      description: descVal,
      severity: Number(this.severity()) as BrownfieldSeverity,
      documentId: this.documentId()
    });
  }
}
