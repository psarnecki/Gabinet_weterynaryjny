using System.ComponentModel.DataAnnotations;

namespace VetClinicManager.Areas.Admin.DTOs.Medications;

public class MedicationListDto
{
    [Required]
    public int Id { get; set; }
    
    [Display(Name = "Nazwa")]
    public string Name { get; set; } = string.Empty;
}