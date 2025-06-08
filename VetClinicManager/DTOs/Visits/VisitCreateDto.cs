using System.ComponentModel.DataAnnotations;
using VetClinicManager.Models.Enums;
using VetClinicManager.DTOs.Visits.VisitBriefs;

namespace VetClinicManager.DTOs.Visits;

public class VisitCreateDto
{
    [Display(Name = "Tytuł / Powód Wizyty")]
    [Required(ErrorMessage = "Tytuł wizyty jest wymagany.")]
    public string Title { get; set; } = string.Empty;
    
    [Display(Name = "Opis / Szczegóły")]
    public string? Description { get; set; }
    
    [Display(Name = "Status Wizyty")]
    public VisitStatus Status { get; set; }
    
    [Display(Name = "Priorytet")]
    public VisitPriority Priority { get; set; }
    
    [Display(Name = "Zwierzę")]
    [Required(ErrorMessage = "Należy wybrać zwierzę.")]
    public int AnimalId { get; set; }
    public VisitAnimalBriefDto? Animal { get; set; }
    
    [Display(Name = "Przypisany Weterynarz")]
    public string? AssignedVetId { get; set; }
    public VisitVetBriefDto? AssignedVet { get; set; }
    public List<VisitUpdateBriefDto> Updates { get; set; } = new();
}
