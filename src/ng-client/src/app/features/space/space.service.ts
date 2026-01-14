import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CreateSpaceRequest, SpaceDto, UpdateSpaceRequest } from './space.model';
import { IPagination } from '../../core/models/pagination.model';

@Injectable()
export class SpaceService {
  constructor(private httpClient: HttpClient) { }

  getList(request = {
    offset: 0,
    limit: 20,
    search: ''
  }) {

    return this.httpClient.get<IPagination<SpaceDto>>('/api/v1/spaces', {
      params: {
        offset: request.offset.toString(),
        limit: request.limit.toString(),
        search: request.search
      }
    });
  }

  create(request: CreateSpaceRequest) {
    return this.httpClient.post<SpaceDto>('/api/v1/spaces', request);
  }

  update(id: string, request: UpdateSpaceRequest) {
    return this.httpClient.put<SpaceDto>(`/api/v1/spaces/${id}`, request);
  }

  getById(id: string) {
    return this.httpClient.get<SpaceDto>(`/api/v1/spaces/${id}`);
  }
}
