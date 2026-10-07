import { Component } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { StreamDetails as StreamDetailsModel } from '../../../shared/models/stream-details';
import { StreamService } from '../../../core/services/stream';

@Component({
  selector: 'app-stream-details',
  imports: [RouterLink],
  styleUrl: './stream-details.scss',
  templateUrl: './stream-details.html',
})
export class StreamDetails {
  stream?: StreamDetailsModel;
  isLoading = true;
  errorMessage = '';

  constructor(
    private readonly route: ActivatedRoute,
    private readonly streamService: StreamService,
  ) {
    this.loadStream();
  }

  private loadStream(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      this.errorMessage = 'Stream id is missing';
      this.isLoading = false;
      return;
    }

    this.streamService.getStreamById(id).subscribe({
      next: (stream) => {
        this.stream = stream;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Failed to load stream', error);
        this.errorMessage = 'Stream not found';
        this.isLoading = false;
      },
    });
  }
}
