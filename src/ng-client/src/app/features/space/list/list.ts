import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { SpaceDto } from '../space.model';
import { SpaceService } from '../space.service';
import { SpaceEditor } from '../editor/space-editor';

@Component({
  selector: 'app-space-list',
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatDialogModule
  ],
  templateUrl: './list.html',
  styleUrl: './list.scss',
})
export class SpaceList implements OnInit {
  public spaces = signal<SpaceDto[]>([]);
  public displayedColumns: string[] = ['name', 'description', 'createdAt', 'updatedAt', 'actions'];
  public loading = signal(true);

  constructor(
    private spaceService: SpaceService,
    private dialog: MatDialog
  ) { }

  ngOnInit() {
    this.loadSpaces();
  }

  loadSpaces() {
    this.loading.set(true);
    this.spaceService.getList().subscribe({
      next: (result) => {
        this.spaces.set(result.items);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Error loading spaces:', error);
        this.loading.set(false);
      }
    });
  }

  createSpace() {
    const dialogRef = this.dialog.open(SpaceEditor, {
      width: '480px',
      data: { mode: 'create' }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadSpaces();
      }
    });
  }

  editSpace(space: SpaceDto) {
    const dialogRef = this.dialog.open(SpaceEditor, {
      width: '480px',
      data: { mode: 'edit', space: space }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadSpaces();
      }
    });
  }
}
