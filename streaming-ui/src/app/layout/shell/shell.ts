import { Component } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/services/auth';

@Component({
  selector: 'app-shell',
  imports: [RouterLink, RouterOutlet],
  styleUrl: './shell.scss',
  templateUrl: './shell.html',
})
export class Shell {
  constructor(
    private readonly authService: AuthService,
    private readonly router: Router
  ) {}

  logout(): void{
    this.authService.logout().subscribe({
      next: () =>this.router.navigate(['/login']),
      error: () => this.router.navigate(['/login']),
    });
  }
}
