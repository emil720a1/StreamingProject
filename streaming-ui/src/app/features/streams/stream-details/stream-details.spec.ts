import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, provideRouter } from '@angular/router';
import { Observable, of, throwError } from 'rxjs';

import { StreamService } from '../../../core/services/stream';
import { StreamDetails as StreamDetailsModel } from '../../../shared/models/stream-details';
import { StreamDetails } from './stream-details';

describe('StreamDetails', () => {
  let component: StreamDetails;
  let fixture: ComponentFixture<StreamDetails>;

  const stream: StreamDetailsModel = {
    id: 'stream-1',
    userId: 'user-1',
    streamerUsername: 'Alex',
    title: 'Gaming Live',
    description: 'Gaming stream',
    startTime: '2026-10-05T18:00:00Z',
    endTime: null,
  };

  async function configureTest(
    routeId: string,
    getStreamById: () => Observable<StreamDetailsModel>,
  ): Promise<void> {
    await TestBed.configureTestingModule({
      imports: [StreamDetails],
      providers: [
        provideRouter([]),
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: {
                get: (key: string) =>
                  key === 'id' ? routeId : null,
              },
            },
          },
        },
        {
          provide: StreamService,
          useValue: { getStreamById },
        },
      ],
    }).compileComponents();
  }

  it('should create', async () => {
    await configureTest('stream-1', () => of(stream));

    fixture = TestBed.createComponent(StreamDetails);
    component = fixture.componentInstance;

    expect(component).toBeTruthy();
  });

  it('should load stream by route id', async () => {
    await configureTest('stream-1', () => of(stream));

    fixture = TestBed.createComponent(StreamDetails);
    component = fixture.componentInstance;

    expect(component.stream?.title).toBe('Gaming Live');
    expect(component.stream?.streamerUsername).toBe('Alex');
  });

  it('should show an error when the service cannot load the stream', async () => {
    await configureTest('unknown-stream', () =>
      throwError(() => new Error('Stream not found')),
    );

    fixture = TestBed.createComponent(StreamDetails);
    fixture.detectChanges();

    expect(fixture.componentInstance.errorMessage).toBe('Stream not found');
  });
});
