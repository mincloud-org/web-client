import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SpaceService } from './space.service';

@Component({
  selector: 'app-space',
  standalone: true,
  imports: [RouterOutlet],
  providers: [
    SpaceService
  ],
  templateUrl: './space.html',
  styleUrl: './space.scss',
})
export class Space {

}
