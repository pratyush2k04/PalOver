import { Injectable, inject, signal } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import {
  ChecklistCategory,
  ChecklistItem,
  ChecklistVersion,
  ChecklistVersionSummary,
  CreateChecklistItemRequest,
  ReorderChecklistItemsRequest,
  UpdateChecklistItemRequest,
} from './checklist.model';

@Injectable({
  providedIn: 'root',
})
export class ChecklistService {
  private http = inject(HttpClient);

  private readonly publishedLoadingSignal = signal(false);
  private readonly publishedErrorSignal = signal<string | null>(null);
  private readonly publishedNotFoundSignal = signal(false);
  private readonly publishedChecklistSignal = signal<ChecklistVersion | null>(null);

  readonly publishedLoading = this.publishedLoadingSignal.asReadonly();
  readonly publishedError = this.publishedErrorSignal.asReadonly();
  readonly publishedNotFound = this.publishedNotFoundSignal.asReadonly();
  readonly publishedChecklist = this.publishedChecklistSignal.asReadonly();

  private readonly draftLoadingSignal = signal(false);
  private readonly draftErrorSignal = signal<string | null>(null);
  private readonly draftNotFoundSignal = signal(false);
  private readonly currentDraftSignal = signal<ChecklistVersion | null>(null);

  readonly draftLoading = this.draftLoadingSignal.asReadonly();
  readonly draftError = this.draftErrorSignal.asReadonly();
  readonly draftNotFound = this.draftNotFoundSignal.asReadonly();
  readonly currentDraft = this.currentDraftSignal.asReadonly();

  private readonly categoriesSignal = signal<ChecklistCategory[]>([]);
  readonly categories = this.categoriesSignal.asReadonly();

  private readonly versionsLoadingSignal = signal(false);
  private readonly versionsErrorSignal = signal<string | null>(null);
  private readonly versionsSignal = signal<ChecklistVersionSummary[]>([]);

  readonly versionsLoading = this.versionsLoadingSignal.asReadonly();
  readonly versionsError = this.versionsErrorSignal.asReadonly();
  readonly versions = this.versionsSignal.asReadonly();

  getPublished(): void {
    this.publishedLoadingSignal.set(true);
    this.publishedErrorSignal.set(null);
    this.publishedNotFoundSignal.set(false);

    this.http.get<ChecklistVersion>('/api/checklist/published').pipe(
      tap({
        next: (version) => {
          this.publishedChecklistSignal.set(version);
          this.publishedLoadingSignal.set(false);
        },
        error: (err: HttpErrorResponse) => {
          this.publishedChecklistSignal.set(null);
          if (err.status === 404) {
            this.publishedNotFoundSignal.set(true);
          } else {
            this.publishedErrorSignal.set('Unable to load the published checklist. Please try again.');
          }
          this.publishedLoadingSignal.set(false);
        },
      })
    ).subscribe();
  }

  getCurrentDraft(): void {
    this.draftLoadingSignal.set(true);
    this.draftErrorSignal.set(null);
    this.draftNotFoundSignal.set(false);

    this.http.get<ChecklistVersion>('/api/checklist/current-draft').pipe(
      tap({
        next: (version) => {
          this.currentDraftSignal.set(version);
          this.draftLoadingSignal.set(false);
        },
        error: (err: HttpErrorResponse) => {
          this.currentDraftSignal.set(null);
          if (err.status === 404) {
            this.draftNotFoundSignal.set(true);
          } else {
            this.draftErrorSignal.set('Unable to load the current draft. Please try again.');
          }
          this.draftLoadingSignal.set(false);
        },
      })
    ).subscribe();
  }

  loadCategories(): void {
    this.http.get<ChecklistCategory[]>('/api/checklist/categories').subscribe({
      next: (categories) => this.categoriesSignal.set(categories),
    });
  }

  getVersionHistory(): void {
    this.versionsLoadingSignal.set(true);
    this.versionsErrorSignal.set(null);

    this.http.get<ChecklistVersionSummary[]>('/api/checklist/versions').pipe(
      tap({
        next: (versions) => {
          this.versionsSignal.set(versions);
          this.versionsLoadingSignal.set(false);
        },
        error: () => {
          this.versionsErrorSignal.set('Unable to load version history. Please try again.');
          this.versionsLoadingSignal.set(false);
        },
      })
    ).subscribe();
  }

  getVersionById(versionId: number): Observable<ChecklistVersion> {
    return this.http.get<ChecklistVersion>(`/api/checklist/versions/${versionId}`);
  }

  fetchPublished(): Observable<ChecklistVersion> {
    return this.http.get<ChecklistVersion>('/api/checklist/published');
  }

  fetchCurrentDraft(): Observable<ChecklistVersion> {
    return this.http.get<ChecklistVersion>('/api/checklist/current-draft');
  }

  fetchCategories(): Observable<ChecklistCategory[]> {
    return this.http.get<ChecklistCategory[]>('/api/checklist/categories');
  }

  createItem(request: CreateChecklistItemRequest): Observable<ChecklistItem> {
    return this.http.post<ChecklistItem>('/api/checklist/items', request);
  }

  updateItem(id: number, request: UpdateChecklistItemRequest): Observable<ChecklistItem> {
    return this.http.put<ChecklistItem>(`/api/checklist/items/${id}`, request);
  }

  retireItem(id: number): Observable<ChecklistItem> {
    return this.http.put<ChecklistItem>(`/api/checklist/items/${id}/retire`, {});
  }

  reorderItems(request: ReorderChecklistItemsRequest): Observable<ChecklistItem[]> {
    return this.http.put<ChecklistItem[]>('/api/checklist/items/reorder', request);
  }

  publishVersion(versionId: number): Observable<ChecklistVersion> {
    return this.http.post<ChecklistVersion>(`/api/checklist/versions/${versionId}/publish`, {});
  }

  setCurrentDraft(version: ChecklistVersion): void {
    this.currentDraftSignal.set(version);
  }
}
