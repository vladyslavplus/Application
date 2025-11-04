import { Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { toObservable } from '@angular/core/rxjs-interop';
import { jwtDecode } from 'jwt-decode';

export interface AuthResponse {
  token: string;
  expiresIn: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
}

interface JwtPayload {
  sub?: string;
  role?: string | string[];
  [key: string]: any;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;

  loggedIn = signal<boolean>(this.hasToken());

  get loggedIn$() {
    return toObservable(this.loggedIn);
  }

  constructor(private http: HttpClient, private router: Router) {}

  login(data: LoginRequest) {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, data);
  }

  register(data: RegisterRequest) {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, data);
  }

  saveToken(token: string) {
    localStorage.setItem('token', token);
    this.loggedIn.set(true);
  }

  logout() {
    localStorage.removeItem('token');
    this.loggedIn.set(false);
  }

  hasToken(): boolean {
    return !!localStorage.getItem('token');
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isAuthenticated(): boolean {
    return this.loggedIn();
  }

  getTokenUserId(): string | null {
    const token = this.getToken();
    if (!token) return null;

    try {
      const decoded = jwtDecode<JwtPayload>(token);
      return decoded.sub ?? null;
    } catch {
      return null;
    }
  }

  isAdmin(): boolean {
    const token = this.getToken();
    if (!token) return false;

    try {
      const decoded = jwtDecode<JwtPayload>(token);
      const roles = decoded.role;
      if (Array.isArray(roles)) return roles.includes('Admin');
      return roles === 'Admin';
    } catch {
      return false;
    }
  }
}
