using System.ComponentModel.DataAnnotations;

namespace VetClinicManager.Models.Enums;

public enum Gender
{
        [Display(Name = "Samiec")]
        Male,
        
        [Display(Name = "Samica")]
        Female,
        
        [Display(Name = "Nieznana")] 
        Unknown
}