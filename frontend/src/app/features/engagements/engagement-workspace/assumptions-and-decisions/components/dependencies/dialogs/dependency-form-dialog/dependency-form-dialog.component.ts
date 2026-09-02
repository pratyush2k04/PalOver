import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { DatepickerComponent } from '@common/components/form/datepicker/datepicker';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { DependencyDto, DependencyType, DependencyStatus } from '../../../../../../models/dependency.model';
import { PersonService } from '@shared/services/person.service';
import { ToastService } from '@common/services/toast/toast';
import { PersonDto, PersonType } from '@shared/models/person.model';
import { AuthService } from '@shared/services/auth';
import { futureDateValidator } from '@common/validators/date.validators';

export interface DependencyFormDialogData {
  dependency: DependencyDto | null;
  engagementId: number;
}

export interface DependencyFormDialogResult {
  description: string;
  type: DependencyType;
  ownerId: number;
  neededByDate: string;
  status?: DependencyStatus;
}

@Component({
  selector: 'app-dependency-form-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonComponent, TextareaComponent, SelectComponent, DatepickerComponent],
  templateUrl: './dependency-form-dialog.component.html',
  styleUrl: './dependency-form-dialog.component.scss'
})
export class DependencyFormDialogComponent implements OnInit {
  private dialogRef = inject<DialogRef<DependencyFormDialogResult>>(DialogRef);
  private fb = inject(FormBuilder);
  readonly data = inject<DependencyFormDialogData>(DIALOG_DATA);
  private personService = inject(PersonService);
  private toastService = inject(ToastService);
  private authService = inject(AuthService);

  readonly minDate = new Date().toISOString().split('T')[0];

  typeOptions: SelectOption[] = [
    { value: DependencyType.Internal, label: 'Internal' },
    { value: DependencyType.External, label: 'External' }
  ];


  ownerOptions = signal<SelectOption[]>([]);
  allPeople: PersonDto[] = [];

  form = this.fb.group({
    description: ['', [Validators.required]],
    type: [DependencyType.Internal, [Validators.required]],
    ownerId: [null as number | null, [Validators.required]],
    neededByDate: ['', [Validators.required, futureDateValidator]]
  });

  get isEditing(): boolean {
    return !!this.data.dependency;
  }

  constructor() {
    if (this.data.dependency) {
      const dep = this.data.dependency;
      this.form.patchValue({
        description: dep.description,
        type: dep.type,
        ownerId: dep.ownerId,
        neededByDate: dep.neededByDate.split('T')[0] // Extract just the date part for input type="date"
      });
      this.form.markAsPristine();
    }

    // When type changes, filter the owner dropdown
    this.form.get('type')?.valueChanges.subscribe(type => {
      this.filterOwnerOptions(type as DependencyType);
      // Reset owner if they change type and the current owner is no longer valid
      this.form.get('ownerId')?.setValue(null);
    });
  }

  ngOnInit() {
    this.loadPeople();
  }

  private loadPeople() {
    this.personService.getPeopleByEngagement(this.data.engagementId).subscribe({
      next: (people: PersonDto[]) => {
        this.allPeople = people;
        this.filterOwnerOptions(this.form.value.type as DependencyType);
      },
      error: (err: any) => {
        console.error('Failed to load people', err);
        this.toastService.error('Failed to load users for assignment.');
      }
    });
  }

  private filterOwnerOptions(type: DependencyType) {
    if (!type || !this.allPeople) return;
    
    // Assuming PersonDto has a way to distinguish internal vs external.
    // Let's assume it has an isExternal flag or similar. If not, we list all.
    // Typically, internal users have a userId, external don't.
    const currentUserId = this.authService.getUserId();

    const filtered = this.allPeople.filter(p => {
      if (p.userId === currentUserId) return false;

      if (type === DependencyType.Internal) {
        return p.personType === PersonType.Internal || p.personType === 'Internal';
      } else {
        return p.personType === PersonType.Client || p.personType === 'Client';
      }
    });

    this.ownerOptions.set(filtered.map(p => ({
      value: p.personId,
      label: p.name
    })));
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (!this.form.valid) {
      this.form.markAllAsTouched();
      return;
    }

    if (this.isEditing && !this.form.dirty) {
      this.dialogRef.close();
      return;
    }

    const val = this.form.value;
    const result: DependencyFormDialogResult = {
      description: val.description!.trim(),
      type: val.type as DependencyType,
      ownerId: val.ownerId!,
      // Convert date string back to full ISO string
      neededByDate: new Date(val.neededByDate!).toISOString()
    };

    if (this.isEditing) {
      result.status = this.data.dependency!.status;
    }

    this.dialogRef.close(result);
  }
}
