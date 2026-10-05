import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { environment } from '../../../environments/environment';
import { StreamService } from './stream';

describe('StreamService', () => {
  let service: StreamService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(StreamService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should request streams from the API', () => {
    const expectedStreams = [
      {
        id: 'stream-1',
        userId: 'user-1',
        streamerUsername: 'Alex',
        title: 'Gaming Live',
        description: 'Gaming stream',
        startTime: '2026-10-05T18:00:00Z',
      },
    ];

    service.getStreams().subscribe((streams) => {
      expect(streams).toEqual(expectedStreams);
    });

    const request = httpTesting.expectOne(`${environment.apiUrl}/Streams`);

    expect(request.request.method).toBe('GET');
    request.flush(expectedStreams);
  });

  it('should request a stream by id from the API', () => {
    const expectedStream = {
      id: 'stream-1',
      userId: 'user-1',
      streamerUsername: 'Alex',
      title: 'Gaming Live',
      description: 'Gaming stream',
      startTime: '2026-10-05T18:00:00Z',
    };

    service.getStreamById('stream-1').subscribe((stream) => {
      expect(stream).toEqual(expectedStream);
    });

    const request = httpTesting.expectOne(
      `${environment.apiUrl}/Streams/stream-1`,
    );

    expect(request.request.method).toBe('GET');
    request.flush(expectedStream);
  });
});
