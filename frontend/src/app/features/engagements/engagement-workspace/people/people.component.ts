import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { PersonService } from '@shared/services/person.service';
import { PersonDto, PersonType } from '@shared/models/person.model';
import { LoaderService } from '@shared/services/loader.service';
import { ToastService } from '@common/services/toast/toast';
import { ConfirmDialogComponent } from '@common/components/confirm-dialog/confirm-dialog';
import { ButtonComponent } from '@common/components/button/button';
import { SectionHeaderComponent } from '@common/components/section-header/section-header';
import { TooltipDirective } from '@common/directives/tooltip.directive';
import { DialogService } from '@common/services/dialog/dialog.service';
import { AuthService } from '@shared/services/auth';
import { USER_ROLES } from '@shared/constants/app.constants';
import {
  PersonFormDialogComponent,
  PersonFormDialogData,
  PersonFormDialogResult
} from './dialogs/person-form-dialog/person-form-dialog.component';

@Component({
  selector: 'app-people',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ConfirmDialogComponent,
    InlineSVGModule,
    ButtonComponent,
    SectionHeaderComponent,
    TooltipDirective,
  ],
  templateUrl: './people.component.html',
  styleUrl: './people.component.scss'
})
export class PeopleComponent {
  private route = inject(ActivatedRoute);
  private personService = inject(PersonService);
  private toastService = inject(ToastService);
  private dialog = inject(DialogService);
  public loaderService = inject(LoaderService);
  private authService = inject(AuthService);
  private engagementService = inject(EngagementService);

  private paramMap = toSignal(this.route.parent?.paramMap ?? this.route.paramMap);

  engagementId = computed(() => {
    const idStr = this.paramMap()?.get('id');
    return idStr ? +idStr : null;
  });

  people = signal<PersonDto[]>([]);
  isDeleting = signal(false);
  personToDelete = signal<PersonDto | null>(null);
  isImporting = signal(false);
  
  isLeadership = computed(() => this.authService.hasRole(USER_ROLES.LEADERSHIP));
  canManage = computed(() => !this.isLeadership() && !this.engagementService.isSignedOff());

  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.loadPeople(id);

        if (this.route.snapshot.queryParamMap.get('action') === 'create') {
          this.onAddPerson();
        }
      }
    });
  }

  private loadPeople(engagementId: number): void {
    this.loaderService.show();
    this.personService.getPeopleByEngagement(engagementId).subscribe({
      next: (data) => {
        this.people.set(data ?? []);
        this.loaderService.hide();
      },
      error: (err) => {
        console.error('Error fetching people directory:', err);
        this.loaderService.hide();
      }
    });
  }

  getOrgLabel(personType: PersonType | string | number): string {
    const num = Number(personType);
    if (personType === PersonType.Client || personType === 'Client' || num === 2) {
      return 'Client';
    }
    if (personType === PersonType.Internal || personType === 'Internal' || num === 1) {
      return 'Internal';
    }
    return String(personType);
  }

  onAddPerson(): void {
    this.openPersonFormDialog(null);
  }

  onEditPerson(person: PersonDto): void {
    this.openPersonFormDialog(person);
  }

  private openPersonFormDialog(person: PersonDto | null): void {
    const engagementId = this.engagementId();
    if (!engagementId) return;

    const dialogRef = this.dialog.open<PersonFormDialogComponent, PersonFormDialogData, PersonFormDialogResult>(
      PersonFormDialogComponent,
      { data: { person } }
    );

    dialogRef.closed$.subscribe((result) => {
      if (!result) return;

      const payload: Partial<PersonDto> = {
        engagementId,
        name: result.name,
        role: result.role,
        personType: result.personType
      };

      if (person) {
        this.personService.updatePerson(engagementId, person.personId, { ...payload, personId: person.personId }).subscribe({
          next: () => {
            this.toastService.success('Contact updated successfully');
            this.loadPeople(engagementId);
          }
        });
      } else {
        this.personService.createPerson(engagementId, payload).subscribe({
          next: () => {
            this.toastService.success('Contact added successfully');
            this.loadPeople(engagementId);
          }
        });
      }
    });
  }

  onDeletePerson(person: PersonDto): void {
    this.personToDelete.set(person);
  }

  cancelDelete(): void {
    this.personToDelete.set(null);
  }

  confirmDelete(): void {
    const person = this.personToDelete();
    const engagementId = this.engagementId();

    if (!person || !engagementId) return;

    this.isDeleting.set(true);
    this.personService.deletePerson(engagementId, person.personId).subscribe({
      next: () => {
        this.toastService.success(`Removed ${person.name} from directory`);
        this.isDeleting.set(false);
        this.personToDelete.set(null);
        this.loadPeople(engagementId);
      },
      error: () => {
        this.isDeleting.set(false);
      }
    });
  }

  onImportFromHandover(): void {
    const engagementId = this.engagementId();
    if (!engagementId) return;

    this.isImporting.set(true);
    this.personService.importFromHandover(engagementId).subscribe({
      next: (response) => {
        const messages: string[] = [];
        if (response.added > 0) messages.push(`${response.added} added`);
        if (response.alreadyPresent > 0) messages.push(`${response.alreadyPresent} already present`);
        if (response.deleted > 0) messages.push(`${response.deleted} deleted`);

        const toastMessage = messages.length > 0 ? messages.join(', ') : 'No contacts imported';
        this.toastService.success(toastMessage);
        this.isImporting.set(false);
        this.loadPeople(engagementId);
      },
      error: (err) => {
        console.error('Error importing contacts:', err);
        this.toastService.error('Failed to import contacts from handover');
        this.isImporting.set(false);
      }
    });
  }
}
