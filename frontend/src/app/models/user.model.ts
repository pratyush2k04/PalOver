export interface UserListItem {
  id: number;
  fullName: string;
  email: string;
  roleId: number;
  roleName: string;
  isAdmin: boolean;
}

export interface UserListResponse {
  users: UserListItem[];
}
