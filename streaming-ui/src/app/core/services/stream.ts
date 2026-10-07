import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { StreamDetails } from '../../shared/models/stream-details';
import { StreamCreated } from '../../shared/models/stream-created';
import { StreamListItem } from '../../shared/models/stream-list-item';

import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class StreamService {
  private readonly apiUrl = `${environment.apiUrl}/Streams`;

  constructor(private readonly http: HttpClient) {}

  getStreams(): Observable<StreamListItem[]> {
    return this.http.get<StreamListItem[]>(this.apiUrl);
  }

  getStreamById(id: string): Observable<StreamDetails>{
    return this.http.get<StreamDetails>(`${this.apiUrl}/${id}`)
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

  createStream(): Observable<StreamCreated>{
    return this.http.post<StreamCreated>(
      `${this.apiUrl}/create`,
      {},
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
