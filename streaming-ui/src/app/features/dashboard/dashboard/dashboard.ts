import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard {
  streams = [
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
