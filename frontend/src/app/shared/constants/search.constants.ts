export const SEARCH_API_ENDPOINTS = {
  GLOBAL: (query: string, types?: string[]) => {
    const params = new URLSearchParams();
    params.set('q', query);
    if (types && types.length > 0) {
      for (const t of types) {
        params.append('types', t);
      }
    }
    return `/api/search?${params.toString()}`;
  }
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
