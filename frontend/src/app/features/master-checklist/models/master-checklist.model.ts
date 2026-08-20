export interface MasterChecklistItem {
  id: number;
  versionId: number;
  title: string;
  description: string | null;
  categoryId: number;
  categoryName: string;
  isMandatory: boolean;
  evidenceRequired: boolean;
  displayOrder: number;
  isRetired: boolean;
}

export interface MasterChecklistVersion {
  id: number;
  versionNumber: number;
  statusId: number;
  statusName: string;
  updatedAt: string;
  items: MasterChecklistItem[];
}

export interface MasterChecklistVersionSummary {
  id: number;
  versionNumber: number;
  statusId: number;
  statusName: string;
  createdAt: string;
  updatedAt: string;
}

export interface MasterChecklistCategory {
  id: number;
  categoryName: string;
}

export interface CreateMasterChecklistItemRequest {
  title: string;
  description: string | null;
  categoryId: number;
  isMandatory: boolean;
  evidenceRequired: boolean;
  displayOrder: number;
}

export interface UpdateMasterChecklistItemRequest {
  title: string;
  description: string | null;
  categoryId: number;
  isMandatory: boolean;
  evidenceRequired: boolean;
}

export interface MasterChecklistItemOrder {
  itemId: number;
  displayOrder: number;
}

export interface ReorderMasterChecklistItemsRequest {
  items: MasterChecklistItemOrder[];
}
