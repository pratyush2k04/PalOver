using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models;

namespace Pdr.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Note>>> GetAll()
    {
        var notes = await db.Notes.OrderByDescending(n => n.CreatedAt).ToListAsync();
        return Ok(notes);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Note>> GetById(int id)
    {
        var note = await db.Notes.FindAsync(id);
        return note is null ? NotFound() : Ok(note);
    }

    [HttpPost]
    public async Task<ActionResult<Note>> Create(CreateNoteRequest request)
    {
        var note = new Note { Title = request.Title, Content = request.Content };
        db.Notes.Add(note);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = note.Id }, note);
    }
}

public record CreateNoteRequest(string Title, string? Content);
