import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateNoteRequest, Note } from './note.model';

@Injectable({ providedIn: 'root' })
export class NotesService {
  private readonly http = inject(HttpClient);
  // Relative URL: in dev the ng-serve proxy forwards /api to the API;
  // in prod the .NET app serves this SPA and the API from the same origin.
  private readonly baseUrl = '/api/notes';

  getNotes(): Observable<Note[]> {
    return this.http.get<Note[]>(this.baseUrl);
  }

  createNote(request: CreateNoteRequest): Observable<Note> {
    return this.http.post<Note>(this.baseUrl, request);
  }
}
