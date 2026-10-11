export type StreamSortField = 'startTime' | 'viewerCount';
export type SortDirection = 'asc' | 'desc';

export interface GetStreamsParams {
  page?: number;
  pageSize?: number;
  category?: string;
  search?: string;
  sortBy?: StreamSortField;
  sortDirection?: SortDirection;
}
