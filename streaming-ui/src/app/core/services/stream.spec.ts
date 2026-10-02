import { TestBed } from '@angular/core/testing';
import { StreamService } from './stream';

describe('StreamService', () => {
  let service: StreamService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StreamService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should return streams', () => {
    const streams = service.getStreams();

    expect(streams.length).toBe(3);
  });

  it('should find stream by id', () => {
    const stream = service.getStreamById(1);

    expect(stream?.title).toBe('Gaming Live');
  });

  it('should return undefined for unknown id', () => {
    const stream = service.getStreamById(999);

    expect(stream).toBeUndefined();
  });
});
