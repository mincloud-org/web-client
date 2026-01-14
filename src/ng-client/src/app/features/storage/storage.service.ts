import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CreateStorageRequest, StorageDto } from './storage.model';
import { IPagination } from '../../core/models/pagination.model';
import { SelectListItem } from '../../core/models/common.model';

@Injectable()
export class StorageService {
  constructor(private httpClient: HttpClient) { }

  getById(id: string) {
    return this.httpClient.get<StorageDto>(`/api/v1/storages/${id}`);
  }

  getList(request = {
    offset: 0,
    limit: 20,
    search: ''
  }) {
    return this.httpClient.get<IPagination<StorageDto>>('/api/v1/storages', {
      params: {
        offset: request.offset.toString(),
        limit: request.limit.toString(),
        search: request.search
      }
    })
  }

  create(request: CreateStorageRequest) {
    return this.httpClient.post<StorageDto>('/api/v1/storages', request);
  }

  update(id: string, request: CreateStorageRequest) {
    return this.httpClient.put<StorageDto>(`/api/v1/storages/${id}`, request);
  }

  listItems(search: string = '') {
    return this.httpClient.get<SelectListItem[]>('/api/v1/storages/list-items', {
      params: {
        search: search
      }
    });
  }
}
