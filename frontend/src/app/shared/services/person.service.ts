import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpContext } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PersonDto } from '@shared/models/person.model';
import { SKIP_LOADER } from '@shared/interceptors/loader-interceptor';

@Injectable({
  providedIn: 'root'
})
export class PersonService {
  private http = inject(HttpClient);

  getPeopleByEngagement(engagementId: number, skipLoader = false): Observable<PersonDto[]> {
    return this.http.get<PersonDto[]>(`/api/engagements/${engagementId}/people`, {
      context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined
    });
  }

  createPerson(engagementId: number, person: Partial<PersonDto>): Observable<PersonDto> {
    return this.http.post<PersonDto>(`/api/engagements/${engagementId}/people`, person);
  }

  updatePerson(engagementId: number, personId: number, person: Partial<PersonDto>): Observable<PersonDto> {
    return this.http.put<PersonDto>(`/api/engagements/${engagementId}/people/${personId}`, person);
  }

  deletePerson(engagementId: number, personId: number): Observable<void> {
    return this.http.delete<void>(`/api/engagements/${engagementId}/people/${personId}`);
  }

  importFromHandover(engagementId: number): Observable<ImportResponse> {
    return this.http.post<ImportResponse>(`/api/engagements/${engagementId}/people/import-from-handover`, {});
  }
}

export interface ImportResponse {
  added: number;
  alreadyPresent: number;
  deleted: number;
}
