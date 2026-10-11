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
    const expectedResponse = {
      items: [{
        id: 'stream-1',
        userId: 'user-1',
        streamerUsername: 'Alex',
        title: 'Gaming Live',
        description: 'Gaming stream',
        startTime: '2026-10-05T18:00:00Z',
        category: 'Gaming',
        thumbnailUrl: null,
        viewerCount: 25,
      }],
      totalCount: 1,
      currentPage: 1,
      pageSize: 12,
      totalPages: 1,
    };

    service.getStreams({
      category: ' Gaming ',
      search: ' Alex ',
      sortBy: 'viewerCount',
      sortDirection: 'asc',
    }).subscribe((response) => {
      expect(response).toEqual(expectedResponse);
    });

    const request = httpTesting.expectOne(
      request => request.url === `${environment.apiUrl}/Streams`,
    );

    expect(request.request.method).toBe('GET');
    expect(request.request.params.get('page')).toBe('1');
    expect(request.request.params.get('pageSize')).toBe('12');
    expect(request.request.params.get('category')).toBe('Gaming');
    expect(request.request.params.get('search')).toBe('Alex');
    expect(request.request.params.get('sortBy')).toBe('viewerCount');
    expect(request.request.params.get('sortDirection')).toBe('asc');
    request.flush(expectedResponse);
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
        category: 'Gaming',
        thumbnailUrl: null,
        viewerCount: 25,
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
    const expectedStream = {
      id: 'stream-1',
      streamKey: 'stream-key-1',
    };

    const createRequest = {
      title: 'Gaming Live',
      description: 'Gaming stream',
      category: 'Gaming',
      thumbnailUrl: null,
    };

    service.createStream(createRequest).subscribe((stream) => {
      expect(stream).toEqual(expectedStream);
    });

    const request = httpTesting.expectOne(
      `${environment.apiUrl}/Streams/create`,
    );

    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(createRequest);
    request.flush(expectedStream);
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

  it('should request stream through the API', () => {
    service.getStreamStatus('stream-1').subscribe((result) => {
      expect(result.status).toBe('Preparing');
    });

    const request = httpTesting.expectOne(
      `${environment.apiUrl}/Streams/stream-1/status`,
    );

    expect(request.request.method).toBe('GET');

    request.flush({
      streamId: 'stream-1',
      status: 'Preparing',
    });
  })
});
