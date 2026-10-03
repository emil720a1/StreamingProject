import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-register',
  imports: [FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register {
  username = '';
  email = '';
  password = '';
  confirmPassword = '';
  errorMessage = '';

  submit(): void {
    this.errorMessage = '';

    if (!this.username.trim()){
      this.errorMessage = 'Username is required';
      return;
    }

    if (!this.email.trim()){
      this.errorMessage = 'Email is required';
      return;
    }

    if (!this.password.trim()) {
      this.errorMessage = 'Password is required';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.errorMessage = 'Passwords do not match';
      return;
    }

    console.log('Registration submitted', {
      username: this.username,
      email: this.email,
      password: this.password,
    })
  }
}
