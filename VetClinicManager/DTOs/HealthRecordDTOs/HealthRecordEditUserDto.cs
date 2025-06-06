namespace VetClinicManager.DTOs.HealthRecordDTOs;

public class HealthRecordEditUserDto
{
    public int Id { get; set; }
    
    public bool IsSterilized { get; set; }
    
    public string? ChronicDiseases { get; set; }
    
    public string? Allergies { get; set; }
    
    public string? Vaccinations { get; set; }
    public DateTime LastVaccinationDate { get; set; }
}