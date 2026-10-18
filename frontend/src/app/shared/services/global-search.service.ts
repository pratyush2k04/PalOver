import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpContext } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SEARCH_API_ENDPOINTS } from '@shared/constants/search.constants';
import { GlobalSearchResponse, SearchType } from '@shared/models/global-search.model';
import { SKIP_LOADER } from '@shared/interceptors/loader-interceptor';

@Injectable({
  providedIn: 'root'
})
export class GlobalSearchService {
  private http = inject(HttpClient);

  search(query: string, types?: SearchType[]): Observable<GlobalSearchResponse> {
    return this.http.get<GlobalSearchResponse>(SEARCH_API_ENDPOINTS.GLOBAL(query, types), {
      context: new HttpContext().set(SKIP_LOADER, true)
    });
  }
}
