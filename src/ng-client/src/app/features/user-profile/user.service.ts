import { Injectable, signal } from '@angular/core';
import { User } from './user.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  // Use signals for state
  private currentUser = signal<User | null>(null);

  // Expose read-only signal
  readonly user = this.currentUser.asReadonly();

  loadUser(id: string): void {
    // Simulate API call
    setTimeout(() => {
      this.currentUser.set({
        id,
        username: 'demo_user',
        email: 'demo@example.com',
        avatarUrl: 'assets/avatar-placeholder.png',
        isAdmin: false
      });
    }, 500);
  }

  updateEmail(newEmail: string): void {
    this.currentUser.update(user => user ? { ...user, email: newEmail } : null);
  }
}
