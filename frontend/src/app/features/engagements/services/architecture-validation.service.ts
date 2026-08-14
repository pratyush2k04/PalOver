import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TECHNICAL_READINESS_API_ENDPOINTS } from '@shared/constants/technical-readiness.constants';
import { ArchitectureValidationDto, DiscoveryFeatureItemDto } from '../models/architecture-validation/architecture-validation.dtos';
import {
  SaveArchitectureValidationRequest,
  CreateDiscoveryFeatureItemRequest,
  UpdateDiscoveryFeatureItemRequest
} from '../models/architecture-validation/architecture-validation.requests';

@Injectable({
  providedIn: 'root'
})
export class ArchitectureValidationService {
  private http = inject(HttpClient);

  getValidation(engagementId: number): Observable<ArchitectureValidationDto> {
    return this.http.get<ArchitectureValidationDto>(
      TECHNICAL_READINESS_API_ENDPOINTS.ARCHITECTURE_VALIDATION(engagementId)
    );
  }

  saveValidation(
    engagementId: number,
    request: SaveArchitectureValidationRequest
  ): Observable<ArchitectureValidationDto> {
    return this.http.put<ArchitectureValidationDto>(
      TECHNICAL_READINESS_API_ENDPOINTS.ARCHITECTURE_VALIDATION(engagementId),
      request
    );
  }

  addFeatureItem(
    engagementId: number,
    request: CreateDiscoveryFeatureItemRequest
  ): Observable<DiscoveryFeatureItemDto> {
    return this.http.post<DiscoveryFeatureItemDto>(
      TECHNICAL_READINESS_API_ENDPOINTS.ARCHITECTURE_VALIDATION_FEATURES(engagementId),
      request
    );
  }

  updateFeatureItem(
    featureId: number,
    request: UpdateDiscoveryFeatureItemRequest
  ): Observable<DiscoveryFeatureItemDto> {
    return this.http.put<DiscoveryFeatureItemDto>(
      TECHNICAL_READINESS_API_ENDPOINTS.ARCHITECTURE_VALIDATION_FEATURE_ITEM(featureId),
      request
    );
  }

  deleteFeatureItem(featureId: number): Observable<void> {
    return this.http.delete<void>(
      TECHNICAL_READINESS_API_ENDPOINTS.ARCHITECTURE_VALIDATION_FEATURE_ITEM(featureId)
    );
  }
}
