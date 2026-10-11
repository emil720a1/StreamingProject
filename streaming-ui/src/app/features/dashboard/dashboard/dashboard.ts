import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

import { StreamService } from '../../../core/services/stream';
import { StreamListItem } from '../../../shared/models/stream-list-item';
import { StreamCard } from '../../../shared/components/stream-card/stream-card';

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink, StreamCard],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard {
  streams: StreamListItem[] = [];
  totalCount = 0;
  isLoading = true;
  errorMessage = '';

  constructor(private readonly streamService: StreamService) {
    this.loadStreams();
  }

  loadStreams(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.streams = [];
    this.totalCount = 0;

    this.streamService.getStreams({ page: 1, pageSize: 12 }).subscribe({
      next: (response) => {
        this.streams = response.items;
        this.totalCount = response.totalCount;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Failed to load streams', error);
        this.totalCount = 0;
        this.errorMessage = 'Failed to load streams';
        this.isLoading = false;
      },
    });
  }
}
