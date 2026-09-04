import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpContext } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DependencyDto, CreateDependencyDto, UpdateDependencyDto } from '../models/dependency.model';
import { DEPENDENCY_API_ENDPOINTS } from '@shared/constants/app.constants';
import { SKIP_LOADER } from '@shared/interceptors/loader-interceptor';

@Injectable({
  providedIn: 'root'
})
export class DependencyService {
  private http = inject(HttpClient);

  getDependencies(engagementId: number, skipLoader = false): Observable<DependencyDto[]> {
    return this.http.get<DependencyDto[]>(DEPENDENCY_API_ENDPOINTS.GET_DEPENDENCIES(engagementId), {
      context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined
    });
  }

  createDependency(engagementId: number, data: CreateDependencyDto): Observable<DependencyDto> {
    return this.http.post<DependencyDto>(DEPENDENCY_API_ENDPOINTS.CREATE_DEPENDENCY(engagementId), data);
  }

  updateDependency(engagementId: number, id: number, data: UpdateDependencyDto): Observable<DependencyDto> {
    return this.http.put<DependencyDto>(DEPENDENCY_API_ENDPOINTS.UPDATE_DEPENDENCY(engagementId, id), data);
  }
}
