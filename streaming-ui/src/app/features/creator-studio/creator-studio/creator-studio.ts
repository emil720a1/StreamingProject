import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { StreamService } from '../../../core/services/stream';
import { StreamCreated } from '../../../shared/models/stream-created';
import { StreamStatusValue } from '../../../shared/models/stream-status';

@Component({
  selector: 'app-creator-studio',
  standalone: true,
  imports: [ReactiveFormsModule],
  styleUrl: './creator-studio.scss',
  templateUrl: './creator-studio.html',
})
export class CreatorStudio {
  private readonly formBuilder = inject(FormBuilder);
  private readonly streamService = inject(StreamService);

  readonly streamForm = this.formBuilder.nonNullable.group({
    title: ['', [Validators.required, Validators.minLength(3)]],
    description: ['', [Validators.required]],
    category: ['', [Validators.required]],
    thumbnailUrl: [''],
  });

  isSubmitting = false;
  errorMessage = '';
  createdStream: StreamCreated | null = null;
  copyMessage = '';
  isEnding = false;
  endMessage = '';
  hlsUrl: string | null = null;
  isLoadingHls = false;
  hlsError = '';
  status: StreamStatusValue | null = null;
  isLoadingStatus = false;
  statusError = '';

  submit(): void {
    if (this.streamForm.invalid) {
      this.streamForm.markAllAsTouched();
      return;
    }

    const request = this.streamForm.getRawValue();

    this.isSubmitting = true;
    this.errorMessage = '';

    this.streamService.createStream(request).subscribe({
      next: (stream) => {
        this.isSubmitting = false;
        this.createdStream = stream;
        this.loadStreamStatus(stream.id);
        this.loadHlsUrl(stream.id);
      },
      error: () => {
        this.isSubmitting = false;
        this.errorMessage = 'Failed to create stream';
      },
    });
  }

  copyStreamKey(): void {
    const streamKey = this.createdStream?.streamKey;

    if (!streamKey) {
      return;
    }

    navigator.clipboard
      .writeText(streamKey)
      .then(() => {
        this.copyMessage = 'Stream key copied';
      })
      .catch(() => {
        this.copyMessage = 'Failed to copy stream key';
      });
  }

  endCreatedStream(): void {
    const streamId = this.createdStream?.id;

    if (!streamId) {
      return;
    }

    const confirmed = window.confirm(
      'Are you sure you want to end this stream?',
    );

    if (!confirmed) {
      return;
    }

    this.isEnding = true;
    this.endMessage = '';

    this.streamService.endStream(streamId).subscribe({
      next: () => {
        this.isEnding = false;
        this.status = 'Ended';
        this.endMessage = 'Stream ended successfully';
      },
      error: () => {
        this.isEnding = false;
        this.endMessage = 'Failed to end stream';
      },
    });
  }

  loadHlsUrl(streamId: string): void {
    this.isLoadingHls = true;
    this.hlsError = '';

    this.streamService.getHlsUrl(streamId).subscribe({
      next: (url) => {
        this.hlsUrl = url;
        this.isLoadingHls = false;
      },
      error: () => {
        this.hlsUrl = null;
        this.isLoadingHls = false;
        this.hlsError = 'Failed to load HLS URL';
      },
    });
  }

  loadStreamStatus(streamId: string): void {
    this.isLoadingStatus = true;
    this.statusError = '';

    this.streamService.getStreamStatus(streamId).subscribe({
      next: (result) => {
        this.status = result.status;
        this.isLoadingStatus = false;
      },
      error: () => {
        this.status = null;
        this.isLoadingStatus = false;
        this.statusError = 'Failed to load stream status';
      },
    });
  }
}
