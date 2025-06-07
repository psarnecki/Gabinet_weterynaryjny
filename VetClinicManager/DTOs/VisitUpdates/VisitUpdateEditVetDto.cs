using VetClinicManager.DTOs.AnimalMedications;
using VetClinicManager.Models;
namespace VetClinicManager.DTOs.VisitUpdates;

public class VisitUpdateEditVetDto
{
    public int Id { get; set; }

    public string? Notes { get; set; }
    
    public string? ImageUrl { get; set; }
    
    public List<AnimalMedicationEditVetDto> ExistingAnimalMedications { get; set; } = new();
    
    public List<AnimalMedicationCreateVetDto> NewAnimalMedications { get; set; } = new();
    
    public List<int> RemovedMedicationIds { get; set; } = new();
}