import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { KickoffActionItem } from '@shared/models/kickoff.model';
import { KICKOFF_MESSAGES } from '@shared/constants/kickoff.constants';
import { futureDateValidator } from '@common/validators/date.validators';
import { EngagementService } from '@features/engagements/services/engagement.service';
import { EngagementAllocationService } from '@features/engagement-allocation/services/engagement-allocation.service';
import { UserService } from '@features/users/services/user.service';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

export type AddActionDialogResult = Pick<KickoffActionItem, 'title' | 'ownerName' | 'dueDate'>;

export interface AddActionDialogData {
  engagementId: number;
}

@Component({
  selector: 'app-add-action-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonComponent, InputComponent, SelectComponent],
  templateUrl: './add-action-dialog.component.html',
  styleUrl: './add-action-dialog.component.scss'
})
export class AddActionDialogComponent implements OnInit {
  private fb = inject(FormBuilder);
  private dialogRef = inject<DialogRef<AddActionDialogResult>>(DialogRef);
  readonly data = inject<AddActionDialogData>(DIALOG_DATA);
  
  private engagementService = inject(EngagementService);
  private engagementAllocationService = inject(EngagementAllocationService);
  private userService = inject(UserService);

  readonly messages = KICKOFF_MESSAGES;
  readonly minDate = new Date().toISOString().split('T')[0];

  ownerOptions = signal<SelectOption[]>([]);

  readonly form = this.fb.nonNullable.group({
    title: ['', [Validators.required]],
    ownerName: ['', [Validators.required]],
    dueDate: ['', [Validators.required, futureDateValidator]]
  });

  get titleControl() {
    return this.form.get('title')!;
  }

  get ownerNameControl() {
    return this.form.get('ownerName')!;
  }

  get dueDateControl() {
    return this.form.get('dueDate')!;
  }

  ngOnInit() {
    this.loadOwners(this.data.engagementId);
  }

  private loadOwners(engagementId: number): void {
    forkJoin({
      owners: this.engagementService.getEngagementOwners(engagementId).pipe(catchError(() => of(null))),
      leadership: this.engagementAllocationService.getLeadershipUsers(engagementId).pipe(catchError(() => of([]))),
      projectManagers: this.userService.getUsersInRole(2).pipe(catchError(() => of([]))),
      techLeads: this.userService.getUsersInRole(3).pipe(catchError(() => of([]))),
      preSales: this.userService.getUsersInRole(1).pipe(catchError(() => of([])))
    }).subscribe({
      next: ({ owners, leadership, projectManagers, techLeads, preSales }) => {
        const options: SelectOption[] = [];
        const seenNames = new Set<string>();

        const addOption = (name: string, role: string) => {
          if (name && !seenNames.has(name)) {
            seenNames.add(name);
            options.push({
              value: name, 
              label: name
            });
          }
        };

        if (owners?.projectManagerName) addOption(owners.projectManagerName, 'Project Manager');
        if (owners?.techLeadName) addOption(owners.techLeadName, 'Technical Lead');

        leadership?.forEach((u: any) => addOption(u.fullName, 'Leadership'));
        projectManagers?.forEach((u: any) => addOption(u.fullName, 'Project Manager'));
        techLeads?.forEach((u: any) => addOption(u.fullName, 'Technical Lead'));
        preSales?.forEach((u: any) => addOption(u.fullName, 'PreSales'));

        this.ownerOptions.set([
          { value: '', label: '-- Select Owner --' },
          ...options
        ]);
      }
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.dialogRef.close(this.form.getRawValue());
  }
}
