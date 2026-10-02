import { Component } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-shell',
  imports: [RouterLink, RouterOutlet],
  styleUrl: './shell.scss',
  templateUrl: './shell.html',
})
export class Shell {}
