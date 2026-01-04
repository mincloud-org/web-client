import { Component, OnInit } from '@angular/core';
import { SpaceDto } from '../space.model';
import { SpaceService } from '../space.service';

@Component({
  selector: 'app-space-list',
  imports: [],
  templateUrl: './list.html',
  styleUrl: './list.scss',
})
export class SpaceList implements OnInit {

  public spaces: SpaceDto[] = [];
  constructor(
    private spaceService: SpaceService
  ) { }

  ngOnInit() {
    this.spaceService.getSpaces().subscribe((spaces) => {
      this.spaces = spaces;
    });
  }
}
