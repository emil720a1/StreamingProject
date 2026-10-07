import { StreamListItem } from './stream-list-item';

export interface StreamDetails extends StreamListItem {
  endTime: string | null;
}
