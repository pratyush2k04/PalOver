import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AssumptionDto, CreateAssumptionDto, UpdateAssumptionDto, ValidateAssumptionDto } from '../models/assumption.model';
import { ASSUMPTION_API_ENDPOINTS } from '@shared/constants/app.constants';

@Injectable({
  providedIn: 'root'
})
export class AssumptionService {
  private http = inject(HttpClient);
  
  getAssumptions(engagementId: number): Observable<AssumptionDto[]> {
    return this.http.get<AssumptionDto[]>(ASSUMPTION_API_ENDPOINTS.GET_ASSUMPTIONS(engagementId));
  }

  createAssumption(engagementId: number, data: CreateAssumptionDto): Observable<AssumptionDto> {
    return this.http.post<AssumptionDto>(ASSUMPTION_API_ENDPOINTS.CREATE_ASSUMPTION(engagementId), data);
  }

  updateAssumption(engagementId: number, id: number, data: UpdateAssumptionDto): Observable<AssumptionDto> {
    return this.http.put<AssumptionDto>(ASSUMPTION_API_ENDPOINTS.UPDATE_ASSUMPTION(engagementId, id), data);
  }

  validateAssumption(engagementId: number, id: number, data: ValidateAssumptionDto): Observable<AssumptionDto> {
    return this.http.put<AssumptionDto>(ASSUMPTION_API_ENDPOINTS.VALIDATE_ASSUMPTION(engagementId, id), data);
  }
}
