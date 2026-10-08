export type StreamStatusValue = 'Preparing' | 'Live' | 'Ended';

export interface StreamStatus {
  streamId: string;
  status: StreamStatusValue;
}
