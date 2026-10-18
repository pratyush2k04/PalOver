export type EvidenceSnapshotItemStatus = 'Open' | 'Complete' | 'NotApplicable';

export interface EvidenceSnapshotItem {
  title: string;
  categoryName: string;
  statusAtSignOff: EvidenceSnapshotItemStatus;
  evidenceDocumentId: number | null;
  evidenceDocumentFileName: string | null;
  evidenceSubmittedAt: string | null;
  evidenceSubmittedByName: string | null;
  ownerId: number | null;
  ownerName: string | null;
}

export interface EvidenceSnapshot {
  signedOffAt: string;
  signedOffByUserName: string;
  billingStartDate: string;
  items: EvidenceSnapshotItem[];
}
