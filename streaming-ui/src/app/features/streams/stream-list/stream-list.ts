import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Stream } from '../../../shared/models/stream';
import { StreamService } from '../../../core/services/stream';

@Component({
  selector: 'app-stream-list',
  imports: [RouterLink],
  templateUrl: './stream-list.html',
  styleUrl: './stream-list.scss',
})
export class StreamList {
  streams: Stream[];

  constructor(private readonly streamService: StreamService) {
    this.streams = this.streamService.getStreams();
  }
}
