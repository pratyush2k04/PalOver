export interface HomeUser {
  id: number;
  fullName: string;
  role: string;
  isAdmin: boolean;
}

export interface WorkspaceMetadata {
  route: string;
  title: string;
  welcomeMessage: string;
  emptyState: string;
}

export interface HomeResponse {
  user: HomeUser;
  home: WorkspaceMetadata;
}

export interface AppContext {
  user: HomeUser;
  workspace: WorkspaceMetadata;
}
