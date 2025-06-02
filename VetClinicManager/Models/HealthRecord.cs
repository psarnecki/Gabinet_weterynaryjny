namespace VetClinicManager.Models;

public class HealthRecord
{
    public int Id { get; set; }
    public int AnimalId { get; set; }
    public Animal Animal { get; set; }
    
    public bool IsSterilized { get; set; }
    public string? ChronicDiseases { get; set; }
    public string? Allergies { get; set; }
    public string? Vaccinations { get; set; }
    public DateTime LastVaccinationDate { get; set; }
}