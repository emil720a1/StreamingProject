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
}
