import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NEVER, Observable, of, throwError } from 'rxjs';
import { provideRouter } from '@angular/router';

import { StreamListItem } from '../../../shared/models/stream-list-item';
import { StreamService } from '../../../core/services/stream';
import { Dashboard } from './dashboard';
import { PaginatedResponse } from '../../../shared/models/paginated-response';

describe('Dashboard', () => {
  let component: Dashboard;
  let fixture: ComponentFixture<Dashboard>;
  let streamsResponse: Observable<PaginatedResponse<StreamListItem>>;

  const page = (
    items: StreamListItem[],
    totalCount = items.length,
  ): PaginatedResponse<StreamListItem> => ({
    items,
    totalCount,
    currentPage: 1,
    pageSize: 12,
    totalPages: totalCount === 0 ? 0 : Math.ceil(totalCount / 12),
  });

  beforeEach(async () => {
    streamsResponse = of(page([]));
    await TestBed.configureTestingModule({
      imports: [Dashboard],
      providers: [
        provideRouter([]),
        {
          provide: StreamService,
          useValue: {
            getStreams: () => streamsResponse,
          },
        },
      ],
    }).compileComponents();
  });

  it('should create', () => {
    fixture = TestBed.createComponent(Dashboard);
    component = fixture.componentInstance;

    expect(component).toBeTruthy();
  });

  it('should show an error when streams cannot be loaded', () => {
    streamsResponse = throwError(() => new Error('Network error'));

    fixture = TestBed.createComponent(Dashboard);

    expect(fixture.componentInstance.errorMessage)
      .toBe('Failed to load streams');

    expect(fixture.componentInstance.isLoading)
      .toBe(false);
  });

  it('should retry loading streams after an error', () => {
    streamsResponse = throwError(() => new Error('Network error'));

    fixture = TestBed.createComponent(Dashboard);
    fixture.detectChanges();

    const retryButton = fixture.nativeElement.querySelector('button');

    expect(retryButton).toBeTruthy();

    streamsResponse = of(page([]));

    retryButton.click();
    fixture.detectChanges();

    expect(fixture.componentInstance.errorMessage).toBe('');
    expect(fixture.componentInstance.isLoading).toBe(false);
    expect(fixture.componentInstance.streams).toEqual([]);
  });

  it('should load streams successfully', () => {
    const streams: StreamListItem[] = [
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

    streamsResponse = of(page(streams, 25));

    fixture = TestBed.createComponent(Dashboard);

    expect(fixture.componentInstance.streams).toEqual(streams);
    expect(fixture.componentInstance.totalCount).toBe(25);
    expect(fixture.componentInstance.isLoading).toBe(false);
    expect(fixture.componentInstance.errorMessage).toBe('');
  });

  it('should show empty state when no streams are available', () => {
    streamsResponse = of(page([]));

    fixture = TestBed.createComponent(Dashboard);
    fixture.detectChanges();

    expect(fixture.componentInstance.streams).toEqual([]);
    expect(fixture.componentInstance.isLoading).toBe(false);
    expect(fixture.nativeElement.textContent)
      .toContain('No live streams available.');
  });

  it('should show loading state while streams are loading', () => {
    streamsResponse = NEVER;

    fixture = TestBed.createComponent(Dashboard);
    fixture.detectChanges();

    expect(fixture.componentInstance.isLoading).toBe(true);
    expect(fixture.nativeElement.textContent)
      .toContain('Loading streams...');
  });

  it('should clear stale streams when reload fails', () => {
    const streams: StreamListItem[] = [
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

    streamsResponse = of(page(streams));

    fixture = TestBed.createComponent(Dashboard);

    expect(fixture.componentInstance.streams).toEqual(streams);

    streamsResponse = throwError(() => new Error('Network error'));

    fixture.componentInstance.loadStreams();

    expect(fixture.componentInstance.streams).toEqual([]);
    expect(fixture.componentInstance.errorMessage)
      .toBe('Failed to load streams');
  });
});
