import { Component } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Stream } from '../../../shared/models/stream';
import { StreamService } from '../../../core/services/stream';

@Component({
  selector: 'app-stream-details',
  imports: [RouterLink],
  styleUrl: './stream-details.scss',
  templateUrl: './stream-details.html',
})
export class StreamDetails {
  stream?: Stream;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly streamService: StreamService,
  ){
    const id = Number(
      this.route.snapshot.paramMap.get('id'),
    );

    this.stream = this.streamService.getStreamById(id);
  }
}
