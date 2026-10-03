import { Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Stream } from '../../models/stream';

@Component({
  selector: 'app-stream-card',
  imports: [RouterLink],
  templateUrl: './stream-card.html',
  styleUrl: './stream-card.scss',
})
export class StreamCard {
  @Input({ required: true }) stream!: Stream;
}
