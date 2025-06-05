using Microsoft.EntityFrameworkCore;
using VetClinicManager.Areas.Admin.DTOs.Medications;
using VetClinicManager.Areas.Admin.Mappers;
using VetClinicManager.Data;

namespace VetClinicManager.Services;

public class MedicationService : IMedicationService
{
    private readonly ApplicationDbContext _context;
    private readonly MedicationMapper _medicationMapper;

    public MedicationService(ApplicationDbContext context, MedicationMapper medicationMapper)
    {
        _context = context;
        _medicationMapper = medicationMapper;
    }

    // Dla akcji Index GET
    public async Task<List<MedicationListDto>> GetAllMedicationsAsync()
    {
        var medications = await _context.Medications.ToListAsync();
        var medicationListDtos = _medicationMapper.ToMedicationListDtos(medications).ToList();
        
        return medicationListDtos;
    }

    // Dla akcji Details GET (reuse DeleteDto dla prostoty)
    public async Task<MedicationDeleteDto?> GetMedicationForDetailsAsync(int id)
    {
        var medication = await _context.Medications.FirstOrDefaultAsync(m => m.Id == id);

        if (medication == null)
        {
            return null;
        }
        
        var detailsDto = _medicationMapper.ToMedicationDeleteDto(medication);
        
        return detailsDto;
    }


    // Dla akcji Create POST
    public async Task<MedicationListDto> CreateMedicationAsync(MedicationCreateDto createDto)
    {
        var medication = _medicationMapper.ToMedication(createDto);
        _context.Add(medication);
        await _context.SaveChangesAsync();
        var resultDto = _medicationMapper.ToMedicationListDto(medication);
        
        return resultDto;
    }

    // Dla akcji Edit GET
    public async Task<MedicationEditDto?> GetMedicationForEditAsync(int id)
    {
        var medication = await _context.Medications.FindAsync(id);
        if (medication == null) return null;
        var editDto = _medicationMapper.ToMedicationEditDto(medication);
        
        return editDto;
    }

    // Dla akcji Edit POST
    public async Task UpdateMedicationAsync(MedicationEditDto editDto)
    {
        var medication = await _context.Medications.FindAsync(editDto.Id);
        
        if (medication == null)
        {
             throw new KeyNotFoundException($"Medication with ID {editDto.Id} not found.");
        }

        _medicationMapper.UpdateMedicationFromDto(editDto, medication);
        await _context.SaveChangesAsync();
    }

    // Dla akcji Delete GET
    public async Task<MedicationDeleteDto?> GetMedicationForDeleteAsync(int id)
    {
         var medication = await _context.Medications.FindAsync(id);
         
         if (medication == null)
         {
             return null;
         }
         
         var deleteDto = _medicationMapper.ToMedicationDeleteDto(medication); 
         
         return deleteDto;
    }

    // Dla akcji Delete POST
    public async Task DeleteMedicationAsync(int id)
    {
        var medication = await _context.Medications.FindAsync(id);
        
        if (medication == null)
        {
             return;
        }

        // TODO: W tym miejscu można by dodać logikę walidacji przed usunięciem (sprawdzanie powiązanych danych)

        _context.Medications.Remove(medication); 
        await _context.SaveChangesAsync();
    }
    
    // Dla metody pomocniczej
    public async Task<bool> MedicationExistsAsync(int id)
    {
        return await _context.Medications.AnyAsync(e => e.Id == id);
    }
}