import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SpaceDto } from './space.model';

@Injectable()
export class SpaceService {
  constructor(private httpClient: HttpClient) {}

  getSpaces() {
    return this.httpClient.get<SpaceDto[]>('/api/v1/spaces');
  }
}
