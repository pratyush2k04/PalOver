import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SEARCH_API_ENDPOINTS } from '@shared/constants/search.constants';
import { GlobalSearchResponse, SearchType } from '@shared/models/global-search.model';

@Injectable({
  providedIn: 'root'
})
export class GlobalSearchService {
  private http = inject(HttpClient);

  search(query: string, types?: SearchType[]): Observable<GlobalSearchResponse> {
    return this.http.get<GlobalSearchResponse>(SEARCH_API_ENDPOINTS.GLOBAL(query, types));
  }
}
