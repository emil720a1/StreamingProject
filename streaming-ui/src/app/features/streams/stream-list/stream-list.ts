import { Component } from '@angular/core';
import { Stream } from '../../../shared/models/stream';
import { StreamService } from '../../../core/services/stream';
import { StreamCard } from '../../../shared/components/stream-card/stream-card';

@Component({
  selector: 'app-stream-list',
  imports: [StreamCard],
  templateUrl: './stream-list.html',
  styleUrl: './stream-list.scss',
})
export class StreamList {
  streams: Stream[];
  selectedCategory = 'All';

  constructor(private readonly streamService: StreamService) {
    this.streams = this.streamService.getStreams();
  }

  get categories(): string[] {
    return [
      'All',
      ...new Set(
        this.streams.map((stream) => stream.category),
      ),
    ];
  }

  get filteredStreams(): Stream[] {
    if (this.selectedCategory === 'All') {
      return this.streams;
    }

    return this.streams.filter(
      (stream) => stream.category === this.selectedCategory,
    );
  }

  selectCategory(category: string): void {
    this.selectedCategory = category;
  }
}
