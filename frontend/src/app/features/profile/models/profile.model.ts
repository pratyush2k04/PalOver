export interface ProfileResponse {
  fullName: string;
  email: string;
  role: string;
  lastLoginAt: string | null;
}

export interface UpdateProfileRequest {
  fullName: string;
  email: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

export type MeResponse = ProfileResponse;
export type UpdateMeRequest = UpdateProfileRequest;
