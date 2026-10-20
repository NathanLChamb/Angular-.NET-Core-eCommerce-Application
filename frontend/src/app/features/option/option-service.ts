import { inject, Injectable } from '@angular/core';
import { Environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { PaginationParams } from '../../shared/models/pagination-params';
import { Observable } from 'rxjs';
import { PagedResult } from '../../shared/models/paged-result';
import { CreateOptionRequest, OptionResponse, UpdateOptionRequest } from './models';

@Injectable({
  providedIn: 'root',
})
export class OptionService {
  private apiUrl = `${Environment.apiBaseUrl}/option`
  private http = inject(HttpClient)

  public getAllOptions(params: PaginationParams): Observable<PagedResult<OptionResponse>> {
    return this.http.get<PagedResult<OptionResponse>>(this.apiUrl, {
      params: {
        pageNumber: params.pageNumber,
        pageSize: params.pageSize
      }
    })
  }

  public getOptionById(id: number): Observable<OptionResponse> {
    return this.http.get<OptionResponse>(`${this.apiUrl}/${id}`)
  }

  public createOption(dto: CreateOptionRequest): Observable<OptionResponse> {
    return this.http.post<OptionResponse>(this.apiUrl, dto)
  }

  public updateOption(id: number, dto: UpdateOptionRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto)
  }

  public deleteOption(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`)
  }
}
