using System.ComponentModel.DataAnnotations;

namespace VetClinicManager.Models.Enums;

public enum VisitStatus
{
    [Display(Name = "Zaplanowana")]
    Scheduled,
    
    [Display(Name = "W trakcie")]
    InProgress,
    
    [Display(Name = "Zakończona")]
    Completed,
    
    [Display(Name = "Anulowana")]
    Cancelled
}