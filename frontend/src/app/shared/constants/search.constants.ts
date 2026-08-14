export const SEARCH_API_ENDPOINTS = {
  GLOBAL: (query: string) => `/api/search?q=${encodeURIComponent(query)}`
} as const;

export const SEARCH_CONFIG = {
  MIN_QUERY_LENGTH: 2,
  DEBOUNCE_MS: 300
} as const;

export const SEARCH_MESSAGES = {
  MIN_LENGTH_HINT: 'Type at least 2 characters to search',
  SEARCH_FAILED: 'Something went wrong. Try again.',
  NO_RESULTS: (query: string) => `No results for '${query}'.`
} as const;
