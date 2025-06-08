using System.ComponentModel.DataAnnotations;

namespace VetClinicManager.DTOs.HealthRecords;

public class HealthRecordEditVetDto
{
    public int Id { get; set; }
    
    public int AnimalId { get; set; } 
    
    [Display(Name = "Czy sterylizowane/kastrowane")]
    public bool IsSterilized { get; set; }
    
    [Display(Name = "Choroby przewlekłe")]
    public string? ChronicDiseases { get; set; }
    
    [Display(Name = "Alergie")]
    public string? Allergies { get; set; }
    
    [Display(Name = "Szczepienia")]
    public string? Vaccinations { get; set; }
    
    [Display(Name = "Data ostatniego szczepienia")]
    [DataType(DataType.Date)]
    public DateTime LastVaccinationDate { get; set; }
}