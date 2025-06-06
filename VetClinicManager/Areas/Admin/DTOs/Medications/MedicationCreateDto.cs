using System.ComponentModel.DataAnnotations;

namespace VetClinicManager.Areas.Admin.DTOs.Medications;

public class MedicationCreateDto
{
    [Required(ErrorMessage = "Nazwa leku/materiału jest wymagana.")]
    [MaxLength(150)]
    [Display(Name = "Nazwa")]
    public string Name { get; set; } = string.Empty;
}