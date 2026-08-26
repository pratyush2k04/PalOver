import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DependencyDto, CreateDependencyDto, UpdateDependencyDto } from '../models/dependency.model';
import { DEPENDENCY_API_ENDPOINTS } from '@shared/constants/app.constants';

@Injectable({
  providedIn: 'root'
})
export class DependencyService {
  private http = inject(HttpClient);
  
  getDependencies(engagementId: number): Observable<DependencyDto[]> {
    return this.http.get<DependencyDto[]>(DEPENDENCY_API_ENDPOINTS.GET_DEPENDENCIES(engagementId));
  }

  createDependency(engagementId: number, data: CreateDependencyDto): Observable<DependencyDto> {
    return this.http.post<DependencyDto>(DEPENDENCY_API_ENDPOINTS.CREATE_DEPENDENCY(engagementId), data);
  }

  updateDependency(engagementId: number, id: number, data: UpdateDependencyDto): Observable<DependencyDto> {
    return this.http.put<DependencyDto>(DEPENDENCY_API_ENDPOINTS.UPDATE_DEPENDENCY(engagementId, id), data);
  }
}
