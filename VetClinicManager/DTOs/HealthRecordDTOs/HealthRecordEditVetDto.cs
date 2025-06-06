using System.ComponentModel.DataAnnotations;
using VetClinicManager.Models;

namespace VetClinicManager.DTOs.HealthRecordDTOs;

public class HealthRecordEditVetDto
{
    public int Id { get; set; }
    
    public string? ChronicDiseases { get; set; }
    
    public string? Allergies { get; set; }
    
    public string? Vaccinations { get; set; }
    public DateTime LastVaccinationDate { get; set; }
}