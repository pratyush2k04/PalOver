using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.DTOs.Notes;
using Pdr.Api.Models;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class NoteService(AppDbContext db) : INoteService
{
    public async Task<IEnumerable<Note>> GetAllAsync() =>
        await db.Notes.OrderByDescending(n => n.CreatedAt).ToListAsync();

    public async Task<Note?> GetByIdAsync(int id) =>
        await db.Notes.FindAsync(id);

    public async Task<Note> CreateAsync(CreateNoteRequest request)
    {
        var note = new Note { Title = request.Title, Content = request.Content };
        db.Notes.Add(note);
        await db.SaveChangesAsync();
        return note;
    }
}
