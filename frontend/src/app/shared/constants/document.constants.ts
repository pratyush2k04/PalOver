import { DocumentCategoryId } from '../../features/engagements/models/document.model';
import { UserRoleId, EngagementRoleId } from './users.constants';

export const DOCUMENT_API_ENDPOINTS = {
  BY_ENGAGEMENT: (engagementId: number) => `/api/documents/engagement/${engagementId}`,
  CATEGORIES: '/api/documents/categories',
  UPLOAD: '/api/documents/upload',
  DOWNLOAD: (id: number) => `/api/documents/${id}/download`,
  REMOVE: (id: number) => `/api/documents/${id}/remove`,
  VISIBILITY: (id: number) => `/api/documents/${id}/visibility`,
  VERSIONS: (id: number) => `/api/documents/${id}/versions`,
  DOWNLOAD_VERSION: (id: number, versionId: number) => `/api/documents/${id}/versions/${versionId}/download`
} as const;

export const DOCUMENT_MESSAGES = {
  // Engagement Documents List
  LOAD_FAILED: 'Failed to load documents.',
  DOWNLOAD_FAILED: (fileName: string) => `Failed to download ${fileName}`,
  UPDATE_VISIBILITY_FAILED: 'Failed to update visibility.',
  UPDATE_VISIBILITY_SUCCESS: 'Document visibility updated successfully.',
  NO_SEARCH_RESULTS: 'No documents match your search.',

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
  UPLOAD_SUCCESS: 'Document(s) uploaded successfully.',
  UPLOAD_PARTIAL_FAILURE: (failedFileNames: string) => `Some files failed to upload: ${failedFileNames}`,

  // Remove Document Dialog
  REMOVE_FAILED: 'Failed to remove document.',
  REMOVE_SUCCESS: 'Document removed successfully.',

  // Upload Version Dialog
  UPLOAD_VERSION_FAILED: 'Failed to upload new version.',
  UPLOAD_VERSION_SUCCESS: 'New version uploaded successfully.',

  // Version History Dialog
  VERSION_HISTORY_LOAD_FAILED: 'Failed to load version history.',
  DOWNLOAD_VERSION_FAILED: (fileName: string) => `Failed to download ${fileName}`
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
    PRE_SALES: UserRoleId.PreSales,
    PROJECT_MANAGEMENT: EngagementRoleId.ProjectManager,
    TECH_LEAD: EngagementRoleId.TechLead,
    LEADERSHIP: UserRoleId.Leadership
  },
  AVAILABLE_CUSTOM_VISIBILITY_ROLES: [
    { id: UserRoleId.Leadership, name: 'Leadership', label: 'Leadership' },
    { id: EngagementRoleId.TechLead, name: 'Tech Lead', label: 'Tech Lead' },
    { id: UserRoleId.PreSales, name: 'Pre Sales', label: 'Pre Sales' }
  ]
} as const;
