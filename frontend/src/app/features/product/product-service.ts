import { inject, Injectable } from '@angular/core';
import { Environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedResult } from '../../shared/models/paged-result';
import { CreateProductDto, ProductResponse, ProductSearchFilter, UpdateProductDto } from './models';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private apiUrl = `${Environment.apiBaseUrl}/product`
  private http = inject(HttpClient)

  public getAllProducts(params: ProductSearchFilter): Observable<PagedResult<ProductResponse>> {
    return this.http.get<PagedResult<ProductResponse>>(this.apiUrl, {
      params: {
        search: params.search ?? '',
        CategoryIds: params.categoryIds,
        OptionIds: params.optionIds,
        Sort: params.sort,
        pageNumber: params.pageNumber,
        pageSize: params.pageSize
      }
    })
  }

  public getProductById(id: number): Observable<ProductResponse> {
    return this.http.get<ProductResponse>(`${this.apiUrl}/${id}`)
  }

  public createProduct(dto: CreateProductDto): Observable<ProductResponse> {
    return this.http.post<ProductResponse>(this.apiUrl, dto)
  }

  public updateProduct(id: number, dto: UpdateProductDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto)
  }

  public deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`)
  }
}
