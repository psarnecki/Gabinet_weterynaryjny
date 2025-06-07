using Microsoft.EntityFrameworkCore;
using VetClinicManager.Data;
using VetClinicManager.DTOs.VisitUpdates;
using VetClinicManager.Interfaces;
using VetClinicManager.Models;

namespace VetClinicManager.Services
{
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
                .AsNoTracking()
                .Include(v => v.Visit)
                .ToListAsync();
        }

        public async Task<VisitUpdate> GetVisitUpdateByIdAsync(int id)
        {
            return await _context.VisitUpdates
                .AsNoTracking()
                .Include(v => v.Visit)
                .Include(v => v.AnimalMedications)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<VisitUpdate> CreateVisitUpdateAsync(VisitUpdateCreateDto createDto, string vetId)
        {
            var visitUpdate = new VisitUpdate
            {
                Notes = createDto.Notes,
                ImageUrl = createDto.ImageUrl,
                PrescribedMedications = createDto.PrescribedMedications,
                VisitId = createDto.VisitId,
                UpdatedByVetId = vetId,
                UpdateDate = DateTime.UtcNow
            };
            
            _context.VisitUpdates.Add(visitUpdate);
            await _context.SaveChangesAsync();
            return visitUpdate;
        }

        public async Task<VisitUpdate> UpdateVisitUpdateAsync(int id, VisitUpdateEditVetDto updateDto, string vetId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingUpdate = await _context.VisitUpdates
                    .Include(vu => vu.AnimalMedications)
                    .FirstOrDefaultAsync(vu => vu.Id == id);

                if (existingUpdate == null) return null;
                
                if (existingUpdate.UpdatedByVetId != vetId)
                {
                    throw new UnauthorizedAccessException("Tylko weterynarz, który stworzył aktualizację, może ją modyfikować.");
                }

                // 1. Aktualizuj właściwości podstawowe
                existingUpdate.Notes = updateDto.Notes;
                existingUpdate.ImageUrl = updateDto.ImageUrl;
                existingUpdate.PrescribedMedications = updateDto.PrescribedMedications;
                existingUpdate.UpdateDate = DateTime.UtcNow;

                // 2. Usuń leki
                if (updateDto.RemovedMedicationIds != null && updateDto.RemovedMedicationIds.Any())
                {
                    var medicationsToRemove = existingUpdate.AnimalMedications
                        .Where(am => updateDto.RemovedMedicationIds.Contains(am.Id)).ToList();
                    _context.AnimalMedications.RemoveRange(medicationsToRemove);
                }

                // 3. Zaktualizuj istniejące leki
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

                // 4. Dodaj nowe leki - użyj AnimalId z DTO ustawionego w kontrolerze
                if (updateDto.NewAnimalMedications != null)
                {
                    foreach (var newMedDto in updateDto.NewAnimalMedications.Where(m => m.MedicationId > 0))
                    {
                        if (newMedDto.AnimalId <= 0)
                        {
                            throw new InvalidOperationException("AnimalId nie zostało poprawnie ustawione dla nowego leku.");
                        }

                        var newAnimalMedication = new AnimalMedication
                        {
                            MedicationId = newMedDto.MedicationId,
                            StartDate = newMedDto.StartDate,
                            EndDate = newMedDto.EndDate,
                            VisitUpdateId = existingUpdate.Id,
                            AnimalId = newMedDto.AnimalId // Używamy wartości z DTO
                        };
                        _context.AnimalMedications.Add(newAnimalMedication);
                    }
                }
                
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Zwróć świeżo pobrany obiekt dla pewności
                return await _context.VisitUpdates
                    .AsNoTracking()
                    .Include(vu => vu.Visit)
                    .FirstOrDefaultAsync(vu => vu.Id == id);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // W pliku VetClinicManager/Services/VisitUpdateService.cs

        public async Task DeleteVisitUpdateAsync(int id)
        {
            // Znajdź VisitUpdate do usunięcia, ale dołącz też powiązane leki.
            var visitUpdateToDelete = await _context.VisitUpdates
                .Include(vu => vu.AnimalMedications) // <-- Kluczowy dodatek
                .FirstOrDefaultAsync(vu => vu.Id == id);

            if (visitUpdateToDelete != null)
            {
                // Jeśli istnieją powiązane leki, usuń je najpierw.
                // EF jest na tyle sprytny, że jeśli usuniesz rodzica,
                // a dzieci są załadowane, to usunie też dzieci.
                // Ale dla pewności możemy zrobić to jawnie:
                if (visitUpdateToDelete.AnimalMedications.Any())
                {
                    _context.AnimalMedications.RemoveRange(visitUpdateToDelete.AnimalMedications);
                }

                // Teraz usuń samego rodzica - VisitUpdate
                _context.VisitUpdates.Remove(visitUpdateToDelete);

                // Zapisz wszystkie zmiany (usunięcie leków i aktualizacji) w jednej transakcji.
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> VisitUpdateExistsAsync(int id)
        {
            return await _context.VisitUpdates.AnyAsync(e => e.Id == id);
        }
    }
}