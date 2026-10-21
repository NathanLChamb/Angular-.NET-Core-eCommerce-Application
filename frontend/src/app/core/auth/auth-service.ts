import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TokenService } from './token-service';
import { AuthResponse, LoginRequest, RegisterRequest, User } from './models';
import { Environment } from '../../../environments/environment';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
   private http = inject(HttpClient);
  private router = inject(Router);
  private tokenService = inject(TokenService);

  private currentUser = signal<User | null>(null);
  readonly isLoggedIn = computed(() => this.currentUser() !== null);
  user = this.currentUser.asReadonly();
  private authUrl = `${Environment.apiBaseUrl}/auth`;

  constructor() {

    if (this.tokenService.hasValidToken()) {

      const token = this.tokenService.decodeToken();

      if (token) {
        this.currentUser.set({
          id: token.sub ?? '',
          email: token.email ?? '',
          displayName: token.name,
          role: token["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]
        });
      }

    }
  }


  login(credentials: LoginRequest) {

    return this.http
      .post<AuthResponse>(`${this.authUrl}/login`, credentials, { withCredentials: true })
      .pipe(
        tap(response => {

          this.tokenService.setToken(response.token);

          const token = this.tokenService.decodeToken();

          if (token) {
            this.currentUser.set({
              id: token.sub ?? '',
              email: token.email ?? '',
              displayName: token.name,
              role:  token["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]
            });
          }
        })
      );
  }


  register(request: RegisterRequest) {

    return this.http
      .post<AuthResponse>(`${this.authUrl}/register`, request)
      .pipe(
        tap(response => {

          this.tokenService.setToken(response.token);

          const token = this.tokenService.decodeToken();

          if (token) {
            this.currentUser.set({
              id: token.sub ?? '',
              email: token.email ?? '',
              displayName: token.name,
              role:  token["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]
            });
          }

        })
      );
  }

  logout(): void {
    this.tokenService.removeToken();
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  isAdmin(): boolean {
    return this.currentUser()?.role === 'Admin';
  }

  isAuthenticated(): boolean {
    return this.tokenService.hasValidToken();
  }
}
