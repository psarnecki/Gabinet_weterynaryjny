using VetClinicManager.Models;
namespace VetClinicManager.DTOs.VisitUpdates;

public class VisitUpdateEditVetDto
{
    public int Id { get; set; }

    public string? Notes { get; set; }
    
    public string? ImageUrl { get; set; }
    
    public string? PrescribedMedications { get; set; }
    
    public ICollection<AnimalMedication> AnimalMedications { get; set; } = new List<AnimalMedication>();
}