import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PersonDto } from '../models/person.model';

@Injectable({
  providedIn: 'root'
})
export class PersonService {
  private http = inject(HttpClient);

  getPeopleByEngagement(engagementId: number): Observable<PersonDto[]> {
    return this.http.get<PersonDto[]>(`/api/engagements/${engagementId}/people`);
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
}
