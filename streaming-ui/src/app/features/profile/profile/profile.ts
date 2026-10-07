import { Component } from '@angular/core';

import { StreamService } from '../../../core/services/stream';
import { Stream } from '../../../shared/models/stream';

@Component({
  imports: [],
  selector: 'app-profile',
  styleUrl: './profile.scss',
  templateUrl: './profile.html',
})
export class Profile {
  streams: Stream[] = [];
  isLoading = true;
  errorMessage = '';

  constructor(private readonly streamService: StreamService){
    this.loadStreams();
  }
  loadStreams(): void {
    this.streamService
      .getCurrentUserStreams()
      .subscribe(
      {
        next: (streams) => {
          this.streams = streams;
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Failed to load user stream', error);
          this.errorMessage = 'Failed to load your streams';
          this.isLoading = false;
        },
      });
  }
}
