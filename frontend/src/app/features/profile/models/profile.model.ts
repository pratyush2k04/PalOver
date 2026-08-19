export interface MeResponse {
  fullName: string;
  email: string;
  role: string;
  lastLoginAt: string | null;
}

export interface UpdateMeRequest {
  fullName: string;
  email: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}
