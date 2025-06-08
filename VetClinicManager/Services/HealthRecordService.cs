using Microsoft.EntityFrameworkCore;
using VetClinicManager.Data;
using VetClinicManager.DTOs.HealthRecords;
using VetClinicManager.Mappers;
using VetClinicManager.Models;
using VetClinicManager.Services; // Popraw namespace, jeśli trzeba

namespace VetClinicManager.Services;

public class HealthRecordService : IHealthRecordService
{
    private readonly ApplicationDbContext _context;
    private readonly HealthRecordMapper _mapper;

    public HealthRecordService(ApplicationDbContext context, HealthRecordMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<string?> GetAnimalNameForCreateAsync(int animalId)
    {
        var animal = await _context.Animals.AsNoTracking().Include(a => a.HealthRecord).FirstOrDefaultAsync(a => a.Id == animalId);
        if (animal == null || animal.HealthRecord != null) return null;
        return animal.Name;
    }
    
    public async Task<HealthRecordEditVetDto?> GetForEditAsync(int id)
    {
        var entity = await _context.HealthRecords.AsNoTracking().FirstOrDefaultAsync(h => h.Id == id);
        if (entity == null) return null;
        
        var dto = _mapper.ToEditDto(entity);
        dto.AnimalId = entity.AnimalId; // Ręcznie przypisujemy AnimalId
        return dto;
    }

    public async Task<int> CreateAsync(HealthRecordEditVetDto createDto)
    {
        var entity = _mapper.ToEntity(createDto);
        // AnimalId jest już w DTO, więc mapper powinien je przenieść.
        _context.Add(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(HealthRecordEditVetDto editDto)
    {
        var entityInDb = await _context.HealthRecords.FindAsync(editDto.Id);
        if (entityInDb == null) return false;

        entityInDb.IsSterilized = editDto.IsSterilized;
        entityInDb.ChronicDiseases = editDto.ChronicDiseases;
        entityInDb.Allergies = editDto.Allergies;
        entityInDb.Vaccinations = editDto.Vaccinations;
        entityInDb.LastVaccinationDate = editDto.LastVaccinationDate;
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<HealthRecordEditVetDto?> GetForDeleteAsync(int id)
    {
        return await GetForEditAsync(id); // Możemy reużyć tę samą metodę
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.HealthRecords.FindAsync(id);
        if (entity == null) return false;
        _context.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}