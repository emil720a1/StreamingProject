import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Stream } from '../../../shared/models/stream';
import { StreamService } from '../../../core/services/stream';

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard {
  streams: Stream[];

  constructor(private readonly streamService: StreamService) {
    this.streams = this.streamService.getStreams();
  }

  getTotalViewers(): number {
    return this.streams.reduce(
      (total, stream) => total + stream.viewers,
      0,
    );
  }

  getCategoryCount(): number {
    return new Set(
      this.streams.map((stream) => stream.category),
    ).size;
  }
}
