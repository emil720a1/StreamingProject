import { ComponentFixture, TestBed } from '@angular/core/testing';
import { StreamService } from '../../../core/services/stream';
import { CreatorStudio } from './creator-studio';
import { of, throwError } from 'rxjs';

describe('CreatorStudio', () => {
  let component: CreatorStudio;
  let fixture: ComponentFixture<CreatorStudio>;

  const streamServiceMock = {
    createStream: vi.fn(),
    getHlsUrl: vi.fn(),
    getStreamStatus: vi.fn(),
    endStream: vi.fn(),
  };

  beforeEach(async () => {
    streamServiceMock.createStream.mockReset();
    streamServiceMock.getHlsUrl.mockReset();
    streamServiceMock.getStreamStatus.mockReset();
    streamServiceMock.endStream.mockReset();
    streamServiceMock.getHlsUrl.mockReturnValue(
      of('http://localhost:5228/hls/stream-1/index.m3u8'),
    );
    streamServiceMock.getStreamStatus.mockReturnValue(
      of({
        streamId: 'stream-1',
        status: 'Preparing',
      }),
    );

    await TestBed.configureTestingModule({
      imports: [CreatorStudio],
      providers: [
        {
          provide: StreamService,
          useValue: streamServiceMock,
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CreatorStudio);
    component = fixture.componentInstance;

    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should not create a stream when the form is invalid', () => {
    component.submit();

    expect(streamServiceMock.createStream).not.toHaveBeenCalled();
    expect(component.streamForm.controls.title.touched).toBe(true);
    expect(component.streamForm.controls.description.touched).toBe(true);
    expect(component.streamForm.controls.category.touched).toBe(true);
  });

  it('should create a stream when the form is valid', () => {
    const request = {
      title: 'Gaming Live',
      description: 'My gaming stream',
      category: 'Gaming',
      thumbnailUrl: '',
    };

    const createdStream = {
      id: 'stream-1',
      streamKey: 'stream-key-1',
    };

    streamServiceMock.createStream.mockReturnValue(of(createdStream));

    component.streamForm.setValue(request);
    component.submit();

    expect(streamServiceMock.createStream).toHaveBeenCalledWith(request);
    expect(component.createdStream).toEqual(createdStream);
    expect(component.isSubmitting).toBe(false);
  });

  it('should display the stream key after successful creation', () => {
    const request = {
      title: 'Gaming Live',
      description: 'My gaming stream',
      category: 'Gaming',
      thumbnailUrl: '',
    };

    const createdStream = {
      id: 'stream-1',
      streamKey: 'stream-key-1',
    };

    streamServiceMock.createStream.mockReturnValue(of(createdStream));

    component.streamForm.setValue(request);
    component.submit();
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('stream-key-1');
  });

  it('should load the HLS URL after successful creation', () => {
    const request = {
      title: 'Gaming Live',
      description: 'My gaming stream',
      category: 'Gaming',
      thumbnailUrl: '',
    };
    const createdStream = {
      id: 'stream-1',
      streamKey: 'stream-key-1',
    };

    streamServiceMock.createStream.mockReturnValue(of(createdStream));
    component.streamForm.setValue(request);
    component.submit();

    expect(streamServiceMock.getHlsUrl).toHaveBeenCalledWith('stream-1');
    expect(component.hlsUrl).toBe(
      'http://localhost:5228/hls/stream-1/index.m3u8',
    );
    expect(component.isLoadingHls).toBe(false);
  });

  it('should show an error when loading the HLS URL fails', () => {
    const request = {
      title: 'Gaming Live',
      description: 'My gaming stream',
      category: 'Gaming',
      thumbnailUrl: '',
    };
    const createdStream = {
      id: 'stream-1',
      streamKey: 'stream-key-1',
    };

    streamServiceMock.createStream.mockReturnValue(of(createdStream));
    streamServiceMock.getHlsUrl.mockReturnValue(
      throwError(() => new Error('Network error')),
    );
    component.streamForm.setValue(request);
    component.submit();

    expect(component.hlsUrl).toBeNull();
    expect(component.isLoadingHls).toBe(false);
    expect(component.hlsError).toBe('Failed to load HLS URL');
  });

  it('should load stream status after successful creation', () => {
    const request = {
      title: 'Gaming Live',
      description: 'My gaming stream',
      category: 'Gaming',
      thumbnailUrl: '',
    };
    const createdStream = {
      id: 'stream-1',
      streamKey: 'stream-key-1',
    };

    streamServiceMock.createStream.mockReturnValue(of(createdStream));
    component.streamForm.setValue(request);
    component.submit();

    expect(streamServiceMock.getStreamStatus).toHaveBeenCalledWith('stream-1');
    expect(component.status).toBe('Preparing');
    expect(component.isLoadingStatus).toBe(false);
  });

  it('should show an error when loading stream status fails', () => {
    const request = {
      title: 'Gaming Live',
      description: 'My gaming stream',
      category: 'Gaming',
      thumbnailUrl: '',
    };

    streamServiceMock.createStream.mockReturnValue(
      of({
        id: 'stream-1',
        streamKey: 'stream-key-1',
      }),
    );
    streamServiceMock.getStreamStatus.mockReturnValue(
      throwError(() => new Error('Network error')),
    );

    component.streamForm.setValue(request);
    component.submit();

    expect(component.status).toBeNull();
    expect(component.isLoadingStatus).toBe(false);
    expect(component.statusError).toBe('Failed to load stream status');
  });

  it('should copy the stream key', async () => {
    const writeText = vi.fn().mockResolvedValue(undefined);
    Object.defineProperty(navigator, 'clipboard', {
      configurable: true,
      value: { writeText },
    });

    component.createdStream = {
      id: 'stream-1',
      streamKey: 'stream-key-1',
    };

    component.copyStreamKey();
    await Promise.resolve();
    await Promise.resolve();

    expect(writeText).toHaveBeenCalledWith('stream-key-1');
    expect(component.copyMessage).toBe('Stream key copied');
  });

  it('should show an error when copying the stream key fails', async () => {
    const writeText = vi.fn().mockRejectedValue(new Error('Clipboard error'));
    Object.defineProperty(navigator, 'clipboard', {
      configurable: true,
      value: { writeText },
    });

    component.createdStream = {
      id: 'stream-1',
      streamKey: 'stream-key-1',
    };

    component.copyStreamKey();
    await Promise.resolve();
    await Promise.resolve();

    expect(writeText).toHaveBeenCalledWith('stream-key-1');
    expect(component.copyMessage).toBe('Failed to copy stream key');
  });

  it('should show an error when stream creation fails', () => {
    const request = {
      title: 'Gaming Live',
      description: 'My gaming stream',
      category: 'Gaming',
      thumbnailUrl: '',
    };

    streamServiceMock.createStream.mockReturnValue(
      throwError(() => new Error('Network error')),
    );

    component.streamForm.setValue(request);
    component.submit();

    expect(streamServiceMock.createStream).toHaveBeenCalledWith(request);
    expect(component.errorMessage).toBe('Failed to create stream');
    expect(component.isSubmitting).toBe(false);
    expect(component.createdStream).toBeNull();
  });

  it('should not end the stream when confirmation is cancelled', () => {
    vi.spyOn(window, 'confirm').mockReturnValue(false);
    component.createdStream = {
      id: 'stream-1',
      streamKey: 'stream-key-1',
    };

    component.endCreatedStream();

    expect(streamServiceMock.endStream).not.toHaveBeenCalled();
    expect(component.isEnding).toBe(false);
  });

  it('should end the stream after confirmation', () => {
    vi.spyOn(window, 'confirm').mockReturnValue(true);
    streamServiceMock.endStream.mockReturnValue(of(true));
    component.createdStream = {
      id: 'stream-1',
      streamKey: 'stream-key-1',
    };

    component.endCreatedStream();

    expect(streamServiceMock.endStream).toHaveBeenCalledWith('stream-1');
    expect(component.isEnding).toBe(false);
    expect(component.endMessage).toBe('Stream ended successfully');
  });

  it('should show an error when ending the stream fails', () => {
    vi.spyOn(window, 'confirm').mockReturnValue(true);
    streamServiceMock.endStream.mockReturnValue(
      throwError(() => new Error('Network error')),
    );
    component.createdStream = {
      id: 'stream-1',
      streamKey: 'stream-key-1',
    };

    component.endCreatedStream();

    expect(streamServiceMock.endStream).toHaveBeenCalledWith('stream-1');
    expect(component.isEnding).toBe(false);
    expect(component.endMessage).toBe('Failed to end stream');
  });
});
