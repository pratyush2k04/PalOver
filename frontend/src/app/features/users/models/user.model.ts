export interface UserListItem {
  id: number;
  fullName: string;
  email: string;
  roleId: number;
  roleName: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}
