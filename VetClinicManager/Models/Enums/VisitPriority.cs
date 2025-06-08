using System.ComponentModel.DataAnnotations;

namespace VetClinicManager.Models.Enums;

public enum VisitPriority
{
    [Display(Name = "Normalny")]
    Normal,
    
    [Display(Name = "Pilny")]
    Urgent,
    
    [Display(Name = "Krytyczny")]
    Critical
}