import { Component, ChangeDetectionStrategy, input, output, inject, computed } from '@angular/core';
import { NgOptimizedImage } from '@angular/common';
import { UserService } from './user.service';

@Component({
  selector: 'app-user-profile',
  imports: [NgOptimizedImage],
  template: `
    <div class="profile-card">
      @if (user(); as u) {
        <div class="header">
          <!-- Using NgOptimizedImage for best performance -->
          <img [ngSrc]="u.avatarUrl" width="100" height="100" alt="User Avatar" priority />
          <h2>{{ u.username }}</h2>
          
          @if (isAdmin()) {
            <span class="badge">Admin</span>
          }
        </div>

        <div class="details">
          <p>Email: {{ u.email }}</p>
          <p>Status: {{ statusLabel() }}</p>
        </div>

        <div class="actions">
          <button (click)="onEdit()">Edit Profile</button>
        </div>
      } @else {
        <p>Loading user profile...</p>
      }
    </div>
  `,
  styles: [`
    .profile-card {
      padding: 1rem;
      border: 1px solid #ccc;
      border-radius: 8px;
    }
    .badge {
      background-color: gold;
      padding: 0.2rem 0.5rem;
      border-radius: 4px;
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserProfileComponent {
  private userService = inject(UserService);

  // Signal inputs (replaces @Input)
  userId = input.required<string>();
  
  // Signal outputs (replaces @Output)
  edit = output<void>();

  // Computed state from service
  user = this.userService.user;
  
  // Computed values
  isAdmin = computed(() => this.user()?.isAdmin ?? false);
  statusLabel = computed(() => this.isAdmin() ? 'Administrator' : 'Standard User');

  constructor() {
    // Effects or initialization logic
    // Note: In a real app, you might trigger the load in an effect or route resolver
    // this.userService.loadUser(this.userId()); 
  }

  onEdit() {
    this.edit.emit();
  }
}
