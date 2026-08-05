export interface EngagementChecklistItem {
  id: number;
  engagementChecklistId: number;
  sourceMasterItemId: number | null;
  title: string;
  description: string | null;
  categoryId: number;
  categoryName: string;
  isMandatory: boolean;
  displayOrder: number;
  isApplicable: boolean;
  status: string;
}

export interface EngagementChecklistCategoryGroup {
  categoryId: number;
  categoryName: string;
  items: EngagementChecklistItem[];
}

export interface EngagementChecklist {
  id: number;
  engagementId: number;
  sourceMasterVersionId: number;
  versionNumber: number;
  categories: EngagementChecklistCategoryGroup[];
}
