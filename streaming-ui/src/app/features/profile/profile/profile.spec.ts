import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';

import { StreamService } from '../../../core/services/stream';
import { Profile } from './profile';
import { vi } from 'vitest';

describe('Profile', () => {
  let component: Profile;
  let fixture: ComponentFixture<Profile>;

  const streamServiceMock = {
    getCurrentUserStreams: vi.fn(),
  };

  beforeEach(async () => {
    vi.clearAllMocks();
    streamServiceMock.getCurrentUserStreams
      .mockReturnValue(of([]));

    await TestBed.configureTestingModule({
      imports: [Profile],
      providers: [
        {
          provide: StreamService,
          useValue: streamServiceMock,
        },
      ],
    }).compileComponents();
  });

  it('should create', () => {
    fixture = TestBed.createComponent(Profile);
    component = fixture.componentInstance;

    expect(component).toBeTruthy();
  });

  it('should load current user streams', () => {
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

    streamServiceMock.getCurrentUserStreams
      .mockReturnValue(of(streams));

    fixture = TestBed.createComponent(Profile);
    component = fixture.componentInstance;

    expect(component.streams).toEqual(streams);
    expect(component.isLoading).toBe(false);
  });

  it('should show an error when user streams cannot be loaded', () =>{
    const error = new Error('Network error');

    streamServiceMock.getCurrentUserStreams
      .mockReturnValue(throwError(() => error));

    const consoleErrorSpy = vi
      .spyOn(console, 'error')
      .mockImplementation(() => {});

    fixture = TestBed.createComponent(Profile);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(component.streams).toEqual([]);
    expect(component.errorMessage)
      .toBe('Failed to load your streams');
    expect(component.isLoading).toBe(false);
    expect(fixture.nativeElement.textContent)
      .toContain('Failed to load your streams');

    consoleErrorSpy.mockRestore();
  });

  it('should show an empty state when the user has no streams', () => {
    streamServiceMock.getCurrentUserStreams
      .mockReturnValue(of([]));

    fixture = TestBed.createComponent(Profile);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(component.streams).toEqual([]);
    expect(component.isLoading).toBe(false);
    expect(component.errorMessage).toBe('');
    expect(fixture.nativeElement.textContent)
      .toContain('You have no streams yet.');
  });
});
