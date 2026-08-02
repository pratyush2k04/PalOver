import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserDirectoryService } from '../../services/user-directory';
import { ToastService } from '../../services/toast';
import { UserListItem } from '../../models/user.model';
import { Role } from '../../models/role.model';
import { ConfirmDialogComponent } from '../../shared/confirm-dialog/confirm-dialog';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner/loading-spinner';

interface PendingRoleChange {
  user: UserListItem;
  newRoleId: number;
  newRoleName: string;
}

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, FormsModule, ConfirmDialogComponent, LoadingSpinnerComponent],
  templateUrl: './users.html',
  styleUrl: './users.scss'
})
export class UsersComponent implements OnInit {
  private userDirectoryService = inject(UserDirectoryService);
  private toastService = inject(ToastService);

  users = signal<UserListItem[]>([]);
  roles = signal<Role[]>([]);
  searchTerm = '';
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);

  pendingChange = signal<PendingRoleChange | null>(null);
  isSavingRoleChange = signal(false);

  private searchDebounce: ReturnType<typeof setTimeout> | null = null;

  ngOnInit(): void {
    this.loadRoles();
    this.loadUsers();
  }

  onSearchChange(): void {
    if (this.searchDebounce) {
      clearTimeout(this.searchDebounce);
    }
    this.searchDebounce = setTimeout(() => this.loadUsers(), 300);
  }

  loadUsers(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.userDirectoryService.getUsers(this.searchTerm).subscribe({
      next: (result) => {
        this.users.set(result.items);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to load users. Please try again.');
        this.isLoading.set(false);
      }
    });
  }

  loadRoles(): void {
    this.userDirectoryService.getRoles().subscribe({
      next: (roles) => this.roles.set(roles),
      error: () => {
        this.errorMessage.set('Unable to load roles. Please try again.');
      }
    });
  }

  initials(fullName: string): string {
    return fullName
      .split(' ')
      .filter(Boolean)
      .slice(0, 2)
      .map((part) => part[0]?.toUpperCase())
      .join('');
  }

  onRoleSelected(user: UserListItem, newRoleIdRaw: string): void {
    const newRoleId = Number(newRoleIdRaw);
    if (newRoleId === user.roleId) {
      return;
    }

    const newRole = this.roles().find((r) => r.id === newRoleId);
    if (!newRole) {
      return;
    }

    this.pendingChange.set({ user, newRoleId, newRoleName: newRole.name });
  }

  confirmRoleChange(): void {
    const pending = this.pendingChange();
    if (!pending) {
      return;
    }

    this.isSavingRoleChange.set(true);

    this.userDirectoryService.changeUserRole(pending.user.id, pending.newRoleId).subscribe({
      next: (updatedUser) => {
        this.users.update((list) =>
          list.map((u) => (u.id === updatedUser.id ? updatedUser : u))
        );
        this.isSavingRoleChange.set(false);
        this.pendingChange.set(null);
        this.toastService.success('Role updated');
      },
      error: () => {
        this.errorMessage.set('Unable to update the role. Please try again.');
        this.isSavingRoleChange.set(false);
        this.pendingChange.set(null);
      }
    });
  }

  cancelRoleChange(): void {
    this.pendingChange.set(null);
  }
}
