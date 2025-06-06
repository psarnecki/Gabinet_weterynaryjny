using VetClinicManager.Models;

namespace VetClinicManager.DTOs.AnimalMedications;

public class AnimalMedicationEditVetDto
{
    public int Id { get; set; }
    
    public int MedicationId { get; set; }
    public Medication Medication { get; set; }
    
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}