using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VetClinicManager.Models;

public class HealthRecord
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [ForeignKey("Animal")]
    public int AnimalId { get; set; }
    public Animal Animal { get; set; }
    
    [Display(Name = "Czy sterylizowane/kastrowane")]
    public bool IsSterilized { get; set; }
    
    [Display(Name = "Choroby przewlekłe")]
    [MaxLength(500)]
    public string? ChronicDiseases { get; set; }
    
    [Display(Name = "Alergie")]
    [MaxLength(500)]
    public string? Allergies { get; set; }
    
    [Display(Name = "Szczepienia")]
    [MaxLength(500)]
    public string? Vaccinations { get; set; }
    
    [Display(Name = "Data ostatniego szczepienia")]
    [DataType(DataType.Date)]
    public DateTime LastVaccinationDate { get; set; }
}