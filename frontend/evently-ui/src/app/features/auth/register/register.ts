import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService, AuthResponse } from '../../../core/services/auth.service';
import { catchError, of, tap } from 'rxjs';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  errorMessage = '';
  isLoading = false;

  form = this.fb.nonNullable.group({
    fullName: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
  });

  register$ = of<AuthResponse | null>(null);

  onSubmit() {
    if (this.form.invalid) return;

    this.isLoading = true;
    this.errorMessage = '';

    const { fullName, email, password } = this.form.getRawValue();

    this.register$ = this.auth.register({ fullName, email, password }).pipe(
      tap((res) => {
        this.auth.saveToken(res.token);
        this.router.navigate(['/events']);
      }),
      catchError((err) => {
        this.errorMessage =
          err.error?.message || 'Registration failed. Try another email.';
        this.isLoading = false;
        return of(null);
      }),
      tap(() => (this.isLoading = false))
    );
  }
}