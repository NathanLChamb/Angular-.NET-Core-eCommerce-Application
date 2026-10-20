import { inject, Injectable } from '@angular/core';
import { Environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedResult } from '../../shared/models/paged-result';
import { CategoryResponse, CreateCategoryRequest, UpdateCategoryRequest } from './models';
import { PaginationParams } from '../../shared/models/pagination-params';

@Injectable({
  providedIn: 'root',
})
export class CategoryService {
  private apiUrl = `${Environment.apiBaseUrl}/category`
  private http = inject(HttpClient)

  public getAllCategories(params: PaginationParams): Observable<PagedResult<CategoryResponse>> {
    return this.http.get<PagedResult<CategoryResponse>>(this.apiUrl, {
      params: {
        pageNumber: params.pageNumber,
        pageSize: params.pageSize
      }
    })
  }

  public getCategoryById(id: number): Observable<CategoryResponse> {
    return this.http.get<CategoryResponse>(`${this.apiUrl}/${id}`)
  }

  public createCategory(dto: CreateCategoryRequest): Observable<CategoryResponse> {
    return this.http.post<CategoryResponse>(this.apiUrl, dto)
  }

  public updateCategory(id: number, dto: UpdateCategoryRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto)
  }

  public deleteCategory(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`)
  }
}
