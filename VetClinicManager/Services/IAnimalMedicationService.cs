using Microsoft.AspNetCore.Mvc.Rendering;
using VetClinicManager.DTOs.AnimalMedications;
using VetClinicManager.Models;

namespace VetClinicManager.Services;

public interface IAnimalMedicationService 
{
    Task<List<AnimalMedication>> GetAnimalMedicationsAsync();
    Task<AnimalMedication?> GetAnimalMedicationByIdAsync(int id);
    Task<AnimalMedicationEditVetDto?> GetForEditAsync(int id);
    Task CreateAnimalMedicationAsync(AnimalMedicationCreateVetDto dto);
    Task UpdateAnimalMedicationAsync(AnimalMedicationEditVetDto dto);
    Task DeleteAnimalMedicationAsync(int id);
    Task<bool> AnimalMedicationExistsAsync(int id);
    Task<SelectList> GetAnimalsSelectListAsync();
    Task<SelectList> GetMedicationsSelectListAsync();
}