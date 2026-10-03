import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

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

    console.log('Login submitted', {
      email: this.email,
      password: this.password,
    });
  }
}
