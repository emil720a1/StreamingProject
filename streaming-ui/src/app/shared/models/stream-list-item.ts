export interface StreamListItem {
  id: string;
  userId: string;
  streamerUsername: string | null;
  title: string;
  description: string;
  startTime: string | null;
}
