using System.ComponentModel.DataAnnotations;

namespace VetClinicManager.DTOs.VisitUpdates;

public class VisitUpdateDeleteDto
{
    public int Id { get; set; } 
    
    public int VisitId { get; set; }

    [Display(Name = "Data aktualizacji")]
    public DateTime UpdateDate { get; set; }

    [Display(Name = "Notatki")]
    public string? Notes { get; set; }
}