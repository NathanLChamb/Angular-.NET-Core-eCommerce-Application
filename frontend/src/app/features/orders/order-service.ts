import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateOrderDto, OrderSearchFilter, ReadOrderDto, ReadOrderFromAdminDto } from './models';
import { HttpClient } from '@angular/common/http';
import { Environment } from '../../../environments/environment';
import { PagedResult } from '../../shared/models/paged-result';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  private apiUrl = `${Environment.apiBaseUrl}/orders`;
  private http = inject(HttpClient);

  getMyOrders(): Observable<ReadOrderDto[]> {
    return this.http.get<ReadOrderDto[]>(this.apiUrl);
  }

  getOrderById(id: number): Observable<ReadOrderDto> {
    return this.http.get<ReadOrderDto>(`${this.apiUrl}/${id}`);
  }

  getAdminOrderById(id: number) {
    return this.http.get<ReadOrderFromAdminDto>(`${this.apiUrl}/admin/${id}`);
  }

  createOrder(dto: CreateOrderDto): Observable<ReadOrderDto> {
    return this.http.post<ReadOrderDto>(this.apiUrl, dto);
  }

  cancelOrder(id: number): Observable<ReadOrderDto> {
    return this.http.post<ReadOrderDto>(`${this.apiUrl}/${id}/cancel`, {});
  }

  public getAllOrders(params: OrderSearchFilter): Observable<PagedResult<ReadOrderFromAdminDto>> {
  return this.http.get<PagedResult<ReadOrderFromAdminDto>>(`${this.apiUrl}/admin`, {
      params: {
        status: params.status,
        pageNumber: params.pageNumber,
        pageSize: params.pageSize
      }
    }
  );
}

  updateOrderStatus(id: number, status: number): Observable<ReadOrderDto> {
    return this.http.put<ReadOrderDto>(`${this.apiUrl}/${id}/status`, { status });
  }
}
