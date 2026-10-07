import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Stream } from '../../shared/models/stream';

@Injectable({
  providedIn: 'root',
})
export class StreamService {
  private readonly apiUrl = `${environment.apiUrl}/Streams`;

  constructor(private readonly http: HttpClient) {}

  getStreams(): Observable<Stream[]> {
    return this.http.get<Stream[]>(this.apiUrl);
  }

  getStreamById(id: string): Observable<Stream>{
    return this.http.get<Stream>(`${this.apiUrl}/${id}`)
  }

  getCurrentUserStreams(): Observable<Stream[]> {
    return this.http.get<Stream[]>(
      `${environment.apiUrl}/Users/streams`,
    );
  }

  getHlsUrl(id: string): Observable<string>{
    return this.http.get<string>(
      `${this.apiUrl}/${id}/hls`,
    );
  }

  createStream(): Observable<Stream>{
    return this.http.post<Stream>(
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

  joinStream(id: string): Observable<Stream> {
    return this.http.post<Stream>(
      `${this.apiUrl}/join`,
      id,
    );
  }
}
