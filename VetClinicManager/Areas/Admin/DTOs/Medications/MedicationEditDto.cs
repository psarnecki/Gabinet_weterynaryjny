using System.ComponentModel.DataAnnotations;

namespace VetClinicManager.Areas.Admin.DTOs.Medications;

public class MedicationEditDto
{
    [Required] 
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Nazwa leku/materiału jest wymagana.")]
    [MaxLength(150)]
    [Display(Name = "Nazwa")]
    public string Name { get; set; } = string.Empty;
}