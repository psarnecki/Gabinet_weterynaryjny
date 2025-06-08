using System.ComponentModel.DataAnnotations;
using VetClinicManager.DTOs.AnimalMedications;
using VetClinicManager.Models;
namespace VetClinicManager.DTOs.VisitUpdates;

public class VisitUpdateEditVetDto
{
    public int Id { get; set; }

    [Display(Name = "Notatki")]
    public string? Notes { get; set; }
    
    [Display(Name = "Adres URL obrazu")]
    public string? ImageUrl { get; set; }
    
    [Display(Name = "Załącznik (obraz)")]
    public IFormFile? ImageFile { get; set; }
    
    public List<AnimalMedicationEditVetDto> ExistingAnimalMedications { get; set; } = new();
    
    public List<AnimalMedicationCreateVetDto> NewAnimalMedications { get; set; } = new();
    
    public List<int> RemovedMedicationIds { get; set; } = new();
}