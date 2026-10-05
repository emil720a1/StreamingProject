import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';

import { StreamService } from '../../../core/services/stream';
import { StreamList } from './stream-list';

describe('StreamList', () => {
  const streams = [
    {
      id: 'stream-1',
      userId: 'user-1',
      streamerUsername: 'Alex',
      title: 'Gaming Live',
      description: 'Gaming stream',
      startTime: '2026-10-05T18:00:00Z',
    },
  ];

  let component: StreamList;
  let fixture: ComponentFixture<StreamList>;
  let streamsResponse = streams;

  beforeEach(async () => {
    streamsResponse = streams;

    await TestBed.configureTestingModule({
      imports: [StreamList],
      providers: [
        provideRouter([]),
        {
          provide: StreamService,
          useValue: {
            getStreams: () => of(streamsResponse),
          },
        },
      ],
    }).compileComponents();
  });

  it('should load streams from the service', () => {
    fixture = TestBed.createComponent(StreamList);
    component = fixture.componentInstance;

    expect(component.streams).toEqual(streams);
    expect(component.isLoading).toBe(false);
    expect(component.errorMessage).toBe('');
  });

  it('should show an empty list when the service returns no streams', () => {
    streamsResponse = [];

    fixture = TestBed.createComponent(StreamList);

    expect(fixture.componentInstance.streams).toEqual([]);
    expect(fixture.componentInstance.isLoading).toBe(false);
  });
});
