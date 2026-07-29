using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Notes;
using Pdr.Api.Models;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotesController(INoteService noteService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Note>>> GetAll() =>
        Ok(await noteService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Note>> GetById(int id)
    {
        var note = await noteService.GetByIdAsync(id);
        return note is null ? NotFound() : Ok(note);
    }

    [HttpPost]
    public async Task<ActionResult<Note>> Create(CreateNoteRequest request)
    {
        var note = await noteService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = note.Id }, note);
    }
}
