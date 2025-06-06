using VetClinicManager.Models;

namespace VetClinicManager.DTOs.VisitUpdates;

public class VisitUpdateCreateDto
{
    public string? Notes { get; set; }
    public string? ImageUrl { get; set; }
    public string? PrescribedMedications { get; set; }
    public int VisitId { get; set; } 
    
    [System.Text.Json.Serialization.JsonIgnore]
    public Visit? Visit { get; set; }
    
    [System.Text.Json.Serialization.JsonIgnore]
    public User? UpdatedBy { get; set; }
}