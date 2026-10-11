import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { vi } from 'vitest';

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
      category: 'Gaming',
      thumbnailUrl: null,
      viewerCount: 25,
    },
  ];

  let component: StreamList;
  let fixture: ComponentFixture<StreamList>;
  let streamsResponse = {
    items: streams,
    totalCount: 13,
    currentPage: 1,
    pageSize: 12,
    totalPages: 2,
  };
  const getStreams = vi.fn(() => of(streamsResponse));

  beforeEach(async () => {
    getStreams.mockClear();
    streamsResponse = {
      items: streams,
      totalCount: 13,
      currentPage: 1,
      pageSize: 12,
      totalPages: 2,
    };

    await TestBed.configureTestingModule({
      imports: [StreamList],
      providers: [
        provideRouter([]),
        {
          provide: StreamService,
          useValue: {
            getStreams,
          },
        },
      ],
    }).compileComponents();
  });

  it('should load streams from the service', () => {
    fixture = TestBed.createComponent(StreamList);
    component = fixture.componentInstance;

    expect(component.streams).toEqual(streams);
    expect(component.totalCount).toBe(13);
    expect(component.totalPages).toBe(2);
    expect(component.isLoading).toBe(false);
    expect(component.errorMessage).toBe('');
  });

  it('should show an empty list when the service returns no streams', () => {
    streamsResponse = {
      items: [],
      totalCount: 0,
      currentPage: 1,
      pageSize: 12,
      totalPages: 0,
    };

    fixture = TestBed.createComponent(StreamList);

    expect(fixture.componentInstance.streams).toEqual([]);
    expect(fixture.componentInstance.isLoading).toBe(false);
  });

  it('should request the next page', () => {
    fixture = TestBed.createComponent(StreamList);
    component = fixture.componentInstance;
    getStreams.mockClear();

    component.goToPage(2);

    expect(getStreams).toHaveBeenCalledWith(expect.objectContaining({
      page: 2,
      pageSize: 12,
    }));
  });

  it('should reset to the first page when filters change', () => {
    fixture = TestBed.createComponent(StreamList);
    component = fixture.componentInstance;
    component.currentPage = 2;
    component.search = 'alex';
    component.category = 'Gaming';
    component.sortBy = 'viewerCount';
    component.sortDirection = 'asc';
    getStreams.mockClear();

    component.applyFilters();

    expect(getStreams).toHaveBeenCalledWith({
      page: 1,
      pageSize: 12,
      category: 'Gaming',
      search: 'alex',
      sortBy: 'viewerCount',
      sortDirection: 'asc',
    });
  });
});
