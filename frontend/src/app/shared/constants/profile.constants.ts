export const PROFILE_API_ENDPOINTS = {
  GET_PROFILE: '/api/profile',
  UPDATE_PROFILE: '/api/profile',
  CHANGE_PASSWORD: '/api/profile/change-password',
} as const;

export const PROFILE_MESSAGES = {
  PROFILE_UPDATED: 'Profile updated.',
  PASSWORD_CHANGED: 'Password changed.',
  LOAD_PROFILE_ERROR: 'Unable to load your profile. Please try again.',
  UPDATE_PROFILE_ERROR: 'Unable to update your profile. Please try again.',
  CHANGE_PASSWORD_ERROR: 'Unable to change your password. Please try again.',
  INCORRECT_CURRENT_PASSWORD: 'Your current password is incorrect.',
  INVALID_EMAIL: 'Enter a valid work email address.',
} as const;
