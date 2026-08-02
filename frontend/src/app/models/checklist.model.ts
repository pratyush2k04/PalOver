export interface ChecklistItem {
  id: number;
  versionId: number;
  title: string;
  description: string | null;
  categoryId: number;
  categoryName: string;
  isMandatory: boolean;
  displayOrder: number;
  isRetired: boolean;
}

export interface ChecklistVersion {
  id: number;
  versionNumber: number;
  statusId: number;
  statusName: string;
  items: ChecklistItem[];
}

export interface ChecklistVersionSummary {
  id: number;
  versionNumber: number;
  statusId: number;
  statusName: string;
  createdAt: string;
  updatedAt: string;
}

export interface ChecklistCategory {
  id: number;
  categoryName: string;
}

export interface CreateChecklistItemRequest {
  title: string;
  description: string | null;
  categoryId: number;
  isMandatory: boolean;
  displayOrder: number;
}

export interface UpdateChecklistItemRequest {
  title: string;
  description: string | null;
  categoryId: number;
  isMandatory: boolean;
}

export interface ChecklistItemOrder {
  itemId: number;
  displayOrder: number;
}

export interface ReorderChecklistItemsRequest {
  items: ChecklistItemOrder[];
}
