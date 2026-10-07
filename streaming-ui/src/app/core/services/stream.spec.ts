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

  it('should request current user streams from the API', () => {
    const expectedStream = [
      {
        id: 'stream-1',
        userId: 'user-1',
        streamerUsername: 'Alex',
        title: 'Gaming Live',
        description: 'Gaming stream',
        startTime: '2026-10-05T18:00:00Z',
      },
    ];

    service.getCurrentUserStreams().subscribe((streams) => {
      expect(streams).toEqual(expectedStream);
    })

    const request = httpTesting.expectOne(
      `${environment.apiUrl}/Users/streams`,
    );

    expect(request.request.method).toBe('GET');
    request.flush(expectedStream);
  });

  it('should request HLS URL from the API', () =>{
    const expectedHlsUrl = 'http://localhost:8080/streams/stream-1/index.m3u8';

    service.getHlsUrl('stream-1').subscribe((hlsUrl) => {
      expect(hlsUrl).toBe(expectedHlsUrl);
    });

    const request = httpTesting.expectOne(
      `${environment.apiUrl}/Streams/stream-1/hls`,
    );

    expect(request.request.method).toBe('GET');
    request.flush(expectedHlsUrl);
  })

  it('should create a stream through the API', () => {
    const expectedStreams = {
      id: 'stream-1',
      userId: 'user-1',
      streamerUsername: 'Alex',
      title: 'Gaming Live',
      description: 'Gaming stream',
      startTime: '2026-10-05T18:00:00Z',
    };

    service.createStream().subscribe((stream) => {
      expect(stream).toEqual(expectedStreams);
    });

    const request = httpTesting.expectOne(
      `${environment.apiUrl}/Streams/create`,
    );

    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({});
    request.flush(expectedStreams);
  });

  it('should end a stream through the API', () => {
    service.endStream('stream-1').subscribe((result) => {
      expect(result).toBe(true);
    });

    const request = httpTesting.expectOne(
      `${environment.apiUrl}/Streams/stream-1/end`,
    );

    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({});
    request.flush(true);
  });

  it('should join a stream through the API', () => {
    service.joinStream('stream-1').subscribe((stream) => {
      expect(stream.id).toBe('stream-1');
    });

    const request = httpTesting.expectOne(
      `${environment.apiUrl}/Streams/join`,
    );

    expect(request.request.method).toBe('POST');
    expect(request.request.body).toBe('stream-1');

    request.flush({
      id: 'stream-1',
      userId: 'user-1',
      streamerUsername: 'Alex',
      title: 'Gaming Live',
      description: 'Gaming stream',
      startTime: '2026-10-05T18:00:00Z',
    });
  });
});
