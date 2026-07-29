export interface Note {
  id: number;
  title: string;
  content: string | null;
  createdAt: string;
}

export interface CreateNoteRequest {
  title: string;
  content: string | null;
}
