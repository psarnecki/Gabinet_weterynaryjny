using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VetClinicManager.Data;
using VetClinicManager.DTOs.AnimalMedications;
using VetClinicManager.Mappers;
using VetClinicManager.Models;

namespace VetClinicManager.Services;

public class AnimalMedicationService : IAnimalMedicationService
{
    private readonly ApplicationDbContext _context;
    private readonly AnimalMedicationMapper _mapper;

    public AnimalMedicationService(
        ApplicationDbContext context,
        AnimalMedicationMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<AnimalMedication>> GetAnimalMedicationsAsync()
    {
        return await _context.AnimalMedications
            .Include(a => a.Animal)
            .Include(a => a.Medication)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<AnimalMedication?> GetAnimalMedicationByIdAsync(int id)
    {
        return await _context.AnimalMedications
            .Include(a => a.Animal)
            .Include(a => a.Medication)
            .FirstOrDefaultAsync(m => m.Id == id);
    }
    
    public async Task<AnimalMedicationEditVetDto?> GetForEditAsync(int id)
    {
        var entity = await _context.AnimalMedications.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        
        if (entity == null)
        {
            return null;
        }
        
        return _mapper.MapToEditDto(entity);
    }

    public async Task CreateAnimalMedicationAsync(AnimalMedicationCreateVetDto dto)
    {
        var animalMedication = _mapper.MapToAnimalMedication(dto);
        _context.Add(animalMedication);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAnimalMedicationAsync(AnimalMedicationEditVetDto dto)
    {
        var animalMedication = _mapper.MapToAnimalMedication(dto);
        _context.Update(animalMedication);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAnimalMedicationAsync(int id)
    {
        var animalMedication = await _context.AnimalMedications.FindAsync(id);
        if (animalMedication != null)
        {
            _context.AnimalMedications.Remove(animalMedication);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> AnimalMedicationExistsAsync(int id)
    {
        return await _context.AnimalMedications.AnyAsync(e => e.Id == id);
    }

    public async Task<SelectList> GetAnimalsSelectListAsync()
    {
        var animals = await _context.Animals.ToListAsync();
        return new SelectList(animals, "Id", "Name");
    }

    public async Task<SelectList> GetMedicationsSelectListAsync()
    {
        var medications = await _context.Medications.ToListAsync();
        return new SelectList(medications, "Id", "Name");
    }
}