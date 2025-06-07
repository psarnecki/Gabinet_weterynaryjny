// Plik: VetClinicManager/Services/VisitUpdateService.cs

using Microsoft.EntityFrameworkCore;
using VetClinicManager.Data;
using VetClinicManager.DTOs.VisitUpdates;
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
            .AsSplitQuery() // Opcjonalna optymalizacja
            .ToListAsync();
    }

    public async Task<VisitUpdate> GetVisitUpdateByIdAsync(int id)
    {
        return await _context.VisitUpdates
            .Include(v => v.Visit)
            .Include(v => v.AnimalMedications)
            .AsSplitQuery() // Opcjonalna optymalizacja
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<VisitUpdate> CreateVisitUpdateAsync(VisitUpdateCreateDto createDto, string vetId)
    {
        var visit = await _context.Visits.FindAsync(createDto.VisitId);
        var vet = await _context.Users.FindAsync(vetId);

        if (visit == null || vet == null)
        {
            throw new ArgumentException("Nie znaleziono wizyty lub weterynarza");
        }

        var visitUpdate = new VisitUpdate
        {
            Notes = createDto.Notes,
            ImageUrl = createDto.ImageUrl,
            PrescribedMedications = createDto.PrescribedMedications,
            VisitId = createDto.VisitId,
            UpdatedByVetId = vetId,
            UpdateDate = DateTime.UtcNow
        };
        // Nie musisz ustawiać właściwości nawigacyjnych 'Visit' i 'UpdatedBy'
        // Entity Framework zrobi to automatycznie na podstawie kluczy obcych

        _context.VisitUpdates.Add(visitUpdate);
        await _context.SaveChangesAsync();
    
        // Zwracamy z pełnymi danymi nawigacyjnymi
        return await _context.VisitUpdates
            .Include(vu => vu.Visit)
            .Include(vu => vu.UpdatedBy)
            .FirstAsync(vu => vu.Id == visitUpdate.Id);
    }


    public async Task<VisitUpdate> UpdateVisitUpdateAsync(int id, VisitUpdateEditVetDto updateDto, string vetId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var existingUpdate = await _context.VisitUpdates
                .Include(vu => vu.Visit)
                .Include(vu => vu.AnimalMedications)
                .FirstOrDefaultAsync(vu => vu.Id == id);

            if (existingUpdate == null) return null;
            if (existingUpdate.Visit == null)
            {
                throw new InvalidOperationException($"Wizyta dla aktualizacji o ID {id} nie została znaleziona.");
            }
            if (existingUpdate.UpdatedByVetId != vetId)
            {
                throw new UnauthorizedAccessException("Tylko weterynarz, który stworzył aktualizację, może ją modyfikować.");
            }

                  // 2. Aktualizuj właściwości podstawowe
        existingUpdate.Notes = updateDto.Notes;
        existingUpdate.ImageUrl = updateDto.ImageUrl;
        existingUpdate.PrescribedMedications = updateDto.PrescribedMedications;
        existingUpdate.UpdateDate = DateTime.UtcNow;

        // 3. Usuń leki
        if (updateDto.RemovedMedicationIds != null && updateDto.RemovedMedicationIds.Any())
        {
            var medicationsToRemove = existingUpdate.AnimalMedications
                .Where(am => updateDto.RemovedMedicationIds.Contains(am.Id)).ToList();
            _context.AnimalMedications.RemoveRange(medicationsToRemove);
        }

        // 4. Zaktualizuj istniejące leki
        if (updateDto.ExistingAnimalMedications != null)
        {
            foreach (var medicationDto in updateDto.ExistingAnimalMedications)
            {
                var existingMedication = existingUpdate.AnimalMedications.FirstOrDefault(am => am.Id == medicationDto.Id);
                if (existingMedication != null)
                {
                    existingMedication.MedicationId = medicationDto.MedicationId;
                    existingMedication.StartDate = medicationDto.StartDate;
                    existingMedication.EndDate = medicationDto.EndDate;
                }
            }
        }

        // 5. Dodaj nowe leki
        if (updateDto.NewAnimalMedications != null && updateDto.NewAnimalMedications.Any())
        {
            var animalId = existingUpdate.Visit.AnimalId; 
            foreach (var newMedicationDto in updateDto.NewAnimalMedications)
            {
                if (newMedicationDto.MedicationId > 0)
                {
                    var newAnimalMedication = new AnimalMedication
                    {
                        MedicationId = newMedicationDto.MedicationId,
                        StartDate = newMedicationDto.StartDate,
                        EndDate = newMedicationDto.EndDate,
                        VisitUpdateId = existingUpdate.Id,
                        AnimalId = animalId
                    };
                    _context.AnimalMedications.Add(newAnimalMedication);
                }
            }
        }
        
        // --- ZMIANA ZACZYNA SIĘ TUTAJ ---
        
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        // Zamiast zwracać "stary" obiekt `existingUpdate`,
        // pobierz go ponownie z bazy, aby mieć pewność, że jest kompletny.
        // Użyj `AsNoTracking`, bo nie będziemy go już modyfikować.
        return await _context.VisitUpdates
            .AsNoTracking() // Dobra praktyka, gdy dane są tylko do odczytu
            .Include(vu => vu.Visit)
            .Include(vu => vu.UpdatedBy)
            .Include(vu => vu.AnimalMedications)
                .ThenInclude(am => am.Medication)
            .FirstOrDefaultAsync(vu => vu.Id == id);
    }
    catch (Exception)
    {
        await transaction.RollbackAsync();
        throw;
    }
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