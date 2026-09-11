import { Injectable, inject, signal } from '@angular/core';
import { HttpClient, HttpContext, HttpErrorResponse } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { SKIP_LOADER } from '@shared/interceptors/loader-interceptor';
import {
  MasterChecklistCategory,
  MasterChecklistItem,
  MasterChecklistVersion,
  MasterChecklistVersionSummary,
  CreateMasterChecklistItemRequest,
  ReorderMasterChecklistItemsRequest,
  UpdateMasterChecklistItemRequest,
} from '../models/master-checklist.model';

@Injectable({
  providedIn: 'root',
})
export class MasterChecklistService {
  private http = inject(HttpClient);

  private readonly publishedLoadingSignal = signal(false);
  private readonly publishedErrorSignal = signal<string | null>(null);
  private readonly publishedNotFoundSignal = signal(false);
  private readonly publishedChecklistSignal = signal<MasterChecklistVersion | null>(null);

  readonly publishedLoading = this.publishedLoadingSignal.asReadonly();
  readonly publishedError = this.publishedErrorSignal.asReadonly();
  readonly publishedNotFound = this.publishedNotFoundSignal.asReadonly();
  readonly publishedChecklist = this.publishedChecklistSignal.asReadonly();

  private readonly draftLoadingSignal = signal(false);
  private readonly draftErrorSignal = signal<string | null>(null);
  private readonly draftNotFoundSignal = signal(false);
  private readonly currentDraftSignal = signal<MasterChecklistVersion | null>(null);

  readonly draftLoading = this.draftLoadingSignal.asReadonly();
  readonly draftError = this.draftErrorSignal.asReadonly();
  readonly draftNotFound = this.draftNotFoundSignal.asReadonly();
  readonly currentDraft = this.currentDraftSignal.asReadonly();

  private readonly categoriesSignal = signal<MasterChecklistCategory[]>([]);
  readonly categories = this.categoriesSignal.asReadonly();

  private readonly versionsLoadingSignal = signal(false);
  private readonly versionsErrorSignal = signal<string | null>(null);
  private readonly versionsSignal = signal<MasterChecklistVersionSummary[]>([]);

  readonly versionsLoading = this.versionsLoadingSignal.asReadonly();
  readonly versionsError = this.versionsErrorSignal.asReadonly();
  readonly versions = this.versionsSignal.asReadonly();

  getPublished(): void {
    this.publishedLoadingSignal.set(true);
    this.publishedErrorSignal.set(null);
    this.publishedNotFoundSignal.set(false);

    this.http.get<MasterChecklistVersion>('/api/masterchecklist/published').pipe(
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

    this.http.get<MasterChecklistVersion>('/api/masterchecklist/current-draft').pipe(
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
    this.http.get<MasterChecklistCategory[]>('/api/masterchecklist/categories').subscribe({
      next: (categories) => this.categoriesSignal.set(categories),
    });
  }

  getVersionHistory(): void {
    this.versionsLoadingSignal.set(true);
    this.versionsErrorSignal.set(null);

    this.http.get<MasterChecklistVersionSummary[]>('/api/masterchecklist/versions').pipe(
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

  getVersionById(versionId: number): Observable<MasterChecklistVersion> {
    return this.http.get<MasterChecklistVersion>(`/api/masterchecklist/versions/${versionId}`, {
      context: new HttpContext().set(SKIP_LOADER, true)
    });
  }

  createItem(request: CreateMasterChecklistItemRequest): Observable<MasterChecklistItem> {
    return this.http.post<MasterChecklistItem>('/api/masterchecklist/items', request);
  }

  updateItem(id: number, request: UpdateMasterChecklistItemRequest): Observable<MasterChecklistItem> {
    return this.http.put<MasterChecklistItem>(`/api/masterchecklist/items/${id}`, request);
  }

  retireItem(id: number): Observable<MasterChecklistItem> {
    return this.http.put<MasterChecklistItem>(`/api/masterchecklist/items/${id}/retire`, {});
  }

  reorderItems(request: ReorderMasterChecklistItemsRequest): Observable<MasterChecklistItem[]> {
    return this.http.put<MasterChecklistItem[]>('/api/masterchecklist/items/reorder', request);
  }

  publishVersion(versionId: number): Observable<MasterChecklistVersion> {
    return this.http.post<MasterChecklistVersion>(`/api/masterchecklist/versions/${versionId}/publish`, {});
  }
}
