import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth';

@Component({
  imports: [FormsModule, RouterLink],
  selector: 'app-login',
  styleUrl: './login.scss',
  templateUrl: './login.html',
})
export class Login {
  email = '';
  password = '';
  errorMessage = '';
  isSubmitting = false;

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router,
  ) {}

  submit(): void{
    this.errorMessage = '';

    if (!this.email.trim()){
      this.errorMessage = 'Email is required';
      return;
    }

    if (!this.password.trim()){
      this.errorMessage = 'Password is required';
      return;
    }

    this.isSubmitting = true;

    this.authService
      .login({
        email: this.email,
        password: this.password,
      })
      .subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['/dashboard']);
        },
        error: () => {
          this.isSubmitting = false;
          this.errorMessage = 'Invalid email or password';
        },
      });
  }
}
