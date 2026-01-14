import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { StorageService } from './storage.service';

@Component({
  selector: 'app-storage',
  imports: [RouterOutlet],
  templateUrl: './storage.html',
  styleUrl: './storage.scss',
  providers: [
    StorageService
  ],
})
export class Storage {

}
