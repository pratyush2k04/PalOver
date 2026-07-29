using Pdr.Api.DTOs.Notes;
using Pdr.Api.Models;

namespace Pdr.Api.Services.Interfaces;

public interface INoteService
{
    Task<IEnumerable<Note>> GetAllAsync();
    Task<Note?> GetByIdAsync(int id);
    Task<Note> CreateAsync(CreateNoteRequest request);
}
