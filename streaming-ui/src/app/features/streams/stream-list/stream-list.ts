import { Component } from '@angular/core';

import { StreamService } from '../../../core/services/stream';
import { StreamListItem } from '../../../shared/models/stream-list-item';
import { StreamCard } from '../../../shared/components/stream-card/stream-card';

@Component({
  selector: 'app-stream-list',
  imports: [StreamCard],
  templateUrl: './stream-list.html',
  styleUrl: './stream-list.scss',
})
export class StreamList {
  streams: StreamListItem[] = [];
  isLoading = true;
  errorMessage = '';

  constructor(private readonly streamService: StreamService) {
    this.loadStreams();
  }

  private loadStreams(): void {
    this.streamService.getStreams().subscribe({
      next: (streams) => {
        this.streams = streams;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Failed to load streams', error);
        this.errorMessage = 'Failed to load streams';
        this.isLoading = false;
      },
    });
  }

}
