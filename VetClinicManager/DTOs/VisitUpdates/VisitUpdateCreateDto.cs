using System.ComponentModel.DataAnnotations;
using VetClinicManager.Models;
using VetClinicManager.DTOs.AnimalMedications;

namespace VetClinicManager.DTOs.VisitUpdates;

public class VisitUpdateCreateDto
{
    [Display(Name = "Notatki")]
    public string? Notes { get; set; }
    
    [Display(Name = "Adres URL obrazu")]
    public string? ImageUrl { get; set; }
    
    [Display(Name = "Załącznik (obraz)")]
    public IFormFile? ImageFile { get; set; }
    public int VisitId { get; set; } 
    
    [System.Text.Json.Serialization.JsonIgnore]
    public Visit? Visit { get; set; }
    
    [System.Text.Json.Serialization.JsonIgnore]
    public User? UpdatedBy { get; set; }
    
    [Display(Name = "Przepisane leki")]
    public List<AnimalMedicationCreateVetDto> AnimalMedications { get; set; } = new();
}