using VetClinicManager.Models;

namespace VetClinicManager.DTOs.VisitUpdateDTOs;

public class VisitUpdateCreateDto
{

    public string? Notes { get; set; }
    public DateTime UpdateDate { get; set; } = DateTime.UtcNow;
    
    public string? ImageUrl { get; set; }
    
    public string? PrescribedMedications { get; set; }
    
    public int VisitId { get; set; }
    public Visit Visit { get; set; }

    public string UpdatedByVetId { get; set; }
    public User UpdatedBy { get; set; }
    
    public ICollection<AnimalMedication> AnimalMedications { get; set; } = new List<AnimalMedication>();
}