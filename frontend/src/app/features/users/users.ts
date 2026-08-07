import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from './services/user.service';
import { ToastService } from '@shared/toast/toast.service';
import { UserListItem } from './models/user.model';
import { Role } from './models/role.model';
import { ConfirmDialogComponent } from '../../shared/confirm-dialog/confirm-dialog';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner/loading-spinner';
import { USER_MESSAGES } from '../../constants/app.constants';

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
  private userService = inject(UserService);
  private toastService = inject(ToastService);

  users = signal<UserListItem[]>([]);
  roles = signal<Role[]>([]);
  searchTerm = '';
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);

  page = signal(1);
  pageSize = 5;
  totalCount = signal(0);

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
    this.searchDebounce = setTimeout(() => {
      this.page.set(1);
      this.loadUsers();
    }, 300);
  }

  loadUsers(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.userService.getUsers(this.searchTerm, this.page(), this.pageSize).subscribe({
      next: (result) => {
        this.users.set(result.items);
        this.totalCount.set(result.totalCount);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set(USER_MESSAGES.LOAD_USERS_ERROR);
        this.isLoading.set(false);
      }
    });
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.totalCount() / this.pageSize));
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages || page === this.page()) {
      return;
    }
    this.page.set(page);
    this.loadUsers();
  }

  previousPage(): void {
    this.goToPage(this.page() - 1);
  }

  nextPage(): void {
    this.goToPage(this.page() + 1);
  }

  loadRoles(): void {
    this.userService.getRoles().subscribe({
      next: (roles) => this.roles.set(roles),
      error: () => {
        this.errorMessage.set(USER_MESSAGES.LOAD_ROLES_ERROR);
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

    this.userService.changeUserRole(pending.user.id, pending.newRoleId).subscribe({
      next: (updatedUser) => {
        this.users.update((list) =>
          list.map((u) => (u.id === updatedUser.id ? updatedUser : u))
        );
        this.isSavingRoleChange.set(false);
        this.pendingChange.set(null);
        this.toastService.success(USER_MESSAGES.ROLE_UPDATED_SUCCESS);
      },
      error: () => {
        this.errorMessage.set(USER_MESSAGES.UPDATE_ROLE_ERROR);
        this.isSavingRoleChange.set(false);
        this.pendingChange.set(null);
      }
    });
  }

  cancelRoleChange(): void {
    this.pendingChange.set(null);
  }
}
