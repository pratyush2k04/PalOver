export type EvidenceSnapshotItemStatus = 'Open' | 'Complete' | 'NotApplicable';

export interface EvidenceSnapshotItem {
  title: string;
  categoryName: string;
  statusAtSignOff: EvidenceSnapshotItemStatus;
  evidenceNote: string | null;
  evidenceLink: string | null;
  evidenceSubmittedAt: string | null;
  evidenceSubmittedByName: string | null;
}

export interface EvidenceSnapshot {
  signedOffAt: string;
  signedOffByUserName: string;
  billingStartDate: string;
  items: EvidenceSnapshotItem[];
}
