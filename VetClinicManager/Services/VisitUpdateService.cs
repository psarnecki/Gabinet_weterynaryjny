using Microsoft.EntityFrameworkCore;
using VetClinicManager.Data;
using VetClinicManager.DTOs.VisitUpdateDTOs;
using VetClinicManager.Interfaces;
using VetClinicManager.Models;

namespace VetClinicManager.Services;

public class VisitUpdateService : IVisitUpdateService
{
    private readonly ApplicationDbContext _context;

    public VisitUpdateService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<VisitUpdate>> GetVisitUpdatesAsync()
    {
        return await _context.VisitUpdates
            .Include(v => v.Visit)
            .ToListAsync();
    }

    public async Task<VisitUpdate> GetVisitUpdateByIdAsync(int id)
    {
        return await _context.VisitUpdates
            .Include(v => v.Visit)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<VisitUpdate> CreateVisitUpdateAsync(VisitUpdateCreateDto createDto, string vetId)
    {
        var visitUpdate = new VisitUpdate
        {
            Notes = createDto.Notes,
            UpdateDate = DateTime.UtcNow,
            ImageUrl = createDto.ImageUrl,
            PrescribedMedications = createDto.PrescribedMedications,
            VisitId = createDto.VisitId,
            UpdatedByVetId = vetId,
            AnimalMedications = createDto.AnimalMedications
        };

        _context.VisitUpdates.Add(visitUpdate);
        await _context.SaveChangesAsync();
        return visitUpdate;
    }

    public async Task<VisitUpdate> UpdateVisitUpdateAsync(int id, VisitUpdateEditVetDto updateDto, string vetId)
    {
        var existingUpdate = await _context.VisitUpdates.FindAsync(id);
        if (existingUpdate == null) return null;

        if (existingUpdate.UpdatedByVetId != vetId)
            throw new UnauthorizedAccessException("Only the vet who created the update can modify it.");

        existingUpdate.Notes = updateDto.Notes;
        existingUpdate.ImageUrl = updateDto.ImageUrl;
        existingUpdate.PrescribedMedications = updateDto.PrescribedMedications;
        existingUpdate.UpdateDate = DateTime.UtcNow;
        existingUpdate.AnimalMedications = updateDto.AnimalMedications;

        _context.Update(existingUpdate);
        await _context.SaveChangesAsync();
        return existingUpdate;
    }

    public async Task DeleteVisitUpdateAsync(int id)
    {
        var visitUpdate = await _context.VisitUpdates.FindAsync(id);
        if (visitUpdate != null)
        {
            _context.VisitUpdates.Remove(visitUpdate);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> VisitUpdateExistsAsync(int id)
    {
        return await _context.VisitUpdates.AnyAsync(e => e.Id == id);
    }
}