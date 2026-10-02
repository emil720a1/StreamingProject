import { Injectable } from '@angular/core';
import { Stream } from '../../shared/models/stream';

@Injectable({
  providedIn: 'root',
})
export class StreamService {
  private readonly streams: Stream[] = [
    {
      id: 1,
      title: 'Gaming Live',
      author: 'Alex',
      viewers: 1240,
      category: 'Gaming',
      isLive: true,
    },
    {
      id: 2,
      title: 'Music Session',
      author: 'Maria',
      viewers: 532,
      category: 'Music',
      isLive: true,
    },
    {
      id: 3,
      title: 'Tech Talk',
      author: 'John',
      viewers: 0,
      category: 'Technology',
      isLive: false,
    },
  ];

  getStreams(): Stream[] {
    return this.streams;
  }

  getStreamById(id: number): Stream | undefined {
    return this.streams.find((stream) => stream.id === id);
  }
}
