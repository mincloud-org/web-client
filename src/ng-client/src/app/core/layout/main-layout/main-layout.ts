import { Component, inject, viewChild } from '@angular/core';
import { MatSidenav, MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { LayoutService } from '../../services/layout-service';

@Component({
  selector: 'app-main-layout',
  imports: [
    MatSidenavModule,
    MatToolbarModule,
    MatListModule,
    MatButtonModule,
    MatIconModule,
    RouterOutlet,
    RouterLink,
    RouterLinkActive
  ],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.scss',
  standalone: true
})
export class MainLayout {
  layoutService = inject(LayoutService);
  sidenav = viewChild<MatSidenav>('sidenav');

  toggleTheme() {
    this.layoutService.toggleTheme();
  }
}
