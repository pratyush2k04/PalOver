export type SearchType = 'engagement' | 'document' | 'person' | 'checklist' | 'decision';

export interface SearchResultGroup<T> {
  count: number;
  items: T[];
}

export interface SearchEngagementResult {
  engagementId: number;
  engagementName: string;
  clientName: string;
}

export interface SearchDocumentResult {
  documentId: number;
  fileName: string;
  engagementId: number;
  engagementName: string;
  categoryName: string;
}

export interface SearchPersonResult {
  personId: number;
  name: string;
  role: string;
  engagementId: number;
  engagementName: string;
}

export interface SearchChecklistItemResult {
  itemId: number;
  title: string;
  status: string;
  engagementId: number;
  engagementName: string;
  categoryName: string;
}

export interface SearchDecisionResult {
  decisionId: number;
  title: string;
  status: string;
  engagementId: number;
  engagementName: string;
}

export interface GlobalSearchResponse {
  query: string;
  totalCount: number;
  engagements: SearchResultGroup<SearchEngagementResult>;
  documents: SearchResultGroup<SearchDocumentResult>;
  people: SearchResultGroup<SearchPersonResult>;
  checklistItems: SearchResultGroup<SearchChecklistItemResult>;
  decisions: SearchResultGroup<SearchDecisionResult>;
}
