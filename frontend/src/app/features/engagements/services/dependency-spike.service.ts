import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TECHNICAL_READINESS_API_ENDPOINTS } from '@shared/constants/technical-readiness.constants';
import {
  DependencySpikeDto,
  CreateDependencySpikeRequest,
  UpdateDependencySpikeRequest
} from '../models/dependency-spikes/dependency-spike.model';

@Injectable({
  providedIn: 'root'
})
export class DependencySpikeService {
  private http = inject(HttpClient);

  getSpikes(engagementId: number): Observable<DependencySpikeDto[]> {
    return this.http.get<DependencySpikeDto[]>(
      TECHNICAL_READINESS_API_ENDPOINTS.DEPENDENCY_SPIKES(engagementId)
    );
  }

  createSpike(
    engagementId: number,
    request: CreateDependencySpikeRequest
  ): Observable<DependencySpikeDto> {
    return this.http.post<DependencySpikeDto>(
      TECHNICAL_READINESS_API_ENDPOINTS.DEPENDENCY_SPIKES(engagementId),
      request
    );
  }

  updateSpike(
    engagementId: number,
    id: number,
    request: UpdateDependencySpikeRequest
  ): Observable<DependencySpikeDto> {
    return this.http.put<DependencySpikeDto>(
      TECHNICAL_READINESS_API_ENDPOINTS.DEPENDENCY_SPIKE(engagementId, id),
      request
    );
  }

  deleteSpike(engagementId: number, id: number): Observable<void> {
    return this.http.delete<void>(
      TECHNICAL_READINESS_API_ENDPOINTS.DEPENDENCY_SPIKE(engagementId, id)
    );
  }
}
