import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { AddItemRequest, CartResponse, UpdateItemRequest } from './models';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  private apiUrl = `${Environment.apiBaseUrl}/cart`
  private http = inject(HttpClient)

  public GetCart(): Observable<CartResponse> {
    return this.http.get<CartResponse>(this.apiUrl, { withCredentials: true });
  }

  public AddItem(dto: AddItemRequest): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/items`, dto, { withCredentials: true });
  }

  public UpdateItemQuantity(productVariantId: number, dto: UpdateItemRequest): Observable<CartResponse> {
    return this.http.put<CartResponse>(`${this.apiUrl}/items/${productVariantId}`, dto, { withCredentials: true });
  }

  public RemoveItem(productVariantId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/items/${productVariantId}`, { withCredentials: true });
  }

  public ClearCart(): Observable<void> {
    return this.http.delete<void>(this.apiUrl, { withCredentials: true });
  }
}
