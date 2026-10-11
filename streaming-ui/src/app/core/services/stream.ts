import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { StreamDetails } from '../../shared/models/stream-details';
import { StreamCreated } from '../../shared/models/stream-created';
import { StreamListItem } from '../../shared/models/stream-list-item';
import { StreamStatus } from '../../shared/models/stream-status';
import { GetStreamsParams } from '../../shared/models/get-streams-params';
import { PaginatedResponse } from '../../shared/models/paginated-response';

import { environment } from '../../../environments/environment';
import { CreateStreamRequest } from '../../shared/models/create-stream-request';

@Injectable({
  providedIn: 'root',
})
export class StreamService {
  private readonly apiUrl = `${environment.apiUrl}/Streams`;

  constructor(private readonly http: HttpClient) {}

  getStreams(
    parameters: GetStreamsParams = {},
  ): Observable<PaginatedResponse<StreamListItem>> {
    let params = new HttpParams()
      .set('page', parameters.page ?? 1)
      .set('pageSize', parameters.pageSize ?? 12)
      .set('sortBy', parameters.sortBy ?? 'startTime')
      .set('sortDirection', parameters.sortDirection ?? 'desc');

    if (parameters.category?.trim()) {
      params = params.set('category', parameters.category.trim());
    }

    if (parameters.search?.trim()) {
      params = params.set('search', parameters.search.trim());
    }

    return this.http.get<PaginatedResponse<StreamListItem>>(
      this.apiUrl,
      { params },
    );
  }

  getStreamById(id: string): Observable<StreamDetails>{
    return this.http.get<StreamDetails>(`${this.apiUrl}/${id}`)
  }

  getStreamStatus(id: string): Observable<StreamStatus> {
    return this.http.get<StreamStatus>(
      `${this.apiUrl}/${id}/status`,
    );
  }

  getCurrentUserStreams(): Observable<StreamListItem[]> {
    return this.http.get<StreamListItem[]>(
      `${environment.apiUrl}/Users/streams`,
    );
  }

  getHlsUrl(id: string): Observable<string>{
    return this.http.get<string>(
      `${this.apiUrl}/${id}/hls`,
    );
  }

  createStream(
    request: CreateStreamRequest,
  ): Observable<StreamCreated>{
    return this.http.post<StreamCreated>(
      `${this.apiUrl}/create`,
      request,
    );
  }

  endStream(id: string): Observable<boolean> {
    return this.http.post<boolean>(
      `${this.apiUrl}/${id}/end`,
      {},
    );
  }

  joinStream(id: string): Observable<StreamDetails> {
    return this.http.post<StreamDetails>(
      `${this.apiUrl}/join`,
      id,
    );
  }
}
