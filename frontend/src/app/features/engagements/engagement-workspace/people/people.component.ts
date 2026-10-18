import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { PersonService } from '@shared/services/person.service';
import { PersonDto, PersonType } from '@shared/models/person.model';
import { ToastService } from '@common/services/toast/toast';
import { ConfirmDialogComponent } from '@common/components/confirm-dialog/confirm-dialog';
import { ButtonComponent } from '@common/components/button/button';
import { SectionHeaderComponent } from '@common/components/section-header/section-header';
import { TooltipDirective } from '@common/directives/tooltip.directive';
import { DialogService } from '@common/services/dialog/dialog.service';
import { AuthService } from '@shared/services/auth';
import { EngagementService } from '../../services/engagement.service';
import { USER_ROLES } from '@shared/constants/app.constants';
import { ClientContactTransitionCardComponent } from './client-contact-transition-card/client-contact-transition-card.component';
import {
  PersonFormDialogComponent,
  PersonFormDialogData,
  PersonFormDialogResult
} from './dialogs/person-form-dialog/person-form-dialog.component';
import { ClientContactTransitionService } from '../../services/client-contact-transition.service';
import { TransitionResponse } from '../../models/client-contact-transition.model';

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
    ClientContactTransitionCardComponent,
  ],
  templateUrl: './people.component.html',
  styleUrl: './people.component.scss'
})
export class PeopleComponent {
  private route = inject(ActivatedRoute);
  private personService = inject(PersonService);
  private toastService = inject(ToastService);
  private dialog = inject(DialogService);
  private authService = inject(AuthService);
  private engagementService = inject(EngagementService);

  private paramMap = toSignal(this.route.parent?.paramMap ?? this.route.paramMap);

  engagementId = computed(() => {
    const idStr = this.paramMap()?.get('id');
    return idStr ? +idStr : null;
  });

  people = signal<PersonDto[]>([]);
  transitions = signal<TransitionResponse[]>([]);
  isLoading = signal(false);
  
  filteredPeople = computed(() => {
    const t = this.transitions();
    return this.people().filter(p => !t.some(x => x.outgoingContactName === p.name));
  });

  personToDelete = signal<PersonDto | null>(null);
  isImporting = signal(false);
  
  isLeadership = computed(() => this.authService.hasRole(USER_ROLES.LEADERSHIP));
  canManage = computed(() => !this.engagementService.isSignedOff());

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

  private loadPeople(engagementId: number, skipLoader = false, onDone?: () => void): void {
    if (skipLoader) {
      this.isLoading.set(true);
    }
    this.personService.getPeopleByEngagement(engagementId, skipLoader).subscribe({
      next: (data) => {
        this.people.set(data ?? []);
        if (skipLoader) {
          this.isLoading.set(false);
        }
        onDone?.();
      },
      error: (err) => {
        console.error('Error fetching people directory:', err);
        if (skipLoader) {
          this.isLoading.set(false);
        }
        onDone?.();
      }
    });
  }

  onTransitionsLoaded(data: TransitionResponse[]): void {
    this.transitions.set(data);
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

  canEditPerson(person: PersonDto): boolean {
    return this.canManage() && this.getOrgLabel(person.personType) !== 'Internal';
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

      this.isLoading.set(true);

      if (person) {
        this.personService.updatePerson(engagementId, person.personId, { ...payload, personId: person.personId }, true).subscribe({
          next: () => {
            this.loadPeople(engagementId, true, () => this.toastService.success('Contact updated successfully'));
          },
          error: () => this.isLoading.set(false)
        });
      } else {
        this.personService.createPerson(engagementId, payload, true).subscribe({
          next: () => {
            this.loadPeople(engagementId, true, () => this.toastService.success('Contact added successfully'));
          },
          error: () => this.isLoading.set(false)
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

    this.personToDelete.set(null);
    this.isLoading.set(true);
    this.personService.deletePerson(engagementId, person.personId, true).subscribe({
      next: () => {
        this.loadPeople(engagementId, true, () => this.toastService.success(`Removed ${person.name} from directory`));
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  onTransitionSaved(): void {
    const engagementId = this.engagementId();
    if (engagementId) {
      this.loadPeople(engagementId, true);
    }
  }
}
