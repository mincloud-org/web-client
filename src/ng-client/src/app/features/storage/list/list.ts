import { Component, signal } from '@angular/core';
import { StorageDto } from '../storage.model';
import { StorageService } from '../storage.service';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { RouterLink } from "@angular/router";

@Component({
  selector: 'app-storage-list',
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatProgressSpinnerModule,
    RouterLink
],
  templateUrl: './list.html',
  styleUrl: './list.scss',
})
export class StorageList {
  public storages = signal<StorageDto[]>([]);
  public displayedColumns: string[] = ['name', 'type', 'status', 'quotaBytes', 'createdAt', 'updatedAt', 'actions'];
  public loading = signal(true);

  constructor(
    private storageService: StorageService
  ) { }

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loading.set(true);
    this.storageService.getList().subscribe({
      next: (result) => {
        this.storages.set(result.items);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Error loading storages:', error);
        this.loading.set(false);
      }
    });
  }
}
