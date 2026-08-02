using Microsoft.AspNetCore.Hosting;
using Pdr.Api.DTOs.Documents;
using Pdr.Api.Models.Documents;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pdr.Api.Services;

public class DocumentService(
    IDocumentRepository repository,
    IWebHostEnvironment environment) : IDocumentService
{
    private const string UploadsFolder = "Uploads/Documents";

    public async Task<DocumentResponseDto> UploadDocumentAsync(DocumentUploadDto dto)
    {
        var engagementFolder = Path.Combine(environment.ContentRootPath, UploadsFolder, dto.EngagementId.ToString());
        if (!Directory.Exists(engagementFolder))
        {
            Directory.CreateDirectory(engagementFolder);
        }

        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(dto.File.FileName)}";
        var filePath = Path.Combine(engagementFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await dto.File.CopyToAsync(stream);
        }

        var document = new Document
        {
            EngagementId = dto.EngagementId,
            CategoryId = dto.CategoryId,
            FileName = dto.File.FileName,
            FilePath = filePath,
            FileSizeBytes = dto.File.Length,
            ContentType = string.IsNullOrWhiteSpace(dto.File.ContentType) 
                ? "application/octet-stream" 
                : dto.File.ContentType
        };

        var saved = await repository.AddAsync(document);
        var reloaded = await repository.GetByIdAsync(saved.Id) ?? saved;

        return MapToResponseDto(reloaded);
    }

    public async Task<IEnumerable<DocumentResponseDto>> GetDocumentsByEngagementAsync(int engagementId)
    {
        var docs = await repository.GetByEngagementIdAsync(engagementId);
        return docs.Select(MapToResponseDto);
    }

    public async Task<(Stream FileStream, string ContentType, string FileName)?> DownloadDocumentAsync(int id)
    {
        var doc = await repository.GetByIdAsync(id);
        if (doc == null || doc.IsDeleted || !File.Exists(doc.FilePath))
        {
            return null;
        }

        var stream = new FileStream(doc.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return (stream, doc.ContentType, doc.FileName);
    }

    public async Task<IEnumerable<DocumentCategoryDto>> GetCategoriesAsync()
    {
        var categories = await repository.GetCategoriesAsync();
        return categories.Select(c => new DocumentCategoryDto
        {
            Id = c.Id,
            CategoryName = c.CategoryName
        });
    }

    public async Task<bool> DeleteDocumentAsync(int id)
    {
        return await repository.SoftDeleteAsync(id);
    }

    private static DocumentResponseDto MapToResponseDto(Document doc)
    {
        return new DocumentResponseDto
        {
            Id = doc.Id,
            EngagementId = doc.EngagementId,
            CategoryId = doc.CategoryId,
            CategoryName = doc.Category?.CategoryName ?? string.Empty,
            FileName = doc.FileName,
            FileSizeBytes = doc.FileSizeBytes,
            ContentType = doc.ContentType,
            UploadedBy = doc.CreatedBy ?? string.Empty, // Automatically populated by AppDbContext
            UploadedAt = doc.CreatedAt
        };
    }
}
