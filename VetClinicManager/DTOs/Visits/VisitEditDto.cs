using System.ComponentModel.DataAnnotations;
using VetClinicManager.Models.Enums;
using VetClinicManager.DTOs.Visits.VisitBriefs;

namespace VetClinicManager.DTOs.Visits;

public class VisitEditDto
{
    public int Id { get; set; }
    
    [Display(Name = "Tytuł")]
    [Required(ErrorMessage = "Tytuł wizyty jest wymagany.")]
    public string Title { get; set; } = string.Empty;
    
    [Display(Name = "Opis")]
    public string? Description { get; set; }
    
    [Display(Name = "Status")]
    [Required(ErrorMessage = "Status wizyty jest wymagany.")] 
    public VisitStatus Status { get; set; }
    
    [Display(Name = "Priorytet")]
    [Required(ErrorMessage = "Priorytet wizyty jest wymagany.")]
    public VisitPriority Priority { get; set; }
    
    [Display(Name = "Zwierzę")]
    public VisitAnimalBriefDto? Animal { get; set; }
    
    [Display(Name = "Przypisany Weterynarz")]
    public string? AssignedVetId { get; set; }
    public VisitVetBriefDto? AssignedVet { get; set; }
    public List<VisitUpdateBriefDto> Updates { get; set; } = new();
}