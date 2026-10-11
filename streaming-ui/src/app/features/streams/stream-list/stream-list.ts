import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { StreamService } from '../../../core/services/stream';
import { StreamListItem } from '../../../shared/models/stream-list-item';
import { StreamCard } from '../../../shared/components/stream-card/stream-card';
import {
  SortDirection,
  StreamSortField,
} from '../../../shared/models/get-streams-params';

@Component({
  selector: 'app-stream-list',
  imports: [FormsModule, StreamCard],
  templateUrl: './stream-list.html',
  styleUrl: './stream-list.scss',
})
export class StreamList {
  streams: StreamListItem[] = [];
  isLoading = true;
  errorMessage = '';
  search = '';
  category = '';
  sortBy: StreamSortField = 'startTime';
  sortDirection: SortDirection = 'desc';
  currentPage = 1;
  readonly pageSize = 12;
  totalCount = 0;
  totalPages = 0;

  constructor(private readonly streamService: StreamService) {
    this.loadStreams();
  }

  loadStreams(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.streamService.getStreams({
      page: this.currentPage,
      pageSize: this.pageSize,
      category: this.category,
      search: this.search,
      sortBy: this.sortBy,
      sortDirection: this.sortDirection,
    }).subscribe({
      next: (response) => {
        this.streams = response.items;
        this.currentPage = response.currentPage;
        this.totalCount = response.totalCount;
        this.totalPages = response.totalPages;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Failed to load streams', error);
        this.streams = [];
        this.totalCount = 0;
        this.totalPages = 0;
        this.errorMessage = 'Failed to load streams';
        this.isLoading = false;
      },
    });
  }

  applyFilters(): void {
    this.currentPage = 1;
    this.loadStreams();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages || page === this.currentPage) {
      return;
    }

    this.currentPage = page;
    this.loadStreams();
  }
}
