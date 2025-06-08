using Microsoft.EntityFrameworkCore;
using VetClinicManager.Data;
using VetClinicManager.DTOs.VisitUpdates;
using VetClinicManager.Mappers;
using VetClinicManager.Models;

namespace VetClinicManager.Services;

public class VisitUpdateService : IVisitUpdateService
{
    private readonly ApplicationDbContext _context;
    private readonly VisitUpdateMapper _mapper;
    private readonly IFileService _fileService;
    private readonly IAnimalMedicationService _animalMedicationService;

    public VisitUpdateService(
        ApplicationDbContext context, 
        VisitUpdateMapper mapper, 
        IFileService fileService, 
        IAnimalMedicationService animalMedicationService)
    {
        _context = context;
        _mapper = mapper;
        _fileService = fileService;
        _animalMedicationService = animalMedicationService;
    }

    public async Task<VisitUpdateEditVetDto?> GetForEditAsync(int id, string vetId)
    {
        var visitUpdate = await _context.VisitUpdates.AsNoTracking()
            .Include(vu => vu.AnimalMedications)
            .FirstOrDefaultAsync(vu => vu.Id == id);
            
        if (visitUpdate == null || visitUpdate.UpdatedByVetId != vetId) return null;
        
        return _mapper.ToVisitUpdateEditVetDto(visitUpdate);
    }
    
    public async Task<VisitUpdateDeleteDto?> GetForDeleteAsync(int id, string vetId)
    {
        var visitUpdate = await _context.VisitUpdates.AsNoTracking().FirstOrDefaultAsync(vu => vu.Id == id);
        if (visitUpdate == null || visitUpdate.UpdatedByVetId != vetId) return null;
        
        return _mapper.ToDeleteDto(visitUpdate);
    }
    
    public async Task<int> CreateAsync(VisitUpdateCreateDto createDto, string vetId)
    {
        var visit = await _context.Visits.AsNoTracking().FirstOrDefaultAsync(v => v.Id == createDto.VisitId);
        if (visit == null) throw new KeyNotFoundException("Wizyta nie istnieje.");

        if (createDto.ImageFile != null)
        {
            createDto.ImageUrl = await _fileService.SaveFileAsync(createDto.ImageFile, "uploads/attachments");
        }

        var visitUpdate = _mapper.ToVisitUpdate(createDto);
        visitUpdate.UpdatedByVetId = vetId;
        visitUpdate.UpdateDate = DateTime.UtcNow;

        _context.VisitUpdates.Add(visitUpdate);
        await _context.SaveChangesAsync();

        if (createDto.AnimalMedications != null && createDto.AnimalMedications.Any())
        {
            foreach (var medDto in createDto.AnimalMedications.Where(m => m.MedicationId > 0))
            {
                medDto.AnimalId = visit.AnimalId;
                medDto.VisitUpdateId = visitUpdate.Id;
                await _animalMedicationService.CreateAnimalMedicationAsync(medDto);
            }
        }
        
        return visitUpdate.VisitId;
    }

    public async Task<int> UpdateAsync(int id, VisitUpdateEditVetDto editDto, string vetId)
    {
        var visitUpdateInDb = await _context.VisitUpdates
            .Include(vu => vu.AnimalMedications)
            .FirstOrDefaultAsync(vu => vu.Id == id);
            
        if (visitUpdateInDb == null) throw new KeyNotFoundException("Aktualizacja wizyty nie istnieje.");
        if (visitUpdateInDb.UpdatedByVetId != vetId) throw new UnauthorizedAccessException("Brak uprawnień do edycji tej aktualizacji.");

        if (editDto.ImageFile != null)
        {
            _fileService.DeleteFile(visitUpdateInDb.ImageUrl);
            editDto.ImageUrl = await _fileService.SaveFileAsync(editDto.ImageFile, "uploads/attachments");
        }

        visitUpdateInDb.Notes = editDto.Notes;
        visitUpdateInDb.ImageUrl = editDto.ImageUrl ?? visitUpdateInDb.ImageUrl;
        visitUpdateInDb.UpdateDate = DateTime.UtcNow;
        
        if (editDto.RemovedMedicationIds != null)
        {
            foreach (var medId in editDto.RemovedMedicationIds)
            {
                await _animalMedicationService.DeleteAnimalMedicationAsync(medId);
            }
        }
        if (editDto.ExistingAnimalMedications != null)
        {
            foreach (var medDto in editDto.ExistingAnimalMedications)
            {
                await _animalMedicationService.UpdateAnimalMedicationAsync(medDto);
            }
        }
        if (editDto.NewAnimalMedications != null)
        {
            var visit = await _context.Visits.AsNoTracking().FirstOrDefaultAsync(v => v.Id == visitUpdateInDb.VisitId);
            foreach (var newMedDto in editDto.NewAnimalMedications.Where(m => m.MedicationId > 0))
            {
                newMedDto.VisitUpdateId = id;
                newMedDto.AnimalId = visit!.AnimalId;
                await _animalMedicationService.CreateAnimalMedicationAsync(newMedDto);
            }
        }
        
        await _context.SaveChangesAsync();
        return visitUpdateInDb.VisitId;
    }

    public async Task<int> DeleteAsync(int id, string vetId)
    {
        var visitUpdate = await _context.VisitUpdates
            .Include(vu => vu.AnimalMedications)
            .FirstOrDefaultAsync(vu => vu.Id == id);
            
        if (visitUpdate == null) throw new KeyNotFoundException("Aktualizacja wizyty nie istnieje.");
        if (visitUpdate.UpdatedByVetId != vetId) throw new UnauthorizedAccessException("Brak uprawnień do usunięcia tej aktualizacji.");
        
        _fileService.DeleteFile(visitUpdate.ImageUrl);
        _context.AnimalMedications.RemoveRange(visitUpdate.AnimalMedications);
        _context.VisitUpdates.Remove(visitUpdate);
        await _context.SaveChangesAsync();
        
        return visitUpdate.VisitId;
    }
}