import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { PersonService } from '../../../../services/person.service';
import { PersonDto, PersonType } from '../../../../models/person.model';
import { LoaderService } from '../../../../core/loading/loader.service';
import { ToastService } from '@common/services/toast/toast';
import { ConfirmDialogComponent } from '@common/components/confirm-dialog/confirm-dialog';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';

@Component({
  selector: 'app-people',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ConfirmDialogComponent,
    InlineSVGModule,
    ButtonComponent,
    InputComponent,
    SelectComponent
  ],
  templateUrl: './people.component.html',
  styleUrl: './people.component.scss'
})
export class PeopleComponent {
  private route = inject(ActivatedRoute);
  private personService = inject(PersonService);
  private toastService = inject(ToastService);
  public loaderService = inject(LoaderService);

  private paramMap = toSignal(this.route.parent?.paramMap ?? this.route.paramMap);

  engagementId = computed(() => {
    const idStr = this.paramMap()?.get('id');
    return idStr ? +idStr : null;
  });

  people = signal<PersonDto[]>([]);
  isFormModalOpen = signal(false);
  isSubmitting = signal(false);
  isDeleting = signal(false);
  editingPerson = signal<PersonDto | null>(null);
  personToDelete = signal<PersonDto | null>(null);

  formName = signal('');
  formRole = signal('');
  formPersonType = signal<number>(PersonType.Internal);

  PersonTypeEnum = PersonType;

  readonly personTypeOptions: SelectOption[] = [
    { value: PersonType.Internal, label: 'Internal' },
    { value: PersonType.Client, label: 'Client' }
  ];

  isFormDirty = computed(() => {
    const edit = this.editingPerson();
    if (!edit) {
      return true;
    }
    const numType = Number(edit.personType);
    const originalType = numType === 2 || edit.personType === 'Client' || edit.personType === PersonType.Client
      ? PersonType.Client
      : PersonType.Internal;
    return (
      this.formName().trim() !== edit.name ||
      this.formRole().trim() !== edit.role ||
      this.formPersonType() !== originalType
    );
  });

  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.loadPeople(id);
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
    this.editingPerson.set(null);
    this.formName.set('');
    this.formRole.set('');
    this.formPersonType.set(PersonType.Internal);
    this.isFormModalOpen.set(true);
  }

  onEditPerson(person: PersonDto): void {
    this.editingPerson.set(person);
    this.formName.set(person.name);
    this.formRole.set(person.role);
    const numType = Number(person.personType);
    const pType = numType === 2 || person.personType === 'Client' || person.personType === PersonType.Client 
      ? PersonType.Client 
      : PersonType.Internal;
    this.formPersonType.set(pType);
    this.isFormModalOpen.set(true);
  }

  closeFormModal(): void {
    this.isFormModalOpen.set(false);
    this.editingPerson.set(null);
  }

  onSavePerson(): void {
    const name = this.formName().trim();
    const role = this.formRole().trim();
    const engagementId = this.engagementId();

    if (!name || !role || !engagementId) {
      return;
    }

    const payload: Partial<PersonDto> = {
      engagementId,
      name,
      role,
      personType: Number(this.formPersonType())
    };

    this.isSubmitting.set(true);
    const edit = this.editingPerson();

    if (edit) {
      this.personService.updatePerson(engagementId, edit.personId, { ...payload, personId: edit.personId }).subscribe({
        next: () => {
          this.toastService.success('Contact updated successfully');
          this.isSubmitting.set(false);
          this.closeFormModal();
          this.loadPeople(engagementId);
        },
        error: () => {
          this.isSubmitting.set(false);
        }
      });
    } else {
      this.personService.createPerson(engagementId, payload).subscribe({
        next: () => {
          this.toastService.success('Contact added successfully');
          this.isSubmitting.set(false);
          this.closeFormModal();
          this.loadPeople(engagementId);
        },
        error: () => {
          this.isSubmitting.set(false);
        }
      });
    }
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
}
