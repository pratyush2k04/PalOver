import { DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterOutlet } from '@angular/router';
import { Note } from './notes/note.model';
import { NotesService } from './notes/notes.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, FormsModule, DatePipe],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  private readonly notesService = inject(NotesService);

  protected readonly title = signal('PDR');
  protected readonly notes = signal<Note[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);

  protected newTitle = '';
  protected newContent = '';

  constructor() {
    this.loadNotes();
  }

  loadNotes(): void {
    this.loading.set(true);
    this.error.set(null);
    this.notesService.getNotes().subscribe({
      next: (notes) => {
        this.notes.set(notes);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Failed to load notes. Is the API running?');
        this.loading.set(false);
        console.error(err);
      }
    });
  }

  addNote(): void {
    if (!this.newTitle.trim()) {
      return;
    }
    this.notesService
      .createNote({ title: this.newTitle.trim(), content: this.newContent.trim() || null })
      .subscribe({
        next: () => {
          this.newTitle = '';
          this.newContent = '';
          this.loadNotes();
        },
        error: (err) => {
          this.error.set('Failed to create note.');
          console.error(err);
        }
      });
  }
}
