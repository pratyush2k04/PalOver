import { DocumentCategoryId } from '../../models/document.model';

export const DOCUMENT_MESSAGES = {
  // Engagement Documents List
  LOAD_FAILED: 'Failed to load documents.',
  DOWNLOAD_FAILED: (fileName: string) => `Failed to download ${fileName}`,
  UPDATE_VISIBILITY_FAILED: 'Failed to update visibility.',

  // Custom Visibility Dialog
  CUSTOM_VISIBILITY_NO_ROLE_SELECTED: 'Please select at least one role to restrict access.',
  CUSTOM_VISIBILITY_UPDATE_FAILED: 'Failed to update custom visibility.',
  CUSTOM_VISIBILITY_REMOVE_FAILED: 'Failed to remove visibility override.',

  // Upload Document Dialog
  UPLOAD_NO_FILES_SELECTED: 'Please select at least one file to upload.',
  UPLOAD_NO_CATEGORY_SELECTED: 'Please select a category.',
  UPLOAD_UNSUPPORTED_FILE_TYPE: (fileName: string) => `"${fileName}" is not a supported file type.`,
  UPLOAD_FILE_EXCEEDS_SIZE_LIMIT: (fileName: string) => `"${fileName}" exceeds the 50 MB size limit.`,
  UPLOAD_NETWORK_OR_SERVER_LIMIT_ERROR: 'File exceeds server upload size limit or network connection failed.',
  UPLOAD_FAILED: 'Failed to upload document.',

  // Remove Document Dialog
  REMOVE_FAILED: 'Failed to remove document.'
} as const;

export const DOCUMENT_CONFIG = {
  ALLOWED_EXTENSIONS: ['.pdf', '.docx', '.xlsx', '.pptx', '.png', '.jpg', '.jpeg', '.msg', '.eml'] as readonly string[],
  MAX_FILE_SIZE_BYTES: 50 * 1024 * 1024, // 50 MB
  DEFAULT_CATEGORIES: [
    { id: DocumentCategoryId.Rfp, categoryName: 'RFP' },
    { id: DocumentCategoryId.Proposal, categoryName: 'Proposal' },
    { id: DocumentCategoryId.SignedSow, categoryName: 'Signed SOW' },
    { id: DocumentCategoryId.EmailThread, categoryName: 'Email Thread' },
    { id: DocumentCategoryId.DesignOrApproach, categoryName: 'Design or Approach' },
    { id: DocumentCategoryId.Commercial, categoryName: 'Commercial' },
    { id: DocumentCategoryId.Other, categoryName: 'Other' }
  ],
  VISIBILITY: {
    TEAM_WIDE: 'Team-wide',
    PM_ONLY: 'PM-only',
    CUSTOM: 'Custom'
  },
  PM_ONLY_CATEGORY_IDS: [
    DocumentCategoryId.Proposal,
    DocumentCategoryId.Commercial
  ] as readonly number[],
  ROLE_IDS: {
    PRE_SALES: 1,
    PROJECT_MANAGEMENT: 2,
    TECH_LEAD: 3,
    LEADERSHIP: 4
  },
  AVAILABLE_CUSTOM_VISIBILITY_ROLES: [
    { id: 4, name: 'Leadership', label: 'Leadership' },
    { id: 3, name: 'Tech Lead', label: 'Tech Lead' },
    { id: 1, name: 'Pre Sales', label: 'Pre Sales' }
  ]
} as const;
